using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using TipoCambio.Classes;
using TipoCambio.BusinessRules;

namespace TipoCambio.DataAccess
{
    /*
     * Esta clase sirve de interfaz de usuario para realizar el ETL de las divisas a la BD.
     */
    class AppTiposCambio
    {
        /* Atributos de la clase. */
        // Fuente de la BD.
        private readonly string source = "unicaribe.cdfuelu7xyg0.us-east-1.rds.amazonaws.com";
        // SP a ejecutar.
        private readonly string storedProcedure = "insert_update_valor";
        // Objeto para ejecutar los SP de la BD.
        private readonly StoredProcedures dataBase = null;
        // Lista de parametros del SP a ejecutar.
        private readonly IList<string> parametrosSP = new List<string>()
        {
            "@fecha",
            "@compra",
            "@venta",
            "@moneda"
        };

    // Constructor de la clase. Solo recibe el nombre de la BD donde se subiran los tipos de cambio.
    public AppTiposCambio(string catalog)
        {
            // Solamente se inicializa la conexion a la BD.
            dataBase = new StoredProcedures("AppAccess", "Data@cces18", source, catalog);
        }

        /* Metodos de ETL: Uso de polimorfismo */

        /* Este metodo permite subir el tipo de cambio del dia en la BD.
         * Es el comportamiento por default del metodo SubirFecha.
         */
        public void SubirFecha()
        {
            // Para evitar diferencias de fecha, primero se almacena el string de la fecha del dia.
            string fecha = DateTime.Today.ToString("dd/MM/yyyy");

            // Se ejecuta el ETL.
            ETL(fecha);

            return;
        }

        /* Este metodo permite subir el tipo de cambio una fecha especifica en la BD.
         * Recibe la fecha en formato string de la forma "dd/MM/yyyy".
         */
        public void SubirFecha(string fecha)
        {
            // Se ejecuta el ETL.
            ETL(fecha);

            return;
        }

        // Metodo que permite realizar el ETL completo.
        private void ETL(string fecha)
        {
            // Declaracion e inicializacion de variables.
            MonedaMexico mxn = new MonedaMexico();
            MonedaCanada can = new MonedaCanada();
            MonedaArgentina ars = new MonedaArgentina();
            MonedaBolivia bob = new MonedaBolivia();
            MonedaBrasil brl = new MonedaBrasil();
            MonedaRepublicaDominicana dop = new MonedaRepublicaDominicana();
            MonedaColombia cop = new MonedaColombia();
            MonedaEuro eur = new MonedaEuro();
            IList<IList<string>> tiposCambio = null;

            // Se obtiene cada una de las listas con los tipos de cambio de cada divisa.
            tiposCambio = new List<IList<string>>
            {
                mxn.ObtenerFecha(fecha),
                can.ObtenerFecha(fecha),
                ars.ObtenerFecha(fecha),
                bob.ObtenerFecha(fecha),
                brl.ObtenerFecha(fecha),
                dop.ObtenerFecha(fecha),
                cop.ObtenerFecha(fecha),
                eur.ObtenerFecha(fecha),
            };

            // Finalmente se suben los valores a la BD.
            foreach (IList<string> tipoCambio in tiposCambio)
            {
                dataBase.EjecutarSP(storedProcedure, parametrosSP, tipoCambio);
            }

            return;
        }
    }
}
