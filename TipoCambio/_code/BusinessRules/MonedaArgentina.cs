using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TipoCambio.Classes;

namespace TipoCambio.BusinessRules
{
    // La clase MonedaArgentina hereda a la clase RequestsDivisas.
    class MonedaArgentina : RequestsDivisas
    {
        /* Atributos de la clase. */
        // Para obtener el HTML tabla, los parametros tienen el siguiente formato: "date2=10/10/2018&pp1=2"
        private readonly IList<string> url_html = null;
        private readonly IList<string> parameters_html = null;

        // Constructor de la clase.
        public MonedaArgentina()
        {
            // Se inicializa cada una de las listas de URL y parametros a usar.
            url_html = new List<string>
            {
                "http",
                "www.bcra.gob.ar/PublicacionesEstadisticas/Cotizaciones_por_fecha_2.asp?",
                "GET"
            };

            parameters_html = new List<string>
            {
                "date2",
                "pp1"
            };
        }

        /* Metodo que permite crear la lista de valores que se subiran a la BD. (Para los metodos HTML_Hoy y HTML_Fecha).
         * Notar el uso de la funcion CrearListaIndividual de la clase RequestsDivisas.
         */
        private IList<string> CrearListaHTML()
        {
            // Declaracion e inicializacion de variables.
            IList<string> salida = null;
            string tipoCambio = "0";

            // Se verifica el tipo de cambio obtenido. Si es valido, se almacena.
            if (objetoRequestHTML != null)
            {
                // En este caso como es una lista, tenemos que recorrer y verificar hasta encontrar el valor que deseamos
                int bandera = 0;
                foreach (string dato in objetoRequestHTML)
                {
                    if (dato.Trim() == "Dolar Estadounidense")
                    {
                        tipoCambio = objetoRequestHTML[bandera + 2].Trim();

                    }
                    bandera = bandera + 1;
                }

            }

            // Se crea y regresa la lista de valores que se subiran a la BD. 
            // Como argentina manda su dato con "," comas, tenemos que convertir esa coma a punto
            salida = CrearListaIndividual("0", tipoCambio.Replace(",", "."), "ARG");

            return salida;
        }

        // Metodo ObtenerHoy se sobreescribe con HTML_Hoy.
        public override IList<string> ObtenerHoy() => HTML_Hoy();

        /* Metodo que permite obtener el tipo de cambio de hoy.
         * Regresa la lista de valores que se subiran a la BD.
         */
        public IList<string> HTML_Hoy()
        {
            // Declaracion e inicializacion de variables.
            IList<string> salida = null;

            // El atributo objetoFecha almacena la fecha de hoy.
            objetoFecha = DateTime.Today;

            // Se genera la lista de valores.
            IList<string> values = new List<String>
            {
                objetoFecha.ToString("dd/MM/yyyy"),
                "2"
            };

            // Se ejecuta el metodo WebRequestJSON que hace todo el trabajo, verificando su resultado.
            if (WebRequestHTML(url_html, parameters_html, values) != 0)
            {
                Console.WriteLine("Error al ejecutar la función. La ejecución no se completó de forma correcta.");
                return null;
            }

            // Finalmente se crea y regresa la lista de valores que se subiran a la BD.
            salida = CrearListaHTML();

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

            // Se genera la lista de valores.
            IList<string> values = new List<String>
            {
                objetoFecha.ToString("dd/MM/yyyy"),
                "2"
            };


            // Se ejecuta el metodo WebRequestJSON que hace todo el trabajo, verificando su resultado.
            if (WebRequestHTML(url_html, parameters_html, values) != 0)
            {
                Console.WriteLine("Error al ejecutar la función. La ejecución no se completó de forma correcta.");
                return null;
            }

            // Finalmente se crea y regresa la lista de valores que se subiran a la BD.
            salida = CrearListaHTML();

            Console.WriteLine("La ejecución de la función se completó de forma correcta.");
            return salida;
        }
    }
}