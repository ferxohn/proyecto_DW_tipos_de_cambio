using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using HtmlAgilityPack;
using CsvHelper;
using LinqToExcel;
using System.Xml.Linq;

namespace TipoCambio.Classes
{
    /* Esta clase permite realizar peticiones web de distintos formatos de archivo de origen.
     * Leer la documentacion del metodo RequestWeb para ver los formatos implementados actualmente,
     * y las reglas de uso de la clase.
     */
    class PeticionesWeb
    {
        // Atributos de la clase.
        protected dynamic respuestaRequest = null;
        protected dynamic objetoRequest = null;
        // Atributos especiales de la clase (Para el formato HTML).
        HtmlDocument requestHTML = null;

        /* Metodos que sirven de interfaz estandar para distintas peticiones */

        /* Metodo WebRequestJSON para realizar un Web Request a una fuente de tipo JSON.
         * Recibe parte de los parametros explicados en el metodo RequestWeb.
         * Regresa un entero de verificacion.
         */
        protected int WebRequestJSON(IList<string> datos_url, IList<string> parameters = null, IList<string> values = null)
        {
            // Se ejecuta y verifica el Web Request.
            if (RequestWeb(datos_url, parameters, values) == 0)
            {
                // Se deserializa el JSON, verificando el resultado de la funcion.
                if (DeserializarJSON() != 0)
                {
                    return 1;
                }

                return 0;
            }

            else
            {
                return 1;
            }
        }

        /* Metodo WebRequestHTML para realizar un Web Request a una fuente de tipo HTML.
         * Recibe parte de los parametros explicados en el metodo RequestWeb.
         * Regresa un entero de verificacion.
         */
        protected int WebRequestHTML(IList<string> datos_url, IList<string> parameters = null, IList<string> values = null)
        {
            // Se ejecuta y verifica el Web Request.
            if (RequestWeb(datos_url, parameters, values) == 0)
            {
                // Se deserializa el HTML, verificando el resultado de la funcion.
                return DeserializarHTML();
            }

            else
            {
                return 1;
            }
        }

        /* Metodo WebRequestXLS para realizar un Web Request a una fuente de tipo XLS.
         * Recibe parte de los parametros explicados en el metodo RequestWeb.
         *  El nombre del archivo no lleva la extension ".xls", se agrega automaticamente en el metodo RequestWeb
         * Regresa un entero de verificacion.
         */
        protected int WebRequestXLS(IList<string> datos_url, string nombrearchivo, string nombretabla, IList<string> parameters = null, IList<string> values = null)
        {
            // Se ejecuta y verifica el Web Request.
            if (RequestWeb(datos_url, parameters, values, nombrearchivo) == 0)
            {
                if (DeserializarXLS(nombretabla) != 0)
                {
                    return 1;
                }

                return 0;
            }
            else
            {
                return 1;
            }
        }

        /* Metodo principal para realizar las Web Requests (Metodo RequestWeb) */

