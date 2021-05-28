using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlertReset.Entities.AlertManagement
{
    public class ExperianRiesgoResponse
    {
        public int nRiskType { get; set; }

        public string sDescript { get; set; }

        public int nFlag { get; set; }

        public string sError { get; set; }

        public string score { get; set; }

    }
}