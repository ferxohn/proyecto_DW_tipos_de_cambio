using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TipoCambio.Classes;


namespace TipoCambio.BusinessRules
{
    // La clase MonedaBrasil hereda a la clase RequestsDivisas.
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

        private IList<string> CrearListaCSV(IList<string> url_csv, IList<string> parameters_csv, IList<string> values)
        {
            IList<string> salida = null;

            // Se ejecuta el metodo WebRequestJSON que hace todo el trabajo, verificando su resultado.
            if (RequestWeb(url_csv, parameters_csv, values) != 0)
            {
                Console.WriteLine("Error al ejecutar la función. La ejecución no se completó de forma correcta.");
                return null;
            }

            if (objetoResponse.ContentType.ToString() == "text/html;charset=ISO-8859-1")
            {
                salida = CrearListaIndividual("0", "0", "BRL");
                return salida;
            }

            respuestaRequest = respuestaRequest.Replace("\n", string.Empty);
            respuestaRequest = respuestaRequest.Replace(",", ".");
            respuestaRequest = respuestaRequest.Replace(";", ",");

            if (DeserializarCSV() != 0)
            {
                Console.WriteLine("Error al ejecutar la función. La ejecución no se completó de forma correcta.");
                return null;
            }

            objetoRequest.Read();

            if (objetoFecha.ToString("ddMMyyyy") != objetoRequest[0])
            {
                salida = CrearListaIndividual("0", "0", "BRL");
                return salida;
            }

            else
            {
                salida = CrearListaIndividual(objetoRequest[4], objetoRequest[5], "BRL");
                return salida;
            }
        }

        // Metodo ObtenerHoy se sobreescribe con CSV_Hoy.
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

            if (FinSemana() != 0)
            {
                salida = CrearListaIndividual("0", "0", "BRL");
                return salida;
            }

            // Se genera la lista de valores.
            IList<string> values = new List<String>
            {
                objetoFecha.ToString("dd/MM/yyyy"),
                objetoFecha.AddDays(1).ToString("dd/MM/yyyy")
            };

            salida = CrearListaCSV(url_csv, parameters_csv, values);

            Console.WriteLine("La ejecución de la función se completó de forma correcta.");
            return salida;
        }

        // Metodo ObtenerHoy se sobreescribe con CSV_Hoy.
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
                Console.WriteLine("Error al ejecutar la función. La ejecución no se completó de forma correcta.");
                return null;
            }

            if (FinSemana() != 0)
            {
                salida = CrearListaIndividual("0", "0", "BRL");
                return salida;
            }

            // Se genera la lista de valores.
            IList<string> values = new List<String>
            {
                objetoFecha.ToString("dd/MM/yyyy"),
                objetoFecha.AddDays(1).ToString("dd/MM/yyyy")
            };

            // Finalmente se crea y regresa la lista de valores que se subiran a la BD.
            salida = CrearListaCSV(url_csv, parameters_csv, values);

            Console.WriteLine("La ejecución de la función se completó de forma correcta.");
            return salida;
        }
    }

}