        /* Metodo que permite realizar todo el Web Request.
         * Recibe la lista de datos de la URL, y las listas de parametros y valores (opcionales)
         * y el nombre del archivo a descargar sin la extension del archivo (opcional, aplica para XLS).
         * 
         *  La lista datos_url debe contener lo siguiente:
         *   datos_url[0] = Protocolo a usar (http - https, por ejemplo)
         *   datos_url[1] = La URL de origen, hasta antes del primer parametro dinamico.
         *   datos_url[2] = El metodo HTTP a usar.
         *       Opciones implementadas:
         *           - GET
         *           - POST
         *   datos_url[3] = El formato de origen a recibir.
         *       Opciones implementadas:
         *           - JSON
         *           - XML
         *           - XLS
         *           - HTML
         *           - CSV
         *           - FINANCE (Google Finance en JSON)
         *       
         *       NOTA PARA APLICACION SOAP: No es necesario agregarla en esta lista. Se puede agregar el enlace a su WSDL
         *       desde Explorador de soluciones -> Referencias (Boton derecho sobre la opcion) -> Agregar referencia de servicio...
         *       y anadiendo el enlace mencionado junto con un nombre personalizado para su Espacio de nombres.
         *       Tambien comprobar que la direccion que se agrega en la etiqueta "endpoint address" de App.config sea la correcta.
         *   
         *   Los siguientes parametros NO aplican para GET:
         *   datos_url[4] = Content Type de la peticion (application/xml, por ejemplo).
         *   
         *   Las listas parameters y values deben contener, como su nombre lo indica, los parametros usados en
         *   el Web Request y sus respectivos valores.
         *       Ejemplo: "fecha_inicio=12/09/2017&fecha_fin=12/10/2018"
         *       parameters = [fecha_inicio, fecha_fin]
         *       values = [12/09/2017, 12/10/2018]
         *
         * Regresa un entero de verificacion.
        */
        protected int RequestWeb(IList<string> datos_url, IList<string> parameters = null, IList<string> values = null, string nombrearchivo = null)
        {
            // Declaracion e inicializacion de variables.
            string url = null;
            byte[] postData = null;
            // Ubicacion de la carpeta de Descargas y la ruta completa del archivo a descargar (Para archivos XLS).
            string downloadsPath = null;
            string destino = null;
            // Distintas variables para recibir la respuesta de un request antes de tratarla para almacenarla en respuestaRequest.
            dynamic requestGeneral = null;

            try
            {
                // Implementacion de los metodos HTTP.
                switch (datos_url[2])
                {
                    // Metodo GET.
                    case "GET":
                        // Se crea la URL a partir del protocolo, la lista de valores y parametros.
                        url = FormarURL(datos_url[0], datos_url[1], parameters, values);

                        /* Se implementan las funciones de los formatos de origen */
                        switch (datos_url[3])
                        {
                            // Formato HTML.
                            case "HTML":
                                // objetoRequest se inicializa como lista.
                                objetoRequest = new List<string>();
                                // Se hace la peticion a partir de la URL recibida.
                                requestHTML = new HtmlWeb().Load(url);

                                /* Se almacena el resultado como cadena en el atributo respuestaRequest. 
                                 * La peticion se hace como una consulta de las tablas en la pagina.
                                 */
                                respuestaRequest = from table in requestHTML.DocumentNode.SelectNodes("//table").Cast<HtmlNode>()
                                                   from row in table.SelectNodes("tr").Cast<HtmlNode>()
                                                   from cambio in row.SelectNodes("td").Cast<HtmlNode>()
                                                   select new { Table = table.Id, CambioText = cambio.InnerText };

                                break;

                            // Formato XLS.
                            case "XLS":
                                // Se almacena en respuestaRequest el nombre del archivo.
                                respuestaRequest = nombrearchivo + ".xls";

                                // Se obtiene la ruta de Descargas, y se genera la ruta completa del archivo a descargar.
                                downloadsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
                                destino = Path.Combine(downloadsPath, respuestaRequest);

                                // Se descarga el archivo XLS.
                                requestGeneral = new WebClient();
                                requestGeneral.DownloadFile(url, destino);

                                break;

                            // Formatp XML.
                            case "XML":
                                // Se hace la peticion del XML.
                                objetoRequest = XElement.Load(url);
                                
                                break;

                            // Opcion default para el resto de los formatos que no requieren tratamiento especial.
                            default:
                                // Se hace la peticion y se almacena el resultado como cadena en el atributo respuestaRequest.
                                requestGeneral = (HttpWebRequest)WebRequest.Create(url);
                                objetoRequest = (HttpWebResponse)requestGeneral.GetResponse();
                                respuestaRequest = new StreamReader(objetoRequest.GetResponseStream()).ReadToEnd();

                                // Particularidad del formato FINANCE (Google Finance).
                                if (datos_url[3] == "FINANCE")
                                {
                                    // Se recibe un JSON, cuyos primeros 5 valores deben ser eliminados para no generar problemas en la deserializacion.
                                    respuestaRequest = respuestaRequest.Substring(5);
                                }

                                break;
                        }

                        break;

                    // Metodo POST.
                    case "POST":
                        // Se crea la URL a partir del protocolo, la lista de valores y parametros.
                        url = FormarURL(datos_url[0], datos_url[1], parameters, values, out postData);

                        /* Se implementan las funciones de los formatos de origen */
                        switch (datos_url[3])
                        {
                            // Opcion default para el resto de los formatos que no requieren tratamiento especial.
                            default:
                                /* Se hace la peticion y se almacena el resultado como cadena en el atributo respuestaRequest.
                                 * Notar el uso de los parametros para hacer la peticion.
                                 */
                                requestGeneral = (HttpWebRequest)WebRequest.Create(url);
                                // Parametro del metodo HTTP.
                                requestGeneral.Method = "POST";
                                // Parametro del Content Type (application/xml, por ejemplo).
                                requestGeneral.ContentType = datos_url[4];
                                // Parametro del tamanio de postData.
                                requestGeneral.ContentLength = postData.Length;

                                // Se hace la peticion y se almacena el resultado como cadena en el atributo respuestaRequest.
                                using (var stream = requestGeneral.GetRequestStream())
                                {
                                    stream.Write(postData, 0, postData.Length);
                                }

                                objetoRequest = (HttpWebResponse)requestGeneral.GetResponse();
                                respuestaRequest = new StreamReader(objetoRequest.GetResponseStream()).ReadToEnd();

                                break;
                        }

                        break;

                    // Si se ingresa un metodo HTTP que no esta implementado, se regresa un error.
                    default:
                        Console.WriteLine("No existe una función implementada para este método HTTP.");
                        return 1;
                }

                Console.WriteLine("Se ejecutó la petición web correctamente.");
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al ejecutar la petición web: " + ex.Message);
                return 1;
            }
        }

