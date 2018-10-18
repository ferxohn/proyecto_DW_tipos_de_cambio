using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TipoCambio.Classes;

namespace TipoCambio.BusinessRules
{
    // La clase MonedaMexico hereda a la clase RequestsDivisas.
    class MonedaBrasil : RequestsDivisas
    {
        /* Atributos de la clase. */
        // Para obtener el JSON, los parametros tienen el siguiente formato: "DATAINI=09/10/2018&DATAFIM=10/10/2018"
        private readonly IList<string> url_csv = null;
        private readonly IList<string> parameters_csv = null;

        // Constructor de la clase.
        public MonedaBrasil()
        {
            // Se inicializa cada una de las listas de URL y parametros a usar.
            url_csv = new List<string>
            {
                "https",
                "ptax.bcb.gov.br/ptax_internet/consultaBoletim.do?method=gerarCSVFechamentoMoedaNoPeriodo&ChkMoeda=61&",
                "GET"
            };

            parameters_csv = new List<string>
            {
                "DATAINI",
                "DATAFIM",
            };
        }

        // Metodo ObtenerHoy se sobreescribe con JSON_Hoy.
        public override IList<string> ObtenerHoy() => CSV_Hoy();

        /* Metodo que permite obtener el tipo de cambio de hoy.
         * Regresa la lista de valores que se subiran a la BD.
         */
        private IList<string> CSV_Hoy()
        {
            // Declaracion e inicializacion de variables.
            IList<string> salida = null;

            // El atributo objetoFecha almacena la fecha de hoy.
            objetoFecha = DateTime.Today;

            // Se genera la lista de valores.
            IList<string> values = new List<String>
            {
                objetoFecha.ToString("dd/MM/yyyy"),
                objetoFecha.ToString("dd/MM/yyyy"),
            };

            // Se ejecuta el metodo WebRequestJSON que hace todo el trabajo, verificando su resultado.
            if (WebRequestCSV(url_csv, parameters_csv, values) != 0)
            {
                Console.WriteLine("Error al ejecutar la funci�n. La ejecuci�n no se complet� de forma correcta.");
                return null;
            }

            Console.WriteLine("La ejecuci�n de la funci�n se complet� de forma correcta.");
            return salida;
        }

        // Metodo ObtenerHoy se sobreescribe con JSON_Hoy.
        public override IList<string> ObtenerFecha(string fecha) => CSV_Fecha(fecha);

        /* Metodo que permite obtener el tipo de cambio de una fecha especifica.
         * Regresa la lista de valores que se subiran a la BD.
         */
        private IList<string> CSV_Fecha(string fecha)
        {
            // Declaracion e inicializacion de variables.
            IList<string> salida = null;

            // Se valida la fecha ingresada.
            if (ValidarFecha(fecha) != 0)
            {
                Console.WriteLine("Error al ejecutar la funci�n. La ejecuci�n no se complet� de forma correcta.");
                return null;
            }

            // Se genera la lista de valores.
            IList<string> values = new List<String>
            {
                "09/10/2018",
                "10/10/2018",
            };

            // Se ejecuta el metodo WebRequestJSON que hace todo el trabajo, verificando su resultado.
            if (WebRequestCSV(url_csv, parameters_csv, values) != 0)
            {
                Console.WriteLine("Error al ejecutar la funci�n. La ejecuci�n no se complet� de forma correcta.");
                return null;
            }

            Console.WriteLine("La ejecuci�n de la funci�n se complet� de forma correcta.");
            return salida;
        }
    }
}