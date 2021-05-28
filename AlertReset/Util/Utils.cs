using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;

namespace AlertReset.Util
{
    public class Utils
    {
        public static string GetFielName(string archivo)
        {
            if (!string.IsNullOrEmpty(archivo))
            {
                return Path.GetFileName(archivo);
            }

            return string.Empty;
        }

        public static string GetNewFTPRoute()
        {
            return ConfigurationManager.AppSettings.Get("FtpNewRoute");
        }

        public static string GetClients()
        {
            return ConfigurationManager.AppSettings.Get("Clients");
        }

        public static string GetParents()
        {
            return ConfigurationManager.AppSettings.Get("Parents");
        }
    }
}
