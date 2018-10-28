using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TipoCambio.DataAccess;
using TipoCambio.BusinessRules;
using TipoCambio.Classes;
using TipoCambio._code.Classes;

namespace TipoCambio
{
    class Program
    {
        static void Main(string[] args)
        {
            CorrerPrograma eje = new CorrerPrograma();

            eje.CorrerHoy();
            eje.CorrerFecha("09/10/2018");
        }
    }
}
