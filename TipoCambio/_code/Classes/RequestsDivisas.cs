using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using Newtonsoft.Json;

namespace TipoCambio.Classes
{
    /* Clase abstracta para usar en todas las monedas y poder obtener sus tipos de cambio.
     * Esta clase, al ser abstracta, no puede ser instanciada, solamente puede ser heredada.
     */
    abstract class RequestsDivisas
    {
        // Atributos de la clase.
        protected string respuestaRequest = null;
        protected dynamic objetoRequest = null;
        protected DateTime objetoFecha;

        /* Metodo que permite realizar el Web Request. 
         * La lista datos_url debe contener lo siguiente:
         *  datos_url[0] = Protocolo (http - https, por ejemplo)
         *  datos_url[1] = La URL.
         *  datos_url[2] = El metodo HTTP a usar (GET - POST, por ejemplos)
         *  
         *  Los siguientes parametros NO aplican para GET:
         *  datos_url[3] = Content Type de la peticion (application/xml, por ejemplo).
         *  
         *  Las listas parameters y values deben contener, como su nombre lo indica, los parametros usados en
         *  el Web Request y sus valores.
         *      Ejemplos: "fecha_inicio=12/09/2017&fecha_fin=12/10/2018"
         *      parameters = [fecha_inicio, fecha_fin]
         *      values = [12/09/2017, 12/10/2018]
         *
         * Regresa un entero de verificacion.
        */
        protected int RequestWeb(IList<string> datos_url, IList<string> parameters, IList<string> values)
        {
            // Declaracion e inicializacion de variables.
            int ejecucion = 0;
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            // Inicio de la url a usar.
            string url = datos_url[0] + "://" + datos_url[1];

            try
            {
                // Se crea la URL a partir de la lista de valores y parametros (Si el metodo es GET).
                if (datos_url[2] == "GET")
                {
                    for (int i = 0; i < parameters.Count; i++)
                    {
                        url += parameters[i] + "=" + values[i];

                        if (i < parameters.Count - 1)
                        {
                            url = url + "&";
                        }
                    }

                    // Se hace la peticion y se almacena el resultado como cadena en el atributo respuestaRequest.
                    request = (HttpWebRequest)WebRequest.Create(url);
                    response = (HttpWebResponse)request.GetResponse();
                    respuestaRequest = new StreamReader(response.GetResponseStream()).ReadToEnd();
                }

                // Se crea la lista de parametros a pasar hacia la URL (Si el metodo es POST).
                else if (datos_url[2] == "POST")
                {
                    byte[] data = null;

                    // Se crea la cadena de parametros POST y se codifica en data.
                    string postString = "";

                    for (int i = 0; i < parameters.Count; i++)
                    {
                        postString += parameters[i] + "=" + values[i];

                        if (i < parameters.Count - 1)
                        {
                            postString = postString + "&";
                        }
                    }

                    data = Encoding.ASCII.GetBytes(postString);

                    /* Se hace la peticion y se almacena el resultado como cadena en el atributo respuestaRequest.
                     * Notar el uso de los parametros para hacer la peticion.
                     */
                    request = (HttpWebRequest)WebRequest.Create(url);
                    request.Method = datos_url[2];
                    request.ContentType = datos_url[3];
                    request.ContentLength = data.Length;

                    using (var stream = request.GetRequestStream())
                    {
                        stream.Write(data, 0, data.Length);
                    }

                    response = (HttpWebResponse)request.GetResponse();
                    respuestaRequest = new StreamReader(response.GetResponseStream()).ReadToEnd();
                }

                Console.WriteLine("Se ejecutó la petición web correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al ejecutar la petición web: " + ex.Message);
                ejecucion = 1;
            }

            return ejecucion;
        }

        /* Metodo que permite validar el formato de fecha ingresado.
         * Recordar que el formato de fecha de entrada debe ser: dd/MM/yyyy
         * Regresa un entero de verificacion.
         */
        protected int ValidarFecha(string fecha)
        {
            //DateTime.TryParse permite comprobar que el formato de fecha ingresado sea correcto.
            if (DateTime.TryParse(fecha, out objetoFecha))
            {
                // Como verificacion extra, se comprueba que la fecha ingresada no sea futura.
                if (objetoFecha > DateTime.Today)
                {
                    Console.WriteLine("No es posible buscar una fecha futura.");
                    return 2;
                }

                return 0;
            }

            else
            {
                Console.WriteLine("Se ha ingresado una fecha incorrecta.");
                return 1;
            }
        }

        /* Metodo que permite deserializar un string en formato JSON.
         * Regresa un entero de verificacion.
         */
        protected int DeserializarJSON()
        {
            int ejecucion = 0;

            // Se realiza la deserializacion del JSON.
            try
            {
                objetoRequest = JsonConvert.DeserializeObject(respuestaRequest);
                Console.WriteLine("Se convirtió el JSON correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al convertir el JSON: " + ex.Message);
                ejecucion = 2;
            }

            return ejecucion;
        }

        /* Metodo que permite crear una lista con los valores obtenidos de un tipo de cambio (Formato de acuerdo a los campos en la BD).
         * Notar el uso del atributo objetoFecha para ingresar la fecha a la lista.
         */
        protected IList<string> CrearListaIndividual(string compra, string venta, string moneda)
        {
            IList<string> salida = new List<string>()
            {
                objetoFecha.ToString("yyyyMMdd"),
                compra,
                venta,
                moneda,
            };

            return salida;
        }
        /* Metodo WebRequestJSON para realizar un Web Request a una fuente de tipo JSON.
         * Recibe la lista de parametros explicada en el metodo Requestweb.
         * Regresa un entero de verificacion.
         */
        protected int WebRequestJSON(IList<string> datos_url, IList<string> parameters, IList<string> values)
        {
            // Se ejecuta y verifica el Web Request.
            if (RequestWeb(datos_url, parameters, values) == 0)
            {
                // Se deserializa el JSON, verificando el resultado de la funcion.
                if (DeserializarJSON() != 0)
                {
                    return 2;
                }

                return 0;
            }

            else
            {
                return 1;
            }
        }

        protected int WebRequestCSV(IList<string> datos_url, IList<string> parameters, IList<string> values)
        {
            if (RequestWeb(datos_url, parameters, values) == 0)
            {

                return 0;
            }

            return 1;
        }

        /* Metodo abstracto con el cual es posible obtener el tipo de cambio de hoy.
         * Regresa la lista con los valores a subir a la BD
         */
        public abstract IList<string> ObtenerHoy();

        /* Metodo abstracto con el cual es posible obtener el tipo de cambio de un dia en especifico.
         * Regresa la lista con los valores a subir a la BD
         */
        public abstract IList<string> ObtenerFecha(string fecha);

        /* Metodo abstracto con el cual es posible obtener los tipos de cambio de un periodo.
         * Regresa una lista con las listas que contienen los valores a subir a la BD.
         */
        //public abstract IList<IList> ObtenerPeriodo(string fecha_inicio, string fecha_final);
    }
}