        /* Metodos para formar la URL: Uso de polimorfismo */

        /* Metodo que permite formar la URL para realizar una peticion GET.
         * Recibe la URL de origen y su protocolo y las listas de parametros y valores (opcionales).
         * Regresa la URL formada.
         */
        private string FormarURL(string protocol, string urlOrigen, IList<string> parameters = null, IList<string> values = null)
        {
            // Se une el protocolo con la URL de origen.
            string url = protocol + "://" + urlOrigen;

            // Antes de unir los parametros y valores a la URL, se verifica que la lista de parametros no sea nula.
            if (parameters != null)
            {
                for (int i = 0; i < parameters.Count; i++)
                {
                    // Se forma la URL ingresando el parametro con su valor correspondiente.
                    url += parameters[i] + "=" + values[i];

                    // Antes de ingresar nuevos parametros, se añade el simbolo "&".
                    if (i < parameters.Count - 1)
                    {
                        url += "&";
                    }
                }
            }

            return url;
        }

        /* Metodo que permite formar la URL y codificar la lista de parametros
         * y valores para realizar una peticion POST.
         * Recibe la URL de origen y su protocolo y las listas de parametros y valores (Valor "out" es una salida).
         * Regresa la URL formada y la codificacion en Bytes de la cadena de parametros (Usando "out").
         */
        private string FormarURL(string protocol, string urlOrigen, IList<string> parameters, IList<string> values, out byte[] postData)
        {
            // Se une el protocolo con la URL de origen.
            string url = protocol + "://" + urlOrigen;
            // Se crea la cadena de parametros POST y se codifica.
            string postString = "";

            for (int i = 0; i < parameters.Count; i++)
            {
                // Se forma la cadena ingresando el parametro con su valor correspondiente.
                postString += parameters[i] + "=" + values[i];

                // Antes de ingresar nuevos parametros, se añade el simbolo "&".
                if (i < parameters.Count - 1)
                {
                    postString = postString + "&";
                }
            }

            // Se codifica en Bytes la cadena creada.
            postData = Encoding.ASCII.GetBytes(postString);

            // Se regresa la cadena de parametros codificada en Bytes.
            return url;
        }

