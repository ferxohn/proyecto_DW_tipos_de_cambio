using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TipoCambio.DataAccess;
using TipoCambio.BusinessRules;
using TipoCambio.Classes;

namespace TipoCambio._code.Classes
{

    class CorrerPrograma
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

        public CorrerPrograma()
        {

        }

        public void CorrerHoy()
        {
            // 1 Mexico

            IList<string> a = mex.ObtenerHoy();

            // 2 Canada

            IList<string> c = can.ObtenerHoy();

            // 3 Bolvia

            IList<string> e = bv.ObtenerHoy();

            // 4 Argentina

            IList<string> g = arg.ObtenerHoy();

            // 5 Republica Dominicana 

            IList<string> i = dom.ObtenerHoy();

            // 6 Union Europea

            IList<string> k = eur.ObtenerHoy();

            // 7 Colombia

            IList<string> m = col.ObtenerHoy();

            // 8 Brasil

            IList<string> o = bra.ObtenerHoy();


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
            string storedProcedure = "insert_update_valor";

            // 1 Mexico
            procedimiento.EjecutarSP(storedProcedure, parametros, a);
            // 2 Canada
            procedimiento.EjecutarSP(storedProcedure, parametros, c);
            // 3 Bolvia
            procedimiento.EjecutarSP(storedProcedure, parametros, e);
            // 4 Argentina
            procedimiento.EjecutarSP(storedProcedure, parametros, g);
            // 5 Republica Dominicana 
            procedimiento.EjecutarSP(storedProcedure, parametros, i);
            // 6 Union Europea
            procedimiento.EjecutarSP(storedProcedure, parametros, k);
            // 7 Colombia
            procedimiento.EjecutarSP(storedProcedure, parametros, m);
            // 8 Brasil
            procedimiento.EjecutarSP(storedProcedure, parametros, o);

            return;
        }

        public void CorrerFecha(String fecha)
        {
            // 1 Mexico

            IList<string> b = mex.ObtenerFecha(fecha);

            // 2 Canada

            IList<string> d = can.ObtenerFecha(fecha);

            // 3 Bolvia

            IList<string> f = bv.ObtenerFecha(fecha);

            // 4 Argentina

            IList<string> h = arg.ObtenerFecha(fecha);

            // 5 Republica Dominicana 

            IList<string> j = dom.ObtenerFecha(fecha);

            // 6 Union Europea

            IList<string> l = eur.ObtenerFecha(fecha);

            // 7 Colombia

            IList<string> n = col.ObtenerFecha(fecha);

            // 8 Brasil

            IList<string> p = bra.ObtenerFecha(fecha);

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
            string storedProcedure = "insert_update_valor";

            // 1 Mexico
            procedimiento.EjecutarSP(storedProcedure, parametros, b);
            // 2 Canada
            procedimiento.EjecutarSP(storedProcedure, parametros, d);
            // 3 Bolvia
            procedimiento.EjecutarSP(storedProcedure, parametros, f);
            // 4 Argentina
            procedimiento.EjecutarSP(storedProcedure, parametros, h);
            // 5 Republica Dominicana 
            procedimiento.EjecutarSP(storedProcedure, parametros, j);
            // 6 Union Europea
            procedimiento.EjecutarSP(storedProcedure, parametros, l);
            // 7 Colombia
            procedimiento.EjecutarSP(storedProcedure, parametros, n);
            // 8 Brasil
            procedimiento.EjecutarSP(storedProcedure, parametros, p);

            return;
        }

    }

}
