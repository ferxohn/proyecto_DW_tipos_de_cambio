using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TipoCambio.Classes
{
    /* Clase Registros: Uso de miembros estaticos.
     * Esta clase permite capturar y crear la bitacora de subidas a la BD, asi como capturar los mensajes y errores (logs) que se generan en la ejecucion de la aplicacion.
     * Ambos procesos generan archivos de texto donde se almacenan los registros que se generan.
     * Esta clase es una clase estatica, por lo que no puede ser instanciada, y sus miembros pueden ser accedidos de forma directa.
     */
    static class Registros
    {
        /* Uso de polimorfismo
         * Los siguientes metodos permiten crear o leer los archivos de registros que son necesarios para almacenar
         * los registros de la bitacora y los logs de la aplicacion. Los documentos se ubican en la carpeta
         * TipoCambio\bin\Debug del proyecto.
         * Se crean distintos metodos para aprovechar distintos comportamientos.
         */
        private static void EscribirArchivo(string usuario, string mensaje, TextWriter texto)
        {
            texto.Write("\r\nEntrada : ");
            texto.Write("{0} {1}", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString());
            texto.WriteLine(" :");
            texto.WriteLine("Usuario: {0}", usuario);
            texto.WriteLine("Mensaje: {0}", mensaje);
            texto.WriteLine("-------------------------------");
        }

        private static void EscribirArchivo(string mensaje, TextWriter texto)
        {
            texto.Write("\r\nEntrada : ");
            texto.Write("{0} {1}", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString());
            texto.WriteLine(" :");
            texto.WriteLine("Mensaje: {0}", mensaje);
            texto.WriteLine("-------------------------------");
        }

        private static void EscribirArchivo(string usuario, string moneda, string mensaje, TextWriter texto)
        {
            texto.Write("\r\nEntrada : ");
            texto.Write("{0} {1}", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString());
            texto.WriteLine(" :");
            texto.WriteLine("Usuario: {0}", usuario);
            texto.WriteLine("Moneda:  {0}", moneda);
            texto.WriteLine("Mensaje: {0}", mensaje);
            texto.WriteLine("-------------------------------");
        }

        private static void EscribirArchivo(string usuario, string mensaje, string storeProcedure, IList<string> parameters, IList<string> values, TextWriter texto)
        {
            texto.Write("\r\nEntrada : ");
            texto.Write("{0} {1}", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString());
            texto.WriteLine(" :");
            texto.WriteLine("Usuario: {0}", usuario);
            texto.WriteLine("Procedimiento:  {0}", storeProcedure);
            for (int i = 0; i < parameters.Count; i++)
            {
               texto.WriteLine("Parametros:  {0}", parameters[i]);
                texto.WriteLine("Valores:  {0}", values[i]);
            }
            texto.WriteLine("Mensaje: {0}", mensaje);
            texto.WriteLine("-------------------------------");
        }

        // Clase estatica para escribir en bitacora.txt
        public static class Bitacora
        {
            // Metodo estatico para escribir una nueva entrada en el archivo.
            public static void AgregarRegistro(string usuario, string mensaje)
            {
                using (StreamWriter bitacora = File.AppendText("bitacora.txt"))
                {
                    EscribirArchivo(usuario, mensaje, bitacora);
                }
            }

            public static void AgregarRegistro(string usuario, string mensaje, string storeProcedure, IList<string> parameters, IList<string> values)
            {
                using (StreamWriter bitacora = File.AppendText("bitacora.txt"))
                {
                    EscribirArchivo(usuario, mensaje, storeProcedure, parameters, values, bitacora);
                }
            }
        }

        // Clase estatica para escribir en log.txt
        public static class Log
        {
            /* Metodo estatico para escribir una nueva entrada en el archivo.
             * Uso de polimorfismo para aprovechar distintos comportamientos.
             */
            public static void AgregarRegistro(string usuario, string mensaje)
            {
                using (StreamWriter log = File.AppendText("log.txt"))
                {
                    EscribirArchivo(usuario, mensaje, log);
                }
            }

            public static void AgregarRegistro(string mensaje)
            {
                using (StreamWriter log = File.AppendText("log.txt"))
                {
                    EscribirArchivo(mensaje, log);
                }
            }

            public static void AgregarRegistro(string usuario, string moneda, string mensaje)
            {
                using (StreamWriter log = File.AppendText("log.txt"))
                {
                    EscribirArchivo(usuario, moneda, mensaje, log);
                }
            }
        }
    }
}