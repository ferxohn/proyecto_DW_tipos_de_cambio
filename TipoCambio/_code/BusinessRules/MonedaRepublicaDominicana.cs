using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TipoCambio.Classes;

namespace TipoCambio.BusinessRules
{
    // La clase MonedaRepublicaDominicana hereda a la clase RequestsDivisas.
    class MonedaRepublicaDominicana : RequestsDivisas
    {
        /* Atributos de la clase. */
        // Para obtener el XLS, no hay parametros en este caso
        private readonly IList<string> url_excel = null;

        // Constructor de la clase.
        public MonedaRepublicaDominicana()
        {
            // Se inicializa cada una de las listas de URL y parametros a usar.
            url_excel = new List<string>
            {
                "https",
                "gdc.bancentral.gov.do/Common/public/estadisticas/mercado-cambiario//documents//DOLAR_VENTANILLA_SONDEO.xls",
                "GET",
                "XLS"
            };
        }

        // Metodo ObtenerHoy se sobreescribe con XLS_Hoy.
        public override IList<string> ObtenerHoy() => XLS_Hoy();

        /* Metodo que permite obtener el tipo de cambio de hoy.
         * Regresa la lista de valores que se subiran a la BD.
         */
        public IList<string> XLS_Hoy()
        {
            // El atributo objetoFecha almacena la fecha de hoy.
            objetoFecha = DateTime.Today;

            // Si el dia pertenece al fin de semana, se regresa una lista con tipo de cambio en 0.
            if (VerificarFecha() != 0)
            {
                Console.WriteLine("Se obtuvo el tipo de cambio de República Dominicana correctamente.");
                return CrearListaBD("0", "0", "DOP");
            }

            // Se realiza la peticion web y se regresa la lista con el tipo de cambio.
            return EstandarWebRequest();
        }

        // Metodo ObtenerFecha se sobreescribe con XLS_Fecha.
        public override IList<string> ObtenerFecha(string fecha) => XLS_Fecha(fecha);

        /* Metodo que permite obtener el tipo de cambio de una fecha especifica.
         * Regresa la lista de valores que se subiran a la BD.
         */
        public IList<string> XLS_Fecha(string fecha)
        {
            // Declaracion e inicializacion de variables.
            int resultadoFecha = VerificarFecha(fecha);

            // Se verifica la fecha ingresada.
            if (resultadoFecha <= 0)
            {
                // Caso de dia en fin de semana.
                if (resultadoFecha == -1)
                {
                    Console.WriteLine("Se obtuvo el tipo de cambio de República Dominicana correctamente.");
                    return CrearListaBD("0", "0", "DOP");
                }
            }

            else
            {
                Console.WriteLine("Error al obtener el tipo de cambio de República Dominicana.");
                return null;
            }

            // Se realiza la peticion web y se regresa la lista con el tipo de cambio.
            return EstandarWebRequest();
        }

        /* Metodo que realizar toda la parte estandar del Web Request (Para los metodos XLS_Hoy y XLS_Fecha).
         * Regresa la lista de valores lista para ser subida la Base de Datos.
         */
        private IList<string> EstandarWebRequest()
        {
            // Se comprueba la existencia del archivo para evitar conflictos.
            if (EliminarArchivo() > 0)
            {
                Console.WriteLine("Error al obtener el tipo de cambio de República Dominicana.");
                return null;
            }

            // Se ejecuta el metodo WebRequestXLS que hace todo el trabajo, verificando su resultado.
            if (WebRequestXLS(url_excel, "datos_republica_dominicana", "Tasa Sondeo, Ventanilla, Dólar") != 0)
            {
                Console.WriteLine("Error al obtener el tipo de cambio de República Dominicana.");
                return null;
            }

            // Se elimina el archivo obtenido. No es necesario comprobarlo.
            EliminarArchivo();

            // Finalmente se crea y regresa la lista de valores que se subiran a la BD.
            return CrearLista();
        }

        /* Metodo que permite crear la lista de valores que se subiran a la BD. (Para los metodos Excel_Hoy y Excel_Fecha).
         * Notar el uso de la funcion CrearListaBD de la clase RequestsDivisas.
         */
        private IList<string> CrearLista()
        {
            // Declaracion e inicializacion de variables.
            IList<string> formaFecha = null;
            string tipoCambioCompra = "0";
            string tipoCambioVenta = "0";

            // Se verifica el tipo de cambio obtenido. Si es valido, se almacena.
            if (objetoRequest != null)
            {
                // Primero se obtienen los parametros necesarios para obtener el valor del XLS.
                formaFecha = FormatoFecha();

                // Como se tiene una lista, se recorre y verifica hasta encontrar el valor deseado.
                foreach (var data in objetoRequest)
                {
                    if (data[0] == formaFecha[0] && data[1] == formaFecha[1] && data[2] == formaFecha[2])
                    {
                        tipoCambioCompra = data["F4"].ToString();
                        tipoCambioVenta = data["F5"].ToString();
                    }
                }
            }

            // Si los tipos de cambio se mantienen en 0, se regresa una lista con tipo de cambio 0.
            if (tipoCambioCompra == "0" && tipoCambioVenta == "0")
            {
                Console.WriteLine("Se obtuvo el tipo de cambio de República Dominicana correctamente.");
                return CrearListaBD("0", "0", "DOP");
            }

            // Se crea y regresa la lista de valores que se subiran a la BD.
            Console.WriteLine("Se obtuvo el tipo de cambio de República Dominicana correctamente.");
            return CrearListaBD(tipoCambioCompra, tipoCambioVenta, "DOP");
        }

        // Metodo para darle formato a la fecha y poder obtener los valores del XLS.
        private IList<string> FormatoFecha()
        {
            // Declaracion e inicializacion de variables.
            IList<string> fecha = null;
            string mes_rd = null;
            string dia_rd = objetoFecha.Day.ToString();
            string year_rd = objetoFecha.Year.ToString();

            // Se busca el caso especifico del mes para convertirlo.
            switch (objetoFecha.Month.ToString())
            {
                case "1":
                    mes_rd = "Ene";
                    break;
                case "2":
                    mes_rd = "Feb";
                    break;
                case "3":
                    mes_rd = "Mar";
                    break;
                case "4":
                    mes_rd = "Abr";
                    break;
                case "5":
                    mes_rd = "May";
                    break;
                case "6":
                    mes_rd = "Jun";
                    break;
                case "7":
                    mes_rd = "Jul";
                    break;
                case "8":
                    mes_rd = "Ago";
                    break;
                case "9":
                    mes_rd = "Sep";
                    break;
                case "10":
                    mes_rd = "Oct";
                    break;
                case "11":
                    mes_rd = "Nov";
                    break;
                default:
                    mes_rd = "Dic";
                    break;
            }

            // Finalmente se guarda la lista con la fecha desglosada y formateada.
            fecha = new List<string>
            {
                year_rd,
                mes_rd,
                dia_rd
            };

            return fecha;
        }
    }
}