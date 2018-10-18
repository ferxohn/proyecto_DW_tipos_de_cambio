using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TipoCambio.Classes;

namespace TipoCambio.BusinessRules
{
    // La clase MonedaMexico hereda a la clase RequestsDivisas.
    class MonedaCanada : RequestsDivisas
    {
        /* Atributos de la clase. */
        // Para obtener el JSON, los parametros tienen el siguiente formato: "start_date=2018-10-10&end_date=2018-10-10"
        private readonly IList<string> url_json = null;
        private readonly IList<string> parameters_json = null;

        // Constructor de la clase.
        public MonedaCanada()
        {
            // Se inicializa cada una de las listas de URL y parametros a usar.
            url_json = new List<string>
            {
                "https",
                "www.bankofcanada.ca/valet/observations/FXUSDCAD?",
                "GET"
            };

            parameters_json = new List<string>
            {
                "start_date",
                "end_date"
            };
        }

        /* Metodo que permite crear la lista de valores que se subiran a la BD. (Para los metodos JSON_Hoy y JSON_Fecha).
         * Notar el uso de la funcion CrearListaIndividual de la clase RequestsDivisas.
         */
        private IList<string> CrearListaJSON()
        {
            // Declaracion e inicializacion de variables.
            IList<string> salida = null;
            string tipoCambio = "0";

            // Se verifica el tipo de cambio obtenido. Si es valido, se almacena.
            if (objetoRequest["observations"].ToString() != "[]")
            {
                tipoCambio = objetoRequest["observations"][0]["FXUSDCAD"]["v"].ToString();
            }

            // Se crea y regresa la lista de valores que se subiran a la BD.
            salida = CrearListaIndividual("0", tipoCambio, "CAN");

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
                objetoFecha.ToString("yyyy-MM-dd"),
                objetoFecha.ToString("yyyy-MM-dd")
            };

            // Se ejecuta el metodo WebRequestJSON que hace todo el trabajo, verificando su resultado.
            if (WebRequestJSON(url_json, parameters_json, values) != 0)
            {
                Console.WriteLine("Error al ejecutar la función. La ejecución no se completó de forma correcta.");
                return null;
            }

            // Finalmente se crea y regresa la lista de valores que se subiran a la BD.
            salida = CrearListaJSON();

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

            // Se genera la lista de valores.
            IList<string> values = new List<String>
            {
                objetoFecha.ToString("yyyy-MM-dd"),
                objetoFecha.ToString("yyyy-MM-dd")
            };


            // Se ejecuta el metodo WebRequestJSON que hace todo el trabajo, verificando su resultado.
            if (WebRequestJSON(url_json, parameters_json, values) != 0)
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