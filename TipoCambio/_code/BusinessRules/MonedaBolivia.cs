using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TipoCambio.Classes;

namespace TipoCambio.BusinessRules
{
    // La clase MonedaBolivia hereda a la clase RequestsDivisas.
    class MonedaBolivia : RequestsDivisas
    {
        /* Atributos de la clase. */
        // Para obtener el HTML tabla, los parametros tienen el siguiente formato: "qdd=29&qmm=9&qaa=2018"
        private readonly IList<string> datos_url = null;
        private readonly IList<string> parameters = null;

        // Constructor de la clase.
        public MonedaBolivia()
        {
            // Se inicializa cada una de las listas de URL y parametros a usar.
            datos_url = new List<string>
            {
                "https",
                "www.bcb.gob.bo/librerias/indicadores/otras/anteriores.php?",
                "GET",
                "HTML"
            };

            parameters = new List<string>
            {
                "qdd",
                "qmm",
                "qaa"
            };
        }

        // Metodo ObtenerFecha (Default) se sobreescribe con HTML_Hoy.
        public override IList<string> ObtenerFecha() => HTML_Hoy();

        /* Metodo que permite obtener el tipo de cambio de hoy.
         * Regresa la lista de valores que se subiran a la BD.
         */
        public IList<string> HTML_Hoy()
        {
            // El atributo objetoFecha almacena la fecha de hoy.
            objetoFecha = DateTime.Today;

            // Si el dia pertenece al fin de semana, se regresa una lista con tipo de cambio en 0.
            if (VerificarFecha() != 0)
            {
                Console.WriteLine("Se obtuvo el tipo de cambio de Bolivia correctamente.");
                return CrearListaBD("0", "0", "BOB");
            }

            // Se realiza la peticion web y se regresa la lista con el tipo de cambio.
            return EstandarWebRequest();
        }

        // Metodo ObtenerFecha se sobreescribe con HTML_Fecha.
        public override IList<string> ObtenerFecha(string fecha) => HTML_Fecha(fecha);

        /* Metodo que permite obtener el tipo de cambio de una fecha especifica.
         * Regresa la lista de valores que se subiran a la BD.
         */
        public IList<string> HTML_Fecha(string fecha)
        {
            // Declaracion e inicializacion de variables.
            int resultadoFecha = VerificarFecha(fecha);

            // Se verifica la fecha ingresada.
            if (resultadoFecha <= 0)
            {
                // Caso de dia en fin de semana.
                if (resultadoFecha == -1)
                {
                    Console.WriteLine("Se obtuvo el tipo de cambio de Bolivia correctamente.");
                    return CrearListaBD("0", "0", "BOB");
                }
            }

            else
            {
                Console.WriteLine("Error al obtener el tipo de cambio de Bolivia.");
                return null;
            }

            // Se realiza la peticion web y se regresa la lista con el tipo de cambio.
            return EstandarWebRequest();
        }

        /* Metodo que realizar toda la parte estandar del Web Request (Para los metodos HTML_Hoy y HTML_Fecha).
         * Regresa la lista de valores lista para ser subida la Base de Datos.
         */
        private IList<string> EstandarWebRequest()
        {
            // Declaracion e incializacion de variables.
            int ejecucionWebRequest = 0;
            // Se genera la lista de valores.
            IList<string> values = new List<String>
            {
                Convert.ToString(objetoFecha.Day),
                Convert.ToString(objetoFecha.Month),
                Convert.ToString(objetoFecha.Year)
            };
            // Se ejecuta el metodo WebRequestHTML que hace todo el trabajo, verificando su resultado.
            ejecucionWebRequest = WebRequestHTML(datos_url, parameters, values);

            // Error en el Web Request.
            if (ejecucionWebRequest > 0)
            {
                Console.WriteLine("Error al obtener el tipo de cambio de Bolivia.");
                return null;
            }

            // Ejecucion correcta del Web Request
            else
            {
                /* Se comprueba el caso de nodos vacios, donde la fecha no tiene tipo de cambio y se
                 * regresa una lista con tipo de cambio 0.
                 */
                if (ejecucionWebRequest == -1)
                {
                    Console.WriteLine("Se obtuvo el tipo de cambio de Bolivia correctamente.");
                    return CrearListaBD("0", "0", "BOB");
                }

                // Finalmente se crea y regresa la lista de valores que se subiran a la BD.
                return CrearLista();
            }
        }

        /* Metodo que permite crear la lista de valores que se subiran a la BD. (Para los metodos HTML_Hoy y HTML_Fecha).
         * Notar el uso de la funcion CrearBD de la clase RequestsDivisas.
         */
        private IList<string> CrearLista()
        {
            // Declaracion e inicializacion de variables.
            string tipoCambio = "0";
            string tipoCambioCompra = "0";
            int bandera = 0;
            int bandera2 = 0;

            // Se verifica el tipo de cambio obtenido. Si es valido, se almacena.
            if (objetoRequest != null)
            {
                // Como es una lista, se tiene que recorrer y verificar hasta encontrar el valor deseado.
                foreach (string dato in objetoRequest)
                {
                    if (dato.Trim() == "ESTADOS UNIDOS" && bandera2 == 0)
                    {
                        tipoCambio = objetoRequest[bandera + 3].Trim();
                        tipoCambioCompra = objetoRequest[bandera + 8].Trim();
                        bandera2 = 1;
                    }

                    bandera++;
                }
            }

            // Se crea y regresa la lista de valores que se subiran a la BD.
            Console.WriteLine("Se obtuvo el tipo de cambio de Bolivia correctamente.");
            return CrearListaBD(tipoCambioCompra, tipoCambio, "BOB");
        }
    }
}