        /* Metodos para la deserializacion de distintos formatos */

        /* Metodo que permite deserializar un string en formato JSON.
         * Regresa un entero de verificacion.
         */
        protected int DeserializarJSON()
        {
            // Se realiza la deserializacion del JSON.
            try
            {
                objetoRequest = JsonConvert.DeserializeObject(respuestaRequest);
                Console.WriteLine("Se deserializó el JSON correctamente.");
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al deserializar el JSON: " + ex.Message);
                return 1;
            }
        }

        /* Metodo que permite deserializar un string en formato HTML.
         * Regresa un entero de verificacion.
         */
        protected int DeserializarHTML()
        {
            // Se realiza la deserializacion del HTML.
            try
            {
                // Se comprueba que existan nodos que deserializar.
                if (requestHTML.DocumentNode.SelectSingleNode("//table").SelectNodes("tr") != null)
                {
                    foreach (var cambio in respuestaRequest)
                    {
                        objetoRequest.Add(cambio.CambioText);
                    }

                    Console.WriteLine("Se deserializó el HTML correctamente.");
                    return 0;
                }

                else
                {
                    Console.WriteLine("No se encontraron nodos que deserializar");
                    return -1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al deserializar el HTML: " + ex.Message);
                return 1;
            }
        }

        /* Metodo que permite deserializar un archivo en formato XLS.
         * Recibe el nombre de la tabla, ya que el nombre del archivo se almacena en respuestaRequest.
         * Regresa un entero de verificacion.
         */
        protected int DeserializarXLS(string nombretabla)
        {
            // Declaracion e inicializacion de variables.
            ExcelQueryFactory archivoXLS = null;
            // Por defecto la carpeta a buscar es la carpeta de Descargas.
            string downloadsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");

            // Se realiza la deserializacion del XLS tomando una consulta de los valores.
            try
            {
                string acceso_archivo = "" + downloadsPath + "\\" + respuestaRequest;
                archivoXLS = new ExcelQueryFactory(acceso_archivo);
                objetoRequest = from data in archivoXLS.Worksheet(nombretabla) select data;
                Console.WriteLine("Se deserializó el Excel correctamente.");
                return 0;

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al deserializar el Excel: " + ex.Message);
                return 1;
            }
        }

        /* Metodo que permite deserializar un string en formato CSV.
         * Regresa un entero de verificacion.
         */
        protected int DeserializarCSV()
        {
            // Se realiza la deserializacion del CSV.
            try
            {
                objetoRequest = new CsvReader(new StringReader(respuestaRequest));
                Console.WriteLine("Se deserializó el CSV correctamente.");
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al deserializar el CSV: " + ex.Message);
                return 1;
            }
        }

        /* Metodo extra para los archivos XLS */

        /* Metodo que permite eliminar un archivo de la carpeta Descargas.
         * No recibe nada, ya que el nombre del archivo se almacena en respuestaRequest.
         * Regresa un entero de verificacion.
         */
        protected int EliminarArchivo()
        {
            // Por defecto la carpeta a buscar es la carpeta de Descargas.
            string downloadsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");

            // Se realiza la eliminacion del Archivo, primero verificando si existe.
            if (File.Exists(downloadsPath + "\\" + respuestaRequest))
            {
                try
                {
                    File.Delete(downloadsPath + "\\" + respuestaRequest);
                    Console.WriteLine("Se eliminó el archivo correctamente.");
                    return 0;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al ejecutar la eliminación del archivo, eliminarlo manualmente: " + ex.Message);
                    return 1;
                }
            }

            else
            {
                Console.WriteLine("Error al ejecutar la eliminación de archivo: No existe el archivo.");
                return -1;
            }
        }
    }
}
