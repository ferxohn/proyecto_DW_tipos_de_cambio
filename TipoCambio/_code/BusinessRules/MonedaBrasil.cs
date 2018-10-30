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
        private readonly IList<string> datos_url = null;
        private readonly IList<string> parameters = null;

        // Constructor de la clase.
        public MonedaBrasil(string usuario): base(usuario)
        {
            // Se inicializa cada una de las listas de URL y parametros a usar.
            datos_url = new List<string>
            {
                "https",
                "ptax.bcb.gov.br/ptax_internet/consultaBoletim.do?method=gerarCSVFechamentoMoedaNoPeriodo&ChkMoeda=61&",
                "GET",
                "CSV"
            };

            parameters = new List<string>
            {
                "DATAINI",
                "DATAFIM",
            };
        }

        // Metodo ObtenerFecha (Default) se sobreescribe con CSV_Hoy.
        public override IList<string> ObtenerFecha() => CSV_Hoy();

        /* Metodo que permite obtener el tipo de cambio de hoy.
         * Regresa la lista de valores que se subiran a la BD.
         */
        private IList<string> CSV_Hoy()
        {
            // El atributo objetoFecha almacena la fecha de hoy.
            objetoFecha = DateTime.Today;

            // Si el dia pertenece al fin de semana, se regresa una lista con tipo de cambio en 0.
            if (VerificarFecha() != 0)
            {
                Registros.Log.AgregarRegistro(user, "BRL", "Se obtuvo el tipo de cambio de Brasil correctamente.");
                Console.WriteLine("Se obtuvo el tipo de cambio de Brasil correctamente.");
                return CrearListaBD("0", "0", "BRL");
            }

            // Se realiza la peticion web y se regresa la lista con el tipo de cambio.
            return EstandarWebRequest();
        }

        // Metodo ObtenerFecha se sobreescribe con CSV_Fecha.
        public override IList<string> ObtenerFecha(string fecha) => CSV_Fecha(fecha);

        /* Metodo que permite obtener el tipo de cambio de una fecha especifica.
         * Regresa la lista de valores que se subiran a la BD.
         */
        private IList<string> CSV_Fecha(string fecha)
        {
            // Declaracion e inicializacion de variables.
            int resultadoFecha = VerificarFecha(fecha);

            // Se verifica la fecha ingresada.
            if (resultadoFecha <= 0)
            {
                // Caso de dia en fin de semana.
                if (resultadoFecha == -1)
                {
                    Registros.Log.AgregarRegistro(user, "BRL", "Se obtuvo el tipo de cambio de Brasil correctamente.");
                    Console.WriteLine("Se obtuvo el tipo de cambio de Brasil correctamente.");
                    return CrearListaBD("0", "0", "BRL");
                }
            }

            else
            {
                Registros.Log.AgregarRegistro(user, "BRL", "Error al obtener el tipo de cambio de Brasil.");
                Console.WriteLine("Error al obtener el tipo de cambio de Brasil.");
                return null;
            }

            // Se realiza la peticion web y se regresa la lista con el tipo de cambio.
            return EstandarWebRequest();
        }

        /* Metodo que realizar toda la parte estandar del Web Request (Para los metodos CSV_Hoy y CSV_Fecha).
         * Regresa la lista de valores lista para ser subida la Base de Datos.
         */
        private IList<string> EstandarWebRequest()
        {
            /* Se genera la lista de valores.
             * Notar que la segunda fecha tiene una dia mas a la fecha consultada.
             * Esto debido a que la pagina web de Brasil solo permite consultar un intervalo
             * de fechas.
             */
            IList<string> values = new List<String>
            {
                objetoFecha.ToString("dd/MM/yyyy"),
                objetoFecha.AddDays(1).ToString("dd/MM/yyyy")
            };

            // Se realiza y verifica la peticion web.
            if (RequestWeb(datos_url, parameters, values) != 0)
            {
                Registros.Log.AgregarRegistro(user, "BRL", "Error al obtener el tipo de cambio de Brasil.");
                Console.WriteLine("Error al obtener el tipo de cambio de Brasil.");
                return null;
            }

            /* Si la peticion regresa un HTML, entonces el dia no tiene tipo de cambio
             * y se regresa una lista con tipo de cambio 0.
             */
            if (objetoRequest.ContentType.ToString() == "text/html;charset=ISO-8859-1")
            {
                Registros.Log.AgregarRegistro(user, "BRL", "Se obtuvo el tipo de cambio de Brasil correctamente.");
                Console.WriteLine("Se obtuvo el tipo de cambio de Brasil correctamente.");
                return CrearListaBD("0", "0", "BRL");
            }

            // Se corrigen algunos caracteres del string obtenido para poder deserializarlo.
            respuestaRequest = respuestaRequest.Replace("\n", string.Empty);
            respuestaRequest = respuestaRequest.Replace(",", ".");
            respuestaRequest = respuestaRequest.Replace(";", ",");

            // Se deserializa el CSV y se comprueba el resultado.
            if (DeserializarCSV() != 0)
            {
                Registros.Log.AgregarRegistro(user, "BRL", "Error al obtener el tipo de cambio de Brasil.");
                Console.WriteLine("Error al obtener el tipo de cambio de Brasil.");
                return null;
            }

            // Se ejecuta Read para poder leer el CSV obtenido.
            objetoRequest.Read();

            /* Por la forma en la que funciona la pagina web de Brasil, si se obtiene
             * la fecha del dia siguiente a la fecha consultada en el primer espacio,
             * entonces el dia consultado no tiene tipo de cambio, y se regresa una
             * lista con tipo de cambio 0.
             */
            if (objetoFecha.ToString("ddMMyyyy") != objetoRequest[0])
            {
                Registros.Log.AgregarRegistro(user, "BRL", "Se obtuvo el tipo de cambio de Brasil correctamente.");
                Console.WriteLine("Se obtuvo el tipo de cambio de Brasil correctamente.");
                return CrearListaBD("0", "0", "BRL");
            }

            // Si no es asi, entonces el dia tiene tipo de cambio, y se regresa la lista con esos valores.
            else
            {
                Registros.Log.AgregarRegistro(user, "BRL", "Se obtuvo el tipo de cambio de Brasil correctamente.");
                Console.WriteLine("Se obtuvo el tipo de cambio de Brasil correctamente.");
                return CrearListaBD(objetoRequest[4], objetoRequest[5], "BRL");
            }
        } 
    }
}