using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TipoCambio.Classes;
using System.Xml.Linq;

namespace TipoCambio.BusinessRules
{
    // La clase MonedaEuro hereda a la clase RequestsDivisas.
    class MonedaEuro : RequestsDivisas
    {
        /* Atributos de la clase. */
        // Para obtener el HTML tabla, los parametros tienen el siguiente formato: "yyyy-MM-dd"
        private readonly IList<string> url_xml = null;

        // Constructor de la clase.
        public MonedaEuro()
        {
            // Se inicializa cada una de las listas de URL y parametros a usar.
            url_xml = new List<string>
            {
                "https",
                "www.ecb.europa.eu/stats/policy_and_exchange_rates/euro_reference_exchange_rates/html/usd.xml",
                "GET"
            };

        }

        /* Metodo que permite crear la lista de valores que se subiran a la BD. (Para los metodos XML_Hoy y HTML_Fecha).
         * Notar el uso de la funcion CrearListaIndividual de la clase RequestsDivisas.
         */
        private IList<string> CrearListaXML(DateTime date)
        {
            // Declaracion e inicializacion de variables.
            IList<string> salida = null;
            string tipoCambio = "0";
            string fecha = date.ToString("yyyy-MM-dd");

            // Se verifica el tipo de cambio obtenido. Si es valido, se almacena.

            try
            {
                string url = "https://www.ecb.europa.eu/stats/policy_and_exchange_rates/euro_reference_exchange_rates/html/usd.xml";
                XElement xml = XElement.Load(url), targetObs;

                try
                {
                    XElement DataSet = xml.Descendants().Single((element) => element.Name.LocalName == "DataSet");
                    XElement Series = DataSet.Descendants().Single((element) => element.Name.LocalName == "Series");

                    targetObs = Series.Descendants().Single((element) => element.Attribute("TIME_PERIOD").Value == fecha);
                    Console.WriteLine("Deserialización Estructura de XML.");
                }

                catch
                {
                    Console.WriteLine("Estructura inesperada de XML.");
                    return null;
                }
                tipoCambio = targetObs.Attribute("OBS_VALUE").Value.ToString();
                Console.WriteLine("Obtención Estructura XML");
            }

            catch
            {
                Console.WriteLine("Error");
            }
            // Se crea y regresa la lista de valores que se subiran a la BD. 
            salida = CrearListaIndividual("0", tipoCambio, "EUR");

            return salida;
        }

        // Metodo ObtenerHoy se sobreescribe con XML_Hoy.
        public override IList<string> ObtenerHoy() => XML_Hoy();

        /* Metodo que permite obtener el tipo de cambio de hoy.
         * Regresa la lista de valores que se subiran a la BD.
         */
        public IList<string> XML_Hoy()
        {
            // Declaracion e inicializacion de variables.
            IList<string> salida = null;

            // El atributo objetoFecha almacena la fecha de hoy.
            objetoFecha = DateTime.Today;
            // Se valida si la fecha es fin de semana
            if (ValidarFechaFinSemana(objetoFecha.ToString("dd/MM/yyyy")) != 0)
            {
                salida = CrearListaIndividual("0", "0", "EUR");
                return salida;
            }

            // Se genera la lista de valores.


            // Finalmente se crea y regresa la lista de valores que se subiran a la BD.
            salida = CrearListaXML(objetoFecha);

            Console.WriteLine("La ejecución de la función se completó de forma correcta.");
            return salida;
        }

        // Metodo ObtenerFecha se sobreescribe con HTML_Fecha.
        public override IList<string> ObtenerFecha(string fecha) => HTML_Fecha(fecha);

        /* Metodo que permite obtener el tipo de cambio de una fecha especifica.
         * Regresa la lista de valores que se subiran a la BD.
         */
        public IList<string> HTML_Fecha(string fecha)
        {
            // Declaracion e inicializacion de variables.
            IList<string> salida = null;

            // Se valida la fecha ingresada.
            if (ValidarFecha(fecha) != 0)
            {
                Console.WriteLine("Error al ejecutar la función. La ejecución no se completó de forma correcta.");
                return null;
            }

            // Se valida si la fecha es fin de semana
            if (ValidarFechaFinSemana(objetoFecha.ToString("dd/MM/yyyy")) != 0)
            {
                salida = CrearListaIndividual("0", "0", "EUR");
                return salida;
            }

            // Se genera la lista de valores.


            // Finalmente se crea y regresa la lista de valores que se subiran a la BD.
            salida = CrearListaXML(objetoFecha);

            Console.WriteLine("La ejecución de la función se completó de forma correcta.");
            return salida;
        }
    }
}