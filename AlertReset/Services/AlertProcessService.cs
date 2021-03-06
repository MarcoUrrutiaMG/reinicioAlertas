using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Diagnostics;
using AlertReset.Repositories.AlertsRepository;
using AlertReset.Entities.AlertManagement.Response.ResponseControl;
using System.Net;
using Newtonsoft.Json;

namespace AppBackground.Services
{
    class AlertProcessService
    {
        private string FolderSQL = "C:\\SqlCoincidencias";
        private AlertsProcessRepository _repository;
        static int NTIPOCARGA = 1;
        public AlertProcessService()
        {
            _repository = new AlertsProcessRepository();
        }

        public void coincidencias(int nPeriodoProceso)
        {
            string url = ConfigurationManager.ConnectionStrings["ConexionTime"].ToString();
            string[] options = url.Split(';');
            string user = options[0].Substring(options[0].IndexOf('=') + 1);
            string password = options[1].Substring(options[1].IndexOf('=') + 1);
            string concexion = options[2].Substring(options[2].IndexOf('=') + 1);
            //procedimiento que alimente los archivos
            List<parametro> items = _repository.GetListCoincidence(nPeriodoProceso);
            //List<parametro> items = new List<parametro>();
            //items.Add(
            //    new parametro()
            //    {
            //        P_NPERIODO_PROCESO = 20200930,
            //        P_NIDALERTA = 2,
            //        P_SORIGENARCHIVO = "ONU-ENTITIES",
            //        P_NIDTIPOLISTA = 1,
            //        P_NIDPROVEEDOR = 1,
            //        P_SNOMCOMPLETO = "null",
            //        P_NTIPOCARGA = 1,
            //        P_NCODE = "",
            //        P_SMESSAGE = ""
            //    });;
            StringBuilder s = new StringBuilder();
            s.AppendLine("@echo off");
            foreach (parametro item in items)
            {
                string FileName = "archivo_" + item.P_NPERIODO_PROCESO + "_" + Guid.NewGuid();
                using (FileStream fileSql = File.Open($"{FolderSQL}\\{FileName}.sql", FileMode.OpenOrCreate, FileAccess.Write))
                {
                    using (StreamWriter filaEscribeSql = new StreamWriter(fileSql))
                    {
                        filaEscribeSql.WriteLine(prepareScript(item).ToString());
                        filaEscribeSql.Close();
                        fileSql.Close();
                    }
                }
                s.AppendLine($"sqlplus {user}/{password}@{concexion} @{FolderSQL}\\{FileName}.sql");
            };
            Guid idfile = Guid.NewGuid();
            using (FileStream fileBat = File.Open($"{FolderSQL}\\{idfile}.bat", FileMode.OpenOrCreate, FileAccess.Write))
            {
                using (StreamWriter filaEscribeBat = new StreamWriter(fileBat))
                {
                    filaEscribeBat.WriteLine(s.ToString());
                    filaEscribeBat.Close();
                    fileBat.Close();
                }
            }
            ExecuteProcess($"{FolderSQL}\\{idfile}.bat", FolderSQL);
        }
        protected void ExecuteProcess(string RouteExe, string FolderSQL)
        {
            // EjecutaHilo();
            RunProcess(FolderSQL, RouteExe);
        }

        internal void WCCoincidencias(GlobalVariables gv)
        {
            List<string> items = _repository.GetListIndividues(NTIPOCARGA);
            try
            {
                for (int i = 0; i < items.Count; i++)
                {
                    
                    //Task.Run(async () =>
                    //{
                    var url = ConfigurationManager.AppSettings.Get("WC1Url").ToString(); ;
                    Dictionary<string, string> item = new Dictionary<string, string>();
                    item["name"] = items[i];
                    item["alertId"] = gv.alertId;
                    item["periodId"] = gv.periodId.ToString();
                    item["tipoCargaId"] = NTIPOCARGA.ToString();
                    string sRequest = JsonConvert.SerializeObject(item);
                    byte[] byte1 = Encoding.UTF8.GetBytes(sRequest);
                    int NLength = byte1.Length;
                    var request = (HttpWebRequest)WebRequest.Create(url);
                    request.Method = "POST";
                    request.ContentType = "application/json";
                    request.ContentLength = NLength;
                    request.Headers.Add("Cache-Control", "no-cache");
                    Stream newStream = request.GetRequestStream();
                    newStream.Write(byte1, 0, NLength);
                    try
                    {
                        using (HttpWebResponse response = (HttpWebResponse)request.GetResponseAsync().GetAwaiter().GetResult())
                        {
                            Stream Answer = response.GetResponseStream();
                            StreamReader _Answer = new StreamReader(Answer);
                            string jsontxt = _Answer.ReadToEnd();

                        }
                    }
                    catch (WebException ex)
                    {
                        System.Console.WriteLine("error :" , ex.Message + " - " + items[i]);
                    }

                    //});
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        protected void RunProcess(string Directory, string FileName)
        {
            using (var process = new System.Diagnostics.Process { StartInfo = new ProcessStartInfo { WorkingDirectory = Directory, FileName = FileName } })
            {
                process.Start();
                process.WaitForExit();
            }
        }
        public StringBuilder prepareScript(parametro item)
        {
            StringBuilder script = new StringBuilder();
            script.AppendLine("DECLARE");
            script.AppendLine("V_NCODE NUMBER;");
            script.AppendLine("V_SMESSAGE VARCHAR2(2000);");
            script.AppendLine("BEGIN");
            script.AppendLine($"PKG_BUSQ_COINCIDENCIAS_ALERTAS.SP_BUSQ_COINCIDENCIA_X_NOMBRE(P_NPERIODO_PROCESO => {item.P_NPERIODO_PROCESO},");
            script.AppendLine($"P_NIDALERTA => {2},");
            script.AppendLine($"P_NIDGRUPOSENAL => 1,");
            script.AppendLine($"P_SORIGENARCHIVO => '{item.P_SORIGENARCHIVO}',");
            script.AppendLine($"P_NIDTIPOLISTA => {item.P_NIDTIPOLISTA},");
            script.AppendLine($"P_NIDPROVEEDOR => {item.P_NIDPROVEEDOR},");
            script.AppendLine($"P_SNOMCOMPLETO => null,");
            script.AppendLine($"P_NTIPOCARGA => {item.P_NTIPOCARGA},");
            script.AppendLine("P_NCODE => V_NCODE,");
            script.AppendLine("P_SMESSAGE => V_SMESSAGE);");
            script.AppendLine("IF V_NCODE = 1 THEN");
            script.AppendLine("RAISE_APPLICATION_ERROR(-20001, V_SMESSAGE);");
            script.AppendLine("END IF;");
            script.AppendLine("END;");
            script.AppendLine("/");
            return script;
        }
        public class parametro
        {
            public int P_NPERIODO_PROCESO { get; set; }
            public int P_NIDALERTA { get; set; }
            public string P_SORIGENARCHIVO { get; set; }
            public int P_NIDTIPOLISTA { get; set; }
            public int P_NIDPROVEEDOR { get; set; }
            public string P_SNOMCOMPLETO { get; set; }
            public int P_NTIPOCARGA { get; set; }
            public string P_NCODE { get; set; }
            public string P_SMESSAGE { get; set; }
        }
    }
}
