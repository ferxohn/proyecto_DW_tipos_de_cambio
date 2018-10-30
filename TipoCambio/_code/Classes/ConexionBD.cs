using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace TipoCambio.Classes
{
    /* Definicion de la clase ConexionBD:
     * Esta clase permite realizar la conexion a una BD SQL Server. Todas las operaciones a realizar sobre la BD
     * se propone realizarlas usando la clase StoredProcedures.
     */
    class ConexionBD
    {
        /* Ejemplos de cadena de conexion a la BD:
         * A nuestra BD remota:
         * data source=unicaribe.cdfuelu7xyg0.us-east-1.rds.amazonaws.com; initial catalog=tipo_cambio; user id=AppAccess; password=Data@cces18
         * A una BD local:
         * data source=DESKTOP-6BV572B\\SQLEXPRESS;initial catalog=tipo_cambio; integrated security=True
         */

        /* Atributos de la clase. */
        protected SqlConnection dataBase = null;
        protected readonly string user = null;

        /* Metodos de la clase. */
        // Constructor de la clase. Los parametros recibidos permiten generar la cadena de conexion a la BD
        public ConexionBD(string id, string password, string source, string catalog)
        {
            // Cadena de conexion a la BD.
            string cadena = "data source=" + source + "; initial catalog=" + catalog + "; user id=" + id + "; password=" + password;

            // Objeto Sqlconnection con la conexion a la BD.
            dataBase = new SqlConnection
            {
                ConnectionString = cadena
            };

            // Se almacena el usuario en la clase.
            user = id;
        }

        /* Metodo AbrirConexion.
         * Solamente abre la conexion a la BD a partir del atributo dataBase. Regresa un entero de verificacion
         */
        public int AbrirConexion()
        {
            // Declaracion e inicializacion de variables.
            int ejecucion = 0;

            try
            {
                dataBase.Open();
                Registros.Bitacora.AgregarRegistro(user, "Conexión a la BD abierta correctamente.");
                Console.WriteLine("Conexión a la BD abierta correctamente.");
            }
            catch (Exception ex)
            {
                Registros.Bitacora.AgregarRegistro(user, "Error al abrir la conexión a la BD: " + ex.Message);
                Console.WriteLine("Error al abrir la conexión a la BD: " + ex.Message);
                ejecucion = 1;
            }

            return ejecucion;
        }

        /* Metodo CerrarConexion.
         * Solamente cierra la conexion a la BD a partir del atributo dataBase. Regresa un entero de verificacion
         */
        public int CerrarConexion()
        {
            // Declaracion e inicializacion de variables.
            int ejecucion = 0;

            try
            {
                dataBase.Close();
                Registros.Bitacora.AgregarRegistro(user, "Conexión a la BD cerrada correctamente.");
                Console.WriteLine("Conexión a la BD cerrada correctamente.");
            }
            catch (Exception ex)
            {
                Registros.Bitacora.AgregarRegistro(user, "Error al cerrar la conexión a la BD: " + ex.Message);
                Console.WriteLine("Error al cerrar la conexión a la BD: " + ex.Message);
                ejecucion = 2;
            }

            return ejecucion;
        }
    }
}
