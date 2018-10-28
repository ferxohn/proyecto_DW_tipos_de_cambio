using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TipoCambio.DataAccess;
using TipoCambio.BusinessRules;
using TipoCambio.Classes;

namespace TipoCambio
{
    class Program
    {
        static void Main(string[] args)
        {
            AppTiposCambio tiposCambio = new AppTiposCambio("AppAccess", "Data@cces18", "tipo_cambio");

            MonedaMexico mex = new MonedaMexico();
            MonedaCanada can = new MonedaCanada();
            MonedaArgentina arg = new MonedaArgentina();
            MonedaBolivia bv = new MonedaBolivia();
            MonedaBrasil bra = new MonedaBrasil();
            MonedaRepublicaDominicana dom = new MonedaRepublicaDominicana();
            MonedaColombia col = new MonedaColombia();
            MonedaEuro eur = new MonedaEuro(); 

            // 1 Mexico

            IList<string> a = mex.ObtenerHoy();
            IList<string> b = mex.ObtenerFecha("09/10/2018");

            // 2 Canada

            IList<string> c = can.ObtenerHoy();
            IList<string> d = can.ObtenerFecha("09/10/2018");

            // 3 Bolvia

            IList<string> e = bv.ObtenerHoy();
            IList<string> f = bv.ObtenerFecha("09/10/2018");

            // 4 Argentina

            IList<string> g = arg.ObtenerHoy();
            IList<string> h = arg.ObtenerFecha("09/10/2018");

            // 5 Republica Dominicana 

            IList<string> i = dom.ObtenerHoy();
            IList<string> p = dom.ObtenerFecha("09/10/2018");

            // 6 Union Europea

            IList<string> j = eur.ObtenerHoy();
            IList<string> k = eur.ObtenerFecha("19/10/2018");

            // 7 Colombia

            IList<string> l = col.ObtenerHoy();
            IList<string> m = col.ObtenerFecha("17/08/2018");

            // 8 Brasil

            IList<string> n = bra.ObtenerHoy();
            IList<string> o = bra.ObtenerFecha("12/09/2018");



            // Declaracion e inicializacion de variables.
            IList<string> parametros = new List<string>()
            {
                "@fecha",
                "@compra",
                "@venta",
                "@moneda"
            };

            // Se instancia la clase StoredProcedures para ejecutar un SP en la BD.
            StoredProcedures procedimiento = new StoredProcedures("AppAccess", "Data@cces18", "unicaribe.cdfuelu7xyg0.us-east-1.rds.amazonaws.com", "tipo_cambio");

            // Se preparan los parametros de ejecucion del SP.
            string storedProcedure = "insert_valor";

            // Se ejecuta el SP.
            procedimiento.EjecutarSP(storedProcedure, parametros, a);
        }
    }
}
