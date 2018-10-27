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
        // Para obtener el excel tabla, no hay parametros en este caso
        private readonly IList<string> url_excel = null;
        private readonly IList<string> parameters_excel = null;

        // Constructor de la clase.
        public MonedaRepublicaDominicana()
        {
            // Se inicializa cada una de las listas de URL y parametros a usar.
            url_excel = new List<string>
            {
                "https",
                "gdc.bancentral.gov.do/Common/public/estadisticas/mercado-cambiario//documents//DOLAR_VENTANILLA_SONDEO.xls",
                "GET"
            };

        }

        /* Metodo que permite crear la lista de valores que se subiran a la BD. (Para los metodos Excel_Hoy y Excel_Fecha).
         * Notar el uso de la funcion CrearListaIndividual de la clase RequestsDivisas.
         */
        private IList<string> CrearListaExcel(string fecha)
        {
            // Declaracion e inicializacion de variables.
            IList<string> salida = null;
            string tipoCambioCompra = "0";
            string tipoCambioVenta = "0";
            // Se verifica el tipo de cambio obtenido. Si es valido, se almacena.

            if (objetoRequestExcel != null)
            {
                //Primero obtenemos los parametros necesarios para obtener el valor del excel
                IList<string> formafecha = CrearFormatoExcel(fecha);
                // En este caso como es una lista, tenemos que recorrer y verificar hasta encontrar el valor que deseamos
                foreach (var data in objetoRequestExcel)
                {
                    if (data[0] == formafecha[0] && data[1] == formafecha[1] && data[2] == formafecha[2])
                    {
                        tipoCambioCompra = data["F4"].ToString();
                        tipoCambioVenta = data["F5"].ToString();
                    }

                }

            }
            if (tipoCambioCompra == "0" && tipoCambioVenta == "0")
            {
                Console.WriteLine("Error aun no se ha actualizado el excel del banco de RD, se actualiza antes de la 1:30 pm");
            }

            // Se crea y regresa la lista de valores que se subiran a la BD. 
            salida = CrearListaIndividual(tipoCambioCompra, tipoCambioVenta, "‎RD");

            return salida;
        }
        //Le damos formato para poder obtener nuestros valores de excel
        private IList<string> CrearFormatoExcel(string fecha)
        {
            string year_rd = "";
            string mes_rd = "";
            string dia_rd = "";
            DateTime objetoFecha2;
            if (DateTime.TryParse(fecha, out objetoFecha2))
            {
                year_rd = objetoFecha2.Year.ToString();
                switch (objetoFecha2.Month.ToString())
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
                    case "12":
                        mes_rd = "Dic";
                        break;
                    default:
                        Console.WriteLine("Error al tranformar formato mes para excel de Rep Dominicana");
                        break;
                }
                dia_rd = objetoFecha2.Day.ToString();

            }
            else
            {
                Console.WriteLine("Se ha ingresado una fecha incorrecta.");
            }
            IList<string> salida = new List<string>()
            {
                year_rd,
                mes_rd,
                dia_rd
            };

            return salida;
        }

        // Metodo ObtenerHoy se sobreescribe con Excel_Hoy.
        public override IList<string> ObtenerHoy() => Excel_Hoy();

        /* Metodo que permite obtener el tipo de cambio de hoy.
         * Regresa la lista de valores que se subiran a la BD.
         */
        public IList<string> Excel_Hoy()
        {
            // Declaracion e inicializacion de variables.
            IList<string> salida = null;

            // El atributo objetoFecha almacena la fecha de hoy.
            objetoFecha = DateTime.Today;

            // Se valida si la fecha es fin de semana
            if (ValidarFechaFinSemana(objetoFecha.ToString("dd/MM/yyyy")) != 0)
            {
                salida = CrearListaIndividual("0", "0", "‎RD");
                return salida;
            }

            // Se genera la lista de valores.
            IList<string> values = null;

            // Se ejecuta el metodo WebRequestExcel que hace todo el trabajo, verificando su resultado.
            if (WebRequestExcel(url_excel, parameters_excel, values, "datos_republica_dominicana.xls", "Tasa Sondeo, Ventanilla, Dólar") != 0)
            {
                Console.WriteLine("Error al ejecutar la función. La ejecución no se completó de forma correcta.");
                return null;
            }


            // Finalmente se crea y regresa la lista de valores que se subiran a la BD.
            salida = CrearListaExcel(objetoFecha.ToString("dd/MM/yyyy"));

            if (EliminarArchivo("datos_republica_dominicana.xls") != 0)
            {
                Console.WriteLine("Error al ejecutar la función de eliminar. La ejecución no se completó de forma correcta.");
            }

            Console.WriteLine("La ejecución de la función se completó de forma correcta.");
            return salida;
        }

        // Metodo ObtenerFecha se sobreescribe con Excel_Fecha.
        public override IList<string> ObtenerFecha(string fecha) => Excel_Fecha(fecha);

        /* Metodo que permite obtener el tipo de cambio de una fecha especifica.
         * Regresa la lista de valores que se subiran a la BD.
         */
        public IList<string> Excel_Fecha(string fecha)
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
            if (ValidarFechaFinSemana(fecha) != 0)
            {
                salida = CrearListaIndividual("0", "0", "‎RD");
                return salida;
            }

            // Se genera la lista de valores.
            IList<string> values = null;


            // Se ejecuta el metodo WebRequestExcel que hace todo el trabajo, verificando su resultado.
            if (WebRequestExcel(url_excel, parameters_excel, values, "datos_republica_dominicana.xls", "Tasa Sondeo, Ventanilla, Dólar") != 0)
            {
                Console.WriteLine("Error al ejecutar la función. La ejecución no se completó de forma correcta.");
                return null;
            }

            // Finalmente se crea y regresa la lista de valores que se subiran a la BD.
            salida = CrearListaExcel(objetoFecha.ToString("dd/MM/yyyy"));

            if (EliminarArchivo("datos_republica_dominicana.xls") != 0)
            {
                Console.WriteLine("Error al ejecutar la función de eliminar. La ejecución no se completó de forma correcta.");
            }


            Console.WriteLine("La ejecución de la función se completó de forma correcta.");
            return salida;
        }
    }
}