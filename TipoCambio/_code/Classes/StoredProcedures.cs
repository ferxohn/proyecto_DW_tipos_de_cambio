using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace TipoCambio.Classes
{
    /* Definicion de la clase StoredProcedures que hereda a ConexionBD.
     * Esta clase permite realizar todas las operaciones sobre la BD a partir de Stored Procedures
     * almacenados en ella.
     */
    class StoredProcedures : ConexionBD
    {
        /* Metodos de la clase. */
        // Constructor para inicializar el constructor base.
        public StoredProcedures(string id, string password, string source, string catalog) : base(id, password, source, catalog) { }

        /* Metodo que permite ejecutar un procedimiento almacenado en la BD.
         * Este metodo recibe solamente el nombre del SP (en cadena), la lista de parametros del SP
         * (@moneda, por ejemplo), y la lista de valores a almacenar en los parametros (MXN, por ejemplo).
         * Regresa un entero de verificacion.
         */
        public int EjecutarSP(string storeProcedure, IList<string> parameters, IList<string> values)
        {
            // Declaracion e inicializacion de variables.
            int ejecucion = 0;
            SqlCommand comando = null;

            // Intento de abrir la conexion a la BD con su verificacion.
            if (AbrirConexion() != 0)
            {
                return 1;
            }

            // Se ejeuta el SP en la BD.
            try
            {
                // Se crea un objeto SqlCommand para ejecutar el SP de la BD.
                comando = new SqlCommand(storeProcedure, dataBase)
                {
                    CommandType = CommandType.StoredProcedure
                };

                // Se preparan los parametros de la ejecucion del SP.
                for (int i = 0; i < parameters.Count; i++)
                {
                    comando.Parameters.Add(new SqlParameter(parameters[i], values[i]));
                }

                // Finalmente se ejecuta el SP.
                comando.ExecuteNonQuery();

                Registros.Bitacora.AgregarRegistro(user, "El procedimiento de la BD se ejecutó correctamente.");
                Console.WriteLine("El procedimiento de la BD se ejecutó correctamente.");
            }
            catch (Exception ex)
            {
                Registros.Bitacora.AgregarRegistro(user, "Error al ejecutar el procedimiento de la BD: " + ex.Message);
                Console.WriteLine("Error al ejecutar el procedimiento de la BD: " + ex.Message);
                ejecucion = 3;
            }

            // Intento de cerrar la conexion a la BD con su verificacion.
            if (CerrarConexion() != 0)
            {
                return 2;
            }

            return ejecucion;
        }
    }
}