using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using TipoCambio.Classes;

namespace TipoCambio.DataAccess
{
    class AppTiposCambio
    {
        /* Atributos de la clase. */
        private readonly string source = "unicaribe.cdfuelu7xyg0.us-east-1.rds.amazonaws.com";
        private readonly StoredProcedures BD = null;

        public AppTiposCambio(string id, string password, string catalog)
        {
            BD = new StoredProcedures(id, password, source, catalog);
        }
    }
}
