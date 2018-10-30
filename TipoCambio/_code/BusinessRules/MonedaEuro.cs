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
        private readonly IList<string> datos_url = null;

        // Constructor de la clase.
        public MonedaEuro(string usuario): base(usuario)
        {
            // Se inicializa cada una de las listas de URL y parametros a usar.
            datos_url = new List<string>
            {
                "https",
                "www.ecb.europa.eu/stats/policy_and_exchange_rates/euro_reference_exchange_rates/html/usd.xml",
                "GET",
                "XML"
            };
        }

        // Metodo ObtenerFecha (Default) se sobreescribe con XML_Hoy.
        public override IList<string> ObtenerFecha() => XML_Hoy();

        /* Metodo que permite obtener el tipo de cambio de hoy.
         * Regresa la lista de valores que se subiran a la BD.
         */
        public IList<string> XML_Hoy()
        {
            // El atributo objetoFecha almacena la fecha de hoy.
            objetoFecha = DateTime.Today;

            // Si el dia pertenece al fin de semana, se regresa una lista con tipo de cambio en 0.
            if (VerificarFecha() != 0)
            {
                Registros.Log.AgregarRegistro(user, "EUR", "Se obtuvo el tipo de cambio de la Unión Europea correctamente.");
                Console.WriteLine("Se obtuvo el tipo de cambio de la Unión Europea correctamente.");
                return CrearListaBD("0", "0", "EUR");
            }

            // Se realiza la peticion web y se regresa la lista con el tipo de cambio.
            return EstandarWebRequest();
        }

        // Metodo ObtenerFecha se sobreescribe con XML_Fecha.
        public override IList<string> ObtenerFecha(string fecha) => XML_Fecha(fecha);

        /* Metodo que permite obtener el tipo de cambio de una fecha especifica.
         * Regresa la lista de valores que se subiran a la BD.
         */
        public IList<string> XML_Fecha(string fecha)
        {
            // Declaracion e inicializacion de variables.
            int resultadoFecha = VerificarFecha(fecha);

            // Se verifica la fecha ingresada.
            if (resultadoFecha <= 0)
            {
                // Caso de dia en fin de semana.
                if (resultadoFecha == -1)
                {
                    Registros.Log.AgregarRegistro(user, "EUR", "Se obtuvo el tipo de cambio de la Unión Europea correctamente.");
                    Console.WriteLine("Se obtuvo el tipo de cambio de la Unión Europea correctamente.");
                    return CrearListaBD("0", "0", "EUR");
                }
            }

            else
            {
                Registros.Log.AgregarRegistro(user, "EUR", "Error al obtener el tipo de cambio de la Unión Europea.");
                Console.WriteLine("Error al obtener el tipo de cambio de la Unión Europea.");
                return null;
            }

            // Se realiza la peticion web y se regresa la lista con el tipo de cambio.
            return EstandarWebRequest();
        }

        /* Metodo que permite crear la lista de valores que se subiran a la BD. (Para los metodos XML_Hoy y HTML_Fecha).
         * Notar el uso de la funcion CrearBD de la clase RequestsDivisas.
         */
        private IList<string> EstandarWebRequest()
        {
            // Declaracion e inicializacion de variables.
            string tipoCambio = null;
            XElement xmlObtenido = null;
            XElement dataSet = null;
            XElement series = null;

            // Se ejecuta y verifica el Web Request.
            if (RequestWeb(datos_url) != 0)
            {
                Registros.Log.AgregarRegistro(user, "EUR", "Error al obtener el tipo de cambio de la Unión Europea.");
                Console.WriteLine("Error al obtener el tipo de cambio de la Unión Europea.");
                return null;
            }

            // Se verifica la estructura del XML obtenido.
            try
            {
                // Se almacena el objetoRequest en otra variable para su tratamiento.
                xmlObtenido = objetoRequest;
                // Se deserializa el XML en su etiqueta DataSet.
                dataSet = xmlObtenido.Descendants().Single((element) => element.Name.LocalName == "DataSet");
                // Se deserializa el XML en su etiqueta Series, que ya contiene todos los tipos de cambio.
                series = dataSet.Descendants().Single((element) => element.Name.LocalName == "Series");
                
                // Finalmente se deserializa el XML en su valor TIME_PERIOD.
                try
                {
                    // Si se deserializa correctamente, objetoRequest almacenara un valor.
                    objetoRequest = series.Descendants().Single((element) => element.Attribute("TIME_PERIOD").Value == objetoFecha.ToString("yyyy-MM-dd"));
                    Registros.Log.AgregarRegistro(user, "EUR", "Se deserializó el XML correctamente.");
                    Console.WriteLine("Se deserializó el XML correctamente.");

                    // Finalmente se obtiene el tipo de cambio.
                    tipoCambio = objetoRequest.Attribute("OBS_VALUE").Value.ToString();

                    // Se crea y regresa la lista de valores que se subiran a la BD.
                    Registros.Log.AgregarRegistro(user, "EUR", "Se obtuvo el tipo de cambio de la Unión Europea correctamente.");
                    Console.WriteLine("Se obtuvo el tipo de cambio de la Unión Europea correctamente.");
                    return CrearListaBD("0", tipoCambio, "EUR");
                }
                catch (Exception)
                {
                    // Si se genera una excepcion, entonces la fecha no tiene tipo de cambio, y se regresa una lista con tipo de cambio 0.
                    Registros.Log.AgregarRegistro(user, "EUR", "Se obtuvo el tipo de cambio de la Unión Europea correctamente.");
                    Console.WriteLine("Se obtuvo el tipo de cambio de la Unión Europea correctamente.");
                    return CrearListaBD("0", "0", "EUR");
                }
            }
            catch (Exception ex)
            {
                Registros.Log.AgregarRegistro(user, "EUR", "Error al deserializar el XML: " + ex.Message);
                Console.WriteLine("Error al deserializar el XML: " + ex.Message);
                Registros.Log.AgregarRegistro(user, "EUR", "Error al obtener el tipo de cambio de la Unión Europea.");
                Console.WriteLine("Error al obtener el tipo de cambio de la Unión Europea.");
                return null;
            }
        }
    }
}