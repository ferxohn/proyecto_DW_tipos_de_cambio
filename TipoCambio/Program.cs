using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TipoCambio.Classes;
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
            // ETL del miercoles 17/10/2018 (valido).
            subirTiposCambio.SubirFecha("17/10/2018");
            // ETL del martes 01/05/2018 (invalido).
            subirTiposCambio.SubirFecha("01/05/2018");
            // ETL del sabado 20/10/2018 (fin de semana).
            subirTiposCambio.SubirFecha("20/10/2018");
            // ETL invalido.
            subirTiposCambio.SubirFecha("dsfsdfdsf");

            return;
        }
    }
}
