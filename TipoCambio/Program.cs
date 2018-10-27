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

            col.ObtenerFecha("17/08/2018");

            //bra.ObtenerFecha("12/09/2018");
            

            // Prueba con el dia de hoy.
            IList<string> a = mex.ObtenerHoy();
            IList<string> b = can.ObtenerHoy();

            // Prueba con un dia cualquiera.
            IList<string> c = mex.ObtenerFecha("09/10/2018");
            IList<string> d = can.ObtenerFecha("09/10/2018");

            // Prueba con un sabado.
            IList<string> e = mex.ObtenerFecha("13/10/2018");
            IList<string> f = can.ObtenerFecha("13/10/2018");

            // Prueba con una fecha futura.
            IList<string> g = mex.ObtenerFecha("08/10/2019");
            IList<string> h = can.ObtenerFecha("08/10/2019");

            // Prueba con un string invalido.
            IList<string> i = mex.ObtenerFecha("08/102019");
            IList<string> j = can.ObtenerFecha("08/102019");

            // Prueba Bolvia
            IList<string> m = bv.ObtenerHoy();
            IList<string> n = bv.ObtenerFecha("09/10/2018");

            // Prueba Argentina
            IList<string> k = arg.ObtenerHoy();
            IList<string> l = arg.ObtenerFecha("09/10/2018");

            //Prueba Republica Dominicana 
            IList<string> o = dom.ObtenerHoy();
            IList<string> p = dom.ObtenerFecha("09/10/2018");

            //Prueba xml Euro
            IList<string> q = eur.ObtenerHoy();
            IList<string> r = eur.ObtenerFecha("19/10/2018");


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
