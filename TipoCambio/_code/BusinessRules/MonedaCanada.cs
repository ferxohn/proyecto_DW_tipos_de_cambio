using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TipoCambio.Classes;

namespace TipoCambio.BusinessRules
{
    // La clase MonedaCanada hereda a la clase RequestsDivisas.
    class MonedaCanada : RequestsDivisas
    {
        /* Atributos de la clase. */
        // Para obtener el JSON, los parametros tienen el siguiente formato: "start_date=2018-10-10&end_date=2018-10-10"
        private readonly IList<string> datos_url = null;
        private readonly IList<string> parameters = null;

        // Constructor de la clase.
        public MonedaCanada(string usuario): base(usuario)
        {
            // Se inicializa cada una de las listas de URL y parametros a usar.
            datos_url = new List<string>
            {
                "https",
                "www.bankofcanada.ca/valet/observations/FXUSDCAD?",
                "GET",
                "JSON"
            };

            parameters = new List<string>
            {
                "start_date",
                "end_date"
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
                Registros.Log.AgregarRegistro(user, "CAN", "Se obtuvo el tipo de cambio de Canadá correctamente.");
                Console.WriteLine("Se obtuvo el tipo de cambio de Canadá correctamente.");
                return CrearListaBD("0", "0", "CAN");
            }

            // Se realiza la peticion web y se regresa la lista con el tipo de cambio.
            return EstandarWebRequest();
        }

        // Metodo ObtenerFecha se sobreescribe con JSON_Fecha.
        public override IList<string> ObtenerFecha(string fecha) => JSON_Fecha(fecha);

        /* Metodo que permite obtener el tipo de cambio de una fecha especifica.
         * Regresa la lista de valores que se subiran a la BD.
         */
        private IList<string> JSON_Fecha(string fecha)
        {
            // Declaracion e inicializacion de variables.
            int resultadoFecha = VerificarFecha(fecha);

            // Se verifica la fecha ingresada.
            if (resultadoFecha <= 0)
            {
                // Caso de dia en fin de semana.
                if (resultadoFecha == -1)
                {
                    Registros.Log.AgregarRegistro(user, "CAN", "Se obtuvo el tipo de cambio de Canadá correctamente.");
                    Console.WriteLine("Se obtuvo el tipo de cambio de Canadá correctamente.");
                    return CrearListaBD("0", "0", "CAN");
                }
            }

            else
            {
                Registros.Log.AgregarRegistro(user, "CAN", "Se obtuvo el tipo de cambio de Canadá correctamente.");
                Console.WriteLine("Error al obtener el tipo de cambio de Canadá.");
                return null;
            }

            // Se realiza la peticion web y se regresa la lista con el tipo de cambio.
            return EstandarWebRequest();
        }

        /* Metodo que realizar toda la parte estandar del Web Request (Para los metodos JSON_Hoy y JSON_Fecha).
         * Regresa la lista de valores lista para ser subida la Base de Datos.
         */
        private IList<string> EstandarWebRequest()
        {
            // Se genera la lista de valores.
            IList<string> values = new List<String>
            {
                objetoFecha.ToString("yyyy-MM-dd"),
                objetoFecha.ToString("yyyy-MM-dd")
            };

            // Se ejecuta el metodo WebRequestJSON que hace todo el trabajo de peticion web, verificando su resultado.
            if (WebRequestJSON(datos_url, parameters, values) != 0)
            {
                Registros.Log.AgregarRegistro(user, "CAN", "Se obtuvo el tipo de cambio de Canadá correctamente.");
                Console.WriteLine("Error al obtener el tipo de cambio de Canadá.");
                return null;
            }

            // Finalmente se crea y regresa la lista de valores que se subiran a la BD.
            return CrearLista();
        }

        /* Metodo que permite crear la lista de valores que se subiran a la BD. (Para los metodos JSON_Hoy y JSON_Fecha).
         * Notar el uso de la funcion CrearListaBD de la clase RequestsDivisas.
         * Regresa la lista de valores lista para ser subida la Base de Datos.
         */
        private IList<string> CrearLista()
        {
            // Declaracion e inicializacion de variables.
            string tipoCambio = "0";

            // Se verifica el tipo de cambio obtenido. Si es valido, se almacena.
            if (objetoRequest["observations"].ToString() != "[]")
            {
                tipoCambio = objetoRequest["observations"][0]["FXUSDCAD"]["v"].ToString();
            }

            // Se crea y regresa la lista de valores que se subiran a la BD.
            Registros.Log.AgregarRegistro(user, "CAN", "Se obtuvo el tipo de cambio de Canadá correctamente.");
            Console.WriteLine("Se obtuvo el tipo de cambio de Canadá correctamente.");
            return CrearListaBD("0", tipoCambio, "CAN");
        }
    }
}