using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TipoCambio.Classes;

namespace TipoCambio.BusinessRules
{
    // La clase MonedaColombia hereda a la clase RequestsDivisas.
    class MonedaColombia : RequestsDivisas
    {
        /* Atributos de la clase. */
        // Para obtener el JSON de Google Finance.
        private readonly IList<string> url_google = null;
        private readonly IList<string> parameters_google = null;
        // Para obtener el valor de la app SOAP.
        private readonly IList<string> url_soap = null;
        private readonly IList<string> parameters_soap = null;

        // Constructor de la clase.
        public MonedaColombia()
        {
            // Se inicializa cada una de las listas de URL y parametros a usar.
            url_google = new List<string>
            {
                "https",
                "www.google.com/async/finance_wholepage_price_updates?async=currencies:%2Fm%2F04lt7%5F%2B%2Fm%2F09nqf,_fmt:jspb",
                "GET"
            };

            parameters_google = new List<string> { };

            url_soap = new List<string>
            {
                "https",
                "www.superfinanciera.gov.co/SuperfinancieraWebServiceTRM/TCRMServicesWebService/TCRMServicesWebService?WSDL",
                "POST",
                "text/xml"
            };

            parameters_soap = new List<string> 
            {
                "queryTCRM"
            };
        }
        
        /* Metodo que permite crear la lista de valores que se subiran a la BD. (Para los metodos JSON_Hoy y JSON_Fecha).
        * Notar el uso de la funcion CrearListaIndividual de la clase RequestsDivisas.
        */
        private IList<string> CrearListaJSON()
        {
            // Declaracion e inicializacion de variables.
            IList<string> salida = null;

            return salida;
        }

        // Metodo ObtenerHoy se sobreescribe con JSON_Hoy.
        public override IList<string> ObtenerHoy() => JSON_Hoy();

        /* Metodo que permite obtener el tipo de cambio de hoy.
        * Regresa la lista de valores que se subiran a la BD.
        */
        private IList<string> JSON_Hoy()
        {
            // Declaracion e inicializacion de variables.
            IList<string> salida = null;

            // El atributo objetoFecha almacena la fecha de hoy.
            objetoFecha = DateTime.Today;

            // Se genera la lista de valores.
            IList<string> values = new List<String>
            {
                objetoFecha.ToString("dd/MM/yyyy")
            };

            // Se comprueba si la fecha actual es un fin de semana.
            if (FinSemana() != 0)
            {
                salida = CrearListaIndividual("0", "0", "COP");
                return salida;
            }

            // Se ejecuta y verifica el Web Request.
            if (RequestWeb(url_google, parameters_google, values) == 0)
            {
                respuestaRequest = respuestaRequest.Substring(5);

                // Se deserializa el JSON, verificando el resultado de la funcion.
                if (DeserializarJSON() != 0)
                {
                    return null;
                }
            }

            // Finalmente se crea y regresa la lista de valores que se subiran a la BD.
            salida = CrearListaIndividual("0", (string)objetoRequest["PriceUpdate"][0][0][1][0][3], "COP");

            Console.WriteLine("La ejecución de la función se completó de forma correcta.");
            return salida;
        }

        // Metodo ObtenerHoy se sobreescribe con JSON_Hoy.
        public override IList<string> ObtenerFecha(string fecha) => JSON_Fecha(fecha);

        /* Metodo que permite obtener el tipo de cambio de una fecha especifica.
        * Regresa la lista de valores que se subiran a la BD.
        */
        private IList<string> JSON_Fecha(string fecha)
        {
            // Declaracion e inicializacion de variables.
            IList<string> salida = null;

            // Se valida la fecha ingresada.
            if (ValidarFecha(fecha) != 0)
            {
                Console.WriteLine("Error al ejecutar la función. La ejecución no se completó de forma correcta.");
                return null;
            }

            // Se comprueba si la fecha ingresada es un fin de semana.
            if (FinSemana() != 0)
            {
                salida = CrearListaIndividual("0", "0", "COP");
                return salida;
            }

            // Se genera la lista de valores.
            IList<string> values = new List<String>
            {
                objetoFecha.ToString("yyyy-MM-dd")
            };

            // Se ejecuta el metodo WebRequestJSON que hace todo el trabajo, verificando su resultado.
            if (WebRequestJSON(url_soap, parameters_soap, values) != 0)
            {
                Console.WriteLine("Error al ejecutar la función. La ejecución no se completó de forma correcta.");
                return null;
            }

            // Finalmente se crea y regresa la lista de valores que se subiran a la BD.
            salida = CrearListaJSON();

            Console.WriteLine("La ejecución de la función se completó de forma correcta.");
            return salida;
        }
    }
}
