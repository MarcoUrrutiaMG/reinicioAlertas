using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlertReset.Entities.AlertManagement
{
    public class AlertaRenta
    {
        public int NID { get; set; }
        public string SCOD_PRODUCTO { get; set; }
        public string SPRODUCTO { get; set; }
        public string SNUM_POLIZA { get; set; }
        public int NCERTIFICADO { get; set; }
        public int NTIPO_DOCUMENTO { get; set; }
        public string SCOD_CLIENTE { get; set; }
        public string SNUM_DOCUMENTO { get; set; }
        public string SNOM_COMPLETO { get; set; }
        public string DFEC_INI_POLIZA { get; set; }
        public string DFEC_FIN_POLIZA { get; set; }
        public string DINICIO_VIG_CERT { get; set; }
        public string DFIN_VIG_CERT { get; set; }
        public string DFEC_ANU { get; set; }
        public int REGIMEN { get; set; }
        public int NIDALERTA { get; set; }
        public int NPERIODO_ALERTA { get; set; }
    }
}
