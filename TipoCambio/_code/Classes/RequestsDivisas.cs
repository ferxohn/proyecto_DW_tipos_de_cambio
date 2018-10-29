using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TipoCambio.Classes
{
    /* Clase abstracta para usar en todas las monedas y poder obtener sus tipos de cambio.
     * Esta clase, al ser abstracta, no puede ser instanciada, solamente puede ser heredada.
     */
    abstract class RequestsDivisas : PeticionesWeb
    {
        // Atributos de la clase.
        protected DateTime objetoFecha;

        /* Metodos para la fecha: Uso de polimorfismo */

        /* Metodo que permite verificar la fecha almacenada en el objetoFecha,
         * comprobando que la fecha no sea en fin de semana.
         * No recibe nada, ya que se toma el valor almacenado en objetoFecha.
         * Regresa un entero de verificacion.
         */
        protected int VerificarFecha()
        {
            if (objetoFecha.DayOfWeek.ToString() == "Saturday" || objetoFecha.DayOfWeek.ToString() == "Sunday")
            {
                return -1;
            }

            return 0;
        }

        /* Metodo que permite verificar el formato de fecha ingresado de forma manual.
         * Recordar que el formato de fecha de entrada debe ser: dd/MM/yyyy.
         * Recibe un string que representa la fecha ingresada.
         * Regresa un entero de verificacion.
         */
        protected int VerificarFecha(string fecha)
        {
            //DateTime.TryParse permite comprobar que el formato de fecha ingresado sea correcto.
            if (DateTime.TryParse(fecha, out objetoFecha))
            {
                // Se verifica que la fecha ingresada no sea en fin de semana.
                if (objetoFecha.DayOfWeek.ToString() == "Saturday" || objetoFecha.DayOfWeek.ToString() == "Sunday")
                {
                    return -1;
                }

                // Como verificacion extra, se comprueba que la fecha ingresada no sea futura.
                if (objetoFecha > DateTime.Today)
                {
                    Console.WriteLine("No es posible buscar una fecha futura.");
                    return 1;
                }

                return 0;
            }

            else
            {
                Console.WriteLine("Se ha ingresado una fecha incorrecta.");
                return 1;
            }
        }

        /* Metodo para obtener la lista que se subira a la BD */

        /* Metodo que permite crear una lista con los valores obtenidos de un tipo de cambio (Formato de acuerdo a los campos en la BD).
         * Notar el uso del atributo objetoFecha para ingresar la fecha a la lista.
         */
        protected IList<string> CrearListaBD(string compra, string venta, string moneda)
        {
            IList<string> salida = new List<string>()
            {
                objetoFecha.ToString("yyyyMMdd"),
                compra,
                venta,
                moneda,
            };

            return salida;
        }

        /* Metodos abstractos a implementar en cada divisa: Uso de polimorfismo */

        /* Metodo abstracto con el cual es posible obtener el tipo de cambio de hoy.
         * No recibe parametros, y es comportamiento por defecto de ObtenerFecha.
         * Regresa la lista con los valores a subir a la BD
         */
        public abstract IList<string> ObtenerFecha();

        /* Metodo abstracto con el cual es posible obtener el tipo de cambio de un dia en especifico.
         * Regresa la lista con los valores a subir a la BD
         */
        public abstract IList<string> ObtenerFecha(string fecha);
    }
}
