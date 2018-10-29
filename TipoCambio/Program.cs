using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TipoCambio.DataAccess;

namespace TipoCambio
{
    class Program
    {
        static void Main(string[] args)
        {
            // Se crea una instancia de la app.
            AppTiposCambio subirTiposCambio = new AppTiposCambio("tipo_cambio");

            /* Distintos ETL a ejecutar. */

            // ETL del dia.
            subirTiposCambio.SubirFecha();

            return;
        }
    }
}
