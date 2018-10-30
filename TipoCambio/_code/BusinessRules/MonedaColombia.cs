using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TipoCambio.Classes;
// Espacio de nombres de la aplicacion SOAP de Colombia, previamente referenciada como sevicio en el proyecto.
using TipoCambio.ColombiaSOAP;

namespace TipoCambio.BusinessRules
{
    // La clase MonedaColombia hereda a la clase RequestsDivisas.
    class MonedaColombia : RequestsDivisas
    {
        /* Atributos de la clase. */
        // Para obtener el JSON de Google Finance.
        private readonly IList<string> url_google = null;

        // Constructor de la clase.
        public MonedaColombia(string usuario): base(usuario)
        {
            // Se inicializa la lista para usar Google Finance y obtener el tipo de cambio de hoy.
            url_google = new List<string>
            {
                "https",
                "www.google.com/async/finance_wholepage_price_updates?async=currencies:%2Fm%2F09nqf%2B%2Fm%2F034sw6,_fmt:jspb",
                "GET",
                "FINANCE"
            };
        }

        // Metodo ObtenerFecha (Default) se sobreescribe con JSON_Hoy.
        public override IList<string> ObtenerFecha() => JSON_Hoy();

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
                Registros.Log.AgregarRegistro(user, "COP", "Se obtuvo el tipo de cambio de Colombia correctamente.");
                Console.WriteLine("Se obtuvo el tipo de cambio de Colombia correctamente.");
                return CrearListaBD("0", "0", "COP");
            }

            // Se ejecuta el metodo WebRequestJSON que hace todo el trabajo de peticion web, verificando su resultado.
            if (WebRequestJSON(url_google) != 0)
            {
                Registros.Log.AgregarRegistro(user, "COP", "Error al obtener el tipo de cambio de Colombia.");
                Console.WriteLine("Error al obtener el tipo de cambio de Colombia.");
                return null;
            }

            // Finalmente se crea y regresa la lista de valores que se subiran a la BD.
            Registros.Log.AgregarRegistro(user, "COP", "Se obtuvo el tipo de cambio de Colombia correctamente.");
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
            string tipoCambio = null;
            TCRMServicesInterfaceClient clientTCRM = null;

            // Se verifica la fecha ingresada.
            if (resultadoFecha <= 0)
            {
                // Caso de dia en fin de semana.
                if (resultadoFecha == -1)
                {
                    Registros.Log.AgregarRegistro(user, "COP", "Se obtuvo el tipo de cambio de Colombia correctamente.");
                    Console.WriteLine("Se obtuvo el tipo de cambio de Colombia correctamente.");
                    return CrearListaBD("0", "0", "COP");
                }
            }

            else
            {
                Registros.Log.AgregarRegistro(user, "COP", "Error al obtener el tipo de cambio de Colombia.");
                Console.WriteLine("Error al obtener el tipo de cambio de Colombia.");
                return null;
            }

            /* Se crea una instancia de la aplicacion SOAP obtenida.
             * En Servicios Conectados, la URL ingresada es: https://www.superfinanciera.gov.co/SuperfinancieraWebServiceTRM/TCRMServicesWebService/TCRMServicesWebService?WSDL
             * En App.config, la URL ingresada como endpoint address es: http://www.superfinanciera.gov.co/SuperfinancieraWebServiceTRM/TCRMServicesWebService/TCRMServicesWebService
             */
            clientTCRM = new TCRMServicesInterfaceClient();

            // Se obtiene el tipo de cambio llamando a la aplicacion SOAP.
            try
            {
                tipoCambio = clientTCRM.queryTCRM(objetoFecha).value.ToString();
            }
            catch (Exception ex)
            {
                Registros.Log.AgregarRegistro(user, "COP", "Error al obtener el tipo de cambio de Colombia: " + ex);
                Console.WriteLine("Error al obtener el tipo de cambio de Colombia: " + ex);
                return null;
            }

            // Finalmente se crea y regresa la lista de valores que se subiran a la BD.
            Registros.Log.AgregarRegistro(user, "COP", "Se obtuvo el tipo de cambio de Colombia correctamente.");
            Console.WriteLine("Se obtuvo el tipo de cambio de Colombia correctamente.");
            return CrearListaBD("0", tipoCambio, "COP");
        }
    }
}
