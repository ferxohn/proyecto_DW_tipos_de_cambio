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
                "GET",
                "FINANCE"
            };

            url_soap = new List<string>
            {
                "https",
                "www.superfinanciera.gov.co/SuperfinancieraWebServiceTRM/TCRMServicesWebService/TCRMServicesWebService?WSDL",
                "POST",
                "SOAP",
                "text/xml"
            };

            parameters_soap = new List<string> 
            {
                "queryTCRM"
            };
        }

        // Metodo ObtenerHoy se sobreescribe con JSON_Hoy.
        public override IList<string> ObtenerHoy() => JSON_Hoy();

        /* Metodo que permite obtener el tipo de cambio de hoy.
        * Regresa la lista de valores que se subiran a la BD.
        */
        private IList<string> JSON_Hoy()
        {
            // El atributo objetoFecha almacena la fecha de hoy.
            objetoFecha = DateTime.Today;

            // Si el dia pertenece al fin de semana, se regresa una lista con tipo de cambio en 0.
            if (VerificarFecha() != 0)
            {
                Console.WriteLine("Se obtuvo el tipo de cambio de Colombia correctamente.");
                return CrearListaBD("0", "0", "COP");
            }

            // Se ejecuta el metodo WebRequestJSON que hace todo el trabajo de peticion web, verificando su resultado.
            if (WebRequestJSON(url_google) != 0)
            {
                Console.WriteLine("Error al obtener el tipo de cambio de Colombia.");
                return null;
            }

            // Finalmente se crea y regresa la lista de valores que se subiran a la BD.
            Console.WriteLine("Se obtuvo el tipo de cambio de Colombia correctamente.");
            return CrearListaBD("0", (string)objetoRequest["PriceUpdate"][0][0][1][0][3], "COP");
        }

        // Metodo ObtenerFecha se sobreescribe con SOAP_Fecha.
        public override IList<string> ObtenerFecha(string fecha) => SOAP_Fecha(fecha);

        /* Metodo que permite obtener el tipo de cambio de una fecha especifica.
        * Regresa la lista de valores que se subiran a la BD.
        */
        private IList<string> SOAP_Fecha(string fecha)
        {
            // Declaracion e inicializacion de variables.
            int resultadoFecha = VerificarFecha(fecha);

            // Se verifica la fecha ingresada.
            if (resultadoFecha <= 0)
            {
                // Caso de dia en fin de semana.
                if (resultadoFecha == -1)
                {
                    Console.WriteLine("Se obtuvo el tipo de cambio de Colombia correctamente.");
                    return CrearListaBD("0", "0", "COP");
                }
            }

            else
            {
                Console.WriteLine("Error al obtener el tipo de cambio de Colombia.");
                return null;
            }

            // Se genera la lista de valores.
            IList<string> values = new List<String>
            {
                objetoFecha.ToString("yyyy-MM-dd")
            };

            return null;
        }
    }
}
