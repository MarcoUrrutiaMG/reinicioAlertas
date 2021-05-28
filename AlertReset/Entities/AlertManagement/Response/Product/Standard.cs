using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlertReset.Entities.AlertManagement.Response.Email
{
    public class Standard
    {
        public string Ramo { get; set; }
        public string Producto { get; set; }
        public string TipoDocumentoCont { get; set; }
        public string NumDocumentoCont { get; set; }
        public string RazonSocial { get; set; }
        public string Poliza { get; set; }
        public string InicioVigenciaPol { get; set; }
        public string FinVigenciaPol { get; set; }
    }
}
