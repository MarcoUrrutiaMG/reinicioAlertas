using AlertReset.Entities.AlertManagement.Parameters;
using AlertReset.Util;
using Oracle.ManagedDataAccess.Types;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;
using System.Configuration;
using ExcelDataReader;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using AlertReset.Dao;
using System.Threading.Tasks;
using static AlertReset.Entities.Statements;
using AlertReset.Entities.AlertManagement;
using AlertReset.Entities.AlertManagement.Response.ResponseControl;
using Newtonsoft.Json;

namespace AlertReset.Repositories.AlertsRepository
{
    public class AlertsProcessRepository
    {
        static string PackageInUse = "PKG_LAFT_GESTION_ALERTAS";

        List<Alerts> listClientInfo = new List<Alerts>();

        GlobalVariables gv = new GlobalVariables();

        //Método para insertar el formulario del usuario
        public Dictionary<string, dynamic> InsertUserForm(string alertId, int periodId)
        {
            int Pcode;
            string Pmessage = string.Empty;
            Dictionary<string, dynamic> objRespuesta = new Dictionary<string, dynamic>();
            try
            {
                using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["Conexion"].ToString()))
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        cmd.Connection = cn;
                        cmd.CommandText = string.Format("{0}.{1}", PackageInUse, "SP_INS_FORMULARIO_USUARIO");
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("P_NPERIODO_PROCESO", OracleDbType.Int32).Value = periodId;
                        var P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, ParameterDirection.Output);
                        var P_MESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);
                        P_NCODE.Size = 2000;
                        P_MESSAGE.Size = 2000;
                        cmd.Parameters.Add(P_NCODE);
                        cmd.Parameters.Add(P_MESSAGE);
                        cn.Open();
                        cmd.ExecuteNonQuery();
                        Pcode = Convert.ToInt32(P_NCODE.Value.ToString());
                        Pmessage = Convert.ToString(P_MESSAGE.Value.ToString());
                        objRespuesta["NCODE"] = Pcode;
                        objRespuesta["SMESSAGE"] = Pmessage;
                        /*if (Pcode == 1)
                        {
                            
                            //throw new Exception(Pmessage);
                        }*/
                        cmd.Dispose();
                        
                    }
                }
            }

            catch (Exception ex)
            {

                throw ex;
            }
            Console.Write("\nFormulario Insertado");

            //return Pmessage;
            return objRespuesta;
        }

        //Método para obtener la lista de correos
        public List<Dictionary<string,dynamic>> GetProfileUsers()
        {
            //List<Users> usersList = new List<Users>();
            List<Dictionary<string, dynamic>> usersList = new List<Dictionary<string, dynamic>>(); 

            try
            {
                using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["Conexion"].ToString()))
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        cmd.Connection = cn;
                        IDataReader reader = null;
                        cmd.CommandText = string.Format("{0}.{1}", PackageInUse, "SP_GET_USUARIO_PERF_ALERTA");
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("RC1", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                        cn.Open();
                        reader = cmd.ExecuteReader();

                        if (reader != null)
                        {
                            while (reader.Read())
                            {
                                //Users item = new Users();
                                Dictionary<string, dynamic> item = new Dictionary<string, dynamic>();
                                item["SEMAIL"] = reader["SEMAIL"] == DBNull.Value ? string.Empty : (reader["SEMAIL"].ToString()).ToLower();
                                item["NOMBRECOMPLETO"] = reader["NOMBRECOMPLETO"] == DBNull.Value ? string.Empty : reader["NOMBRECOMPLETO"].ToString();
                                item["SCARGO"] = reader["SCARGO"] == DBNull.Value ? string.Empty : reader["SCARGO"].ToString();
                                item["SNOMPERFIL"] = reader["SNOMPERFIL"] == DBNull.Value ? string.Empty : reader["SNOMPERFIL"].ToString();
                                item["ID_USUARIO"] = reader["ID_USUARIO"];
                                item["NIDPROFILE"] = reader["NIDPROFILE"];
                                item["NIDGRUPOSENAL"] = reader["NIDGRUPOSENAL"];
                                item["SPERIODO_PROCESO"] = reader["SPERIODO_PROCESO"]; 
                                usersList.Add(item);
                            }
                        }
                        cn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Write("\n El error : "+ex);
                throw ex;
            }
            Console.Write("\nLista de usuario obtenida");

            return usersList;
        }

        //Método para enviar correo a los usuarios dentro de la lista de correos
        public void EmailSender(int status, int period,string endDate, List<Dictionary<string, dynamic>> infoUser,string messageError)
        {
            Console.WriteLine("\n el contar SEMAIL: "+ infoUser[0]["SEMAIL"]);

            var emails = infoUser.Select(o => o["SEMAIL"]).ToList();

            if (emails.Count != 0)
            {
                for (int i = 0; i < infoUser.Count; i++)
                {
                    var message = new MailMessage();

                    message.To.Add(new MailAddress(infoUser[i]["SEMAIL"]));

                    Dictionary<string, dynamic> objParamCorreo = new Dictionary<string, dynamic>();
                    objParamCorreo["NIDPROFILE"] = infoUser[i]["NIDPROFILE"];
                    objParamCorreo["NIDGRUPOSENAL"] = infoUser[i]["NIDGRUPOSENAL"];
                    objParamCorreo["NIDACCION"] = 1;//REINICIO DE ALERTAS

                    Console.WriteLine("\n el NIDPROFILE: "+ infoUser[i]["NIDPROFILE"]);
                    Console.WriteLine("\n el NIDGRUPOSENAL: "+ infoUser[i]["NIDGRUPOSENAL"]);
                    Console.WriteLine("\n el SEMAIL: "+ infoUser[i]["SEMAIL"]);
                    Console.WriteLine("\n el SNOMPERFIL: " + infoUser[i]["SNOMPERFIL"]);


                    Dictionary<string, dynamic> respCorreoCustom = getCorreoCustom(objParamCorreo);

                    Console.WriteLine("\n el SASUNTO_CORREO: "+ respCorreoCustom["SASUNTO_CORREO"]);
                    Console.WriteLine("\n el SCUERPO_CORREO: " + respCorreoCustom["SCUERPO_CORREO"]);
                    //Console.WriteLine(respCorreoCustom);

                    string subject = string.Empty;

                    if (infoUser[i]["NIDPROFILE"] != 2)
                    {
                        message.Subject = respCorreoCustom["SASUNTO_CORREO"];
                    }
                    else
                    {
                        message.Subject = "Señales de alerta LAFT - Generación de formularios";//respCorreoCustom["SASUNTO_CORREO"];
                    }

                    

                    /*if (infoUser[i]["NOMBRECOMPLETO"] == "Alfredo Chan Way Diaz")
                    {
                        message.Subject = ConfigurationManager.AppSettings.Get("subjectAlf");
                    }
                    else if (infoUser[i]["NOMBRECOMPLETO == "Diego Rosell Ramírez Gastón")
                    {
                        message.Subject = ConfigurationManager.AppSettings.Get("subjectDie");
                    }
                    else if (infoUser[i].NOMBRECOMPLETO == "Yvan Ruiz Portocarrero")
                    {
                        message.Subject = ConfigurationManager.AppSettings.Get("subjectYva");
                    }
                    else
                    {
                        message.Subject = ConfigurationManager.AppSettings.Get("subjectCal");
                    }*/
                    Console.WriteLine("\n el NOMBRECOMPLETO: " + infoUser[i]["NOMBRECOMPLETO"]);
                    Console.WriteLine("\n el SNOMPERFIL: " + infoUser[i]["SNOMPERFIL"]);
                    Console.WriteLine("\n el NIDPERFIL: " + infoUser[i]["NIDPROFILE"]);
                    //Console.WriteLine("\n el NIDPERFIL: " + infoUser[i]["NIDPERFIL"]);
                    Console.WriteLine("\n el SCUERPO_CORREO: " + respCorreoCustom["SCUERPO_CORREO"]);
                    Console.WriteLine("\n el endDate: " + endDate);
                    Console.WriteLine("\n el status: " + status);
                    Console.WriteLine("\n el period: " + period);

                    string nombre_completo = infoUser[i]["NOMBRECOMPLETO"];
                    string nombre_perfil = infoUser[i]["SNOMPERFIL"];
                    int nidprofile = (int)infoUser[i]["NIDPROFILE"];
                    string cuerpo_correo = respCorreoCustom["SCUERPO_CORREO"];


                    string bodyResponse = string.Empty;
                    
                    subject = message.Subject;
                    bodyResponse = ComposeBody(endDate, status, period, nombre_completo, nombre_perfil, nidprofile, bodyResponse, cuerpo_correo, infoUser[i]["SPERIODO_PROCESO"], infoUser[i]["SCARGO"]);
                    //Console.WriteLine("\n el BODY RESPONSE: " + bodyResponse);
                    message.Body = bodyResponse;
                    message.IsBodyHtml = true;
                    string fileName = string.Format("{0}{1}", ConfigurationManager.AppSettings.Get("template"), "logo.png");
                    AlternateView av = AlternateView.CreateAlternateViewFromString(bodyResponse, null, MediaTypeNames.Text.Html);
                    LinkedResource lr = new LinkedResource(fileName, MediaTypeNames.Image.Jpeg);
                    message.AlternateViews.Add(av);
                    lr.ContentId = "Logo";
                    av.LinkedResources.Add(lr);

                    try
                    {
                        using (var smtp = new SmtpClient())
                        {
                            smtp.Send(message);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("el ex en emailSender : " + ex);
                        throw ex;
                    }
                }              
            }
        }

        public Dictionary<string, dynamic> getCorreoCustom(Dictionary<string, dynamic> param)
        {
            //List<Dictionary<dynamic, dynamic>> lista = new List<Dictionary<dynamic, dynamic>>();
            try
            {
                using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["Conexion"].ToString()))
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        cmd.Connection = cn;
                        cmd.CommandText = string.Format("{0}.{1}", PackageInUse, "SP_GET_CORREO_CUSTOM");
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("P_NIDPROFILE", OracleDbType.Int32).Value = Convert.ToInt32(param["NIDPROFILE"]);
                        cmd.Parameters.Add("P_NIDGRUPOSENAL", OracleDbType.Int32).Value = Convert.ToInt32(param["NIDGRUPOSENAL"]);
                        cmd.Parameters.Add("P_NIDACCION", OracleDbType.Int32).Value = Convert.ToInt32(param["NIDACCION"]);
                        var P_SASUNTO_CORREO = new OracleParameter("P_SASUNTO_CORREO", OracleDbType.Varchar2, ParameterDirection.Output);
                        var P_SCUERPO_CORREO = new OracleParameter("P_SCUERPO_CORREO", OracleDbType.Varchar2, ParameterDirection.Output);
                        P_SASUNTO_CORREO.Size = 2000;
                        P_SCUERPO_CORREO.Size = 2000;
                        cmd.Parameters.Add(P_SASUNTO_CORREO);
                        cmd.Parameters.Add(P_SCUERPO_CORREO);
                        cn.Open();
                        cmd.ExecuteNonQuery();
                        var V_SASUNTO_CORREO = P_SASUNTO_CORREO.Value.ToString();
                        var V_SCUERPO_CORREO = P_SCUERPO_CORREO.Value.ToString();

                        Dictionary<string, dynamic> objRespuesta = new Dictionary<string, dynamic>();
                        //cmd.Dispose();
                        objRespuesta["code"] = 0;
                        objRespuesta["mensaje"] = "Se consultó con exito";
                        objRespuesta["SASUNTO_CORREO"] = V_SASUNTO_CORREO;
                        objRespuesta["SCUERPO_CORREO"] = V_SCUERPO_CORREO;


                        return objRespuesta;
                        
                    }
                }


                
            }
            catch (Exception ex)
            {
                Console.WriteLine("el ex del correo custom: " + ex);
                Dictionary<string, dynamic> objRespuesta = new Dictionary<string, dynamic>();
                objRespuesta["code"] = 2;
                objRespuesta["mensaje"] = ex.Message.ToString();
                objRespuesta["mensajeError"] = ex.ToString();
                return objRespuesta;
            }
        }

        //Método para construir el cuerpo del mensaje.
        public string ComposeBody(string endDate, int status, int period, string user, string profile,int profileId, string bodyResponse, dynamic cuerpo,string fechaPeriodo, string cargo)
        {        
            try
            {
                string statusProcess = string.Empty;
                if (status == 2)
                {
                    statusProcess = "se generó correctamente";
                }
                else
                {
                    statusProcess = "no se generó correctamente";
                }

                if (profile != null)
                {
                    if (profileId != 2)
                    {
                        try
                        {
                            string path = string.Format("{0}{1}", ConfigurationManager.AppSettings.Get("template"), ConfigurationManager.AppSettings.Get("bodyTemplate"));
                            string readText = File.ReadAllText(path);
                            string textFinal = readText.Replace("[Mensaje]", string.Format("<div>{0}</div>", cuerpo));
                            return textFinal

                                .Replace("[Usuario]", string.Format("<strong>{0}</strong>", user))
                                .Replace("[Cargo]", string.Format("<strong>{0}</strong>", cargo))
                                .Replace("[Perfil]", string.Format("<strong>{0}</strong>", profile))
                                //.Replace("[Mensaje]", string.Format("<strong>{0}</strong>", ConfigurationManager.AppSettings.Get("bodyAlf")))
                                .Replace("[FechaFin]", string.Format("<strong>{0}</strong>", fechaPeriodo))
                                .Replace("[Instruccion]", string.Format("<strong>{0}</strong>", "Por favor ingrese a este URL"))
                                .Replace("[Link]", string.Format("<strong>{0}</strong>", "http://190.216.170.173/ApplicationLAFT/"));
                        }

                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                    else
                    {                      
                        try
                        {
                            string path = string.Format("{0}{1}", ConfigurationManager.AppSettings.Get("template"), ConfigurationManager.AppSettings.Get("statusProcessTemplate"));
                            string readText = File.ReadAllText(path);
                            return readText

                                .Replace("[Usuario]",  string.Format("<strong>{0}</strong>", user))
                                .Replace("[Estado]",   string.Format("<strong>{0}</strong>", statusProcess))
                                .Replace("[Link]", string.Format("<strong>{0}</strong>", "Por favor ingrese a este URL: http://190.216.170.173/ApplicationLAFT/"));

                        }

                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                }              
            }
            catch (Exception ex)
            {
                Console.WriteLine("el ex de compose body: " + ex);
                throw ex;
            }
            return bodyResponse;
        }
        //Método que identifica los archivos de la carpeta compartida y invoca al método que los transfiere a otra ruta
        //public void MoveFile(string directory)
        //{

        //    string[] fileClients = Utils.GetClients().Split('|');

        //    string[] fileParents = Utils.GetParents().Split('|');

        //    DirectoryInfo di = new DirectoryInfo(directory);

        //    try
        //    {
        //        FileTransfer(di, directory, fileClients);
        //        FileTransfer(di, directory, fileParents);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        ////Método que transfiere los archivos y invoca el método que procesa los archivos
        //private void FileTransfer(DirectoryInfo di, string dir, string[] routes)
        //{
        //    try
        //    {
        //        foreach (string file in routes)
        //        {
        //            foreach (var fileDetail in di.GetFiles(file))
        //            {
        //                string fileRoute = dir + @"\" + fileDetail.Name;
        //                ProcessFile(fileRoute);
        //            }
        //        }
        //    }

        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    Console.Write("\nArchivos transferidos");
        //}
        //Método que ordena los archivos transferidos y invoca al método que convierte los archivos al formato csv
        //private void ProcessFile(string fileRoute)
        //{
        //    try
        //    {
        //        string newRoute = Utils.GetNewFTPRoute();

        //        string newExcelFile = Path.Combine(newRoute, Path.GetFileNameWithoutExtension(fileRoute) + "_new" + Path.GetExtension(fileRoute));
        //        //string newCsvFile = Path.Combine(newRoute, Path.GetFileNameWithoutExtension(fileRoute) + "_new" + ".csv");
        //        File.Copy(fileRoute, newExcelFile, true);
        //        //var IsConverted = SaveAsCsv(newExcelFile, newCsvFile);
        //        File.Delete(fileRoute);
        //        //File.Delete(newExcelFile);
        //    }

        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    Console.Write("\nArchivos trasladados de la carpeta de IDECON");
        //}
        //Convierte los archivos a formato csv
        //public static bool SaveAsCsv(string excelFilePath, string destinationCsvFilePath)
        //{
        //    using (var stream = new FileStream(excelFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        //    {
        //        IExcelDataReader reader = null;
        //        if (excelFilePath.EndsWith(".xls"))
        //        {
        //            reader = ExcelReaderFactory.CreateBinaryReader(stream);
        //        }
        //        else if (excelFilePath.EndsWith(".xlsx"))
        //        {
        //            reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
        //        }

        //        if (reader == null)
        //            return false;

        //        var ds = reader.AsDataSet(new ExcelDataSetConfiguration()
        //        {
        //            ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration()
        //            {
        //                UseHeaderRow = false
        //            }
        //        });

        //        var csvContent = string.Empty;
        //        int row_no = 0;
        //        while (row_no < ds.Tables[0].Rows.Count)
        //        {
        //            var arr = new List<string>();
        //            for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
        //            {
        //                arr.Add(ds.Tables[0].Rows[row_no][i].ToString().TrimStart().TrimEnd().Replace("&nbsp", " ").Replace("\r\n", " ").Replace("\n", " ")
        //                .Replace("\r", " ").Replace("\t", " ").Replace(";", " ").Replace(".nbsp,", " ").Replace("&nbsp;", " "));
        //            }
        //            row_no++;
        //            csvContent += string.Join(";", arr).ToString() + "\n";
        //        }
        //        StreamWriter csv = new StreamWriter(destinationCsvFilePath, false, System.Text.Encoding.GetEncoding(65001));
        //        csv.Write(csvContent);
        //        csv.Close();
        //        return true;

        //    }
        //}

        //Método que toma la ruta y el archivo CTL y invoca al método que los ejecuta
        public void ExecuteBatFile(string RouteExe, string FolderCTL)
        {
            RunBatFile(FolderCTL, RouteExe);
        }
        //Método que ejecuta el archivo con extensión bat.
        protected void RunBatFile(string Directory, string FileName)
        {
            using (var process = new System.Diagnostics.Process { StartInfo = new ProcessStartInfo { WorkingDirectory = Directory, FileName = FileName } })
            {
                process.Start();
                process.WaitForExit();
            }
            Console.Write("Archivo CTL ejecutado");
        }

        protected void Executethread()
        {
            Thread arrThreads;
            arrThreads = new Thread(new ThreadStart(() => RunBatFile("1", "")));
            arrThreads.Start();
        }


        public List<Dictionary<string,dynamic>>  getComplianceOfficer()
        {
            List<Dictionary<string, dynamic>> complianceOfficerList = new List<Dictionary<string, dynamic>>();

            try
            {
                
                using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["Conexion"].ToString()))
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        cmd.Connection = cn;
                        IDataReader reader = null;
                        cmd.CommandText = string.Format("{0}.{1}", PackageInUse, "SP_GET_CORREO_OFICIAL");
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("RC1", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                        cn.Open();
                        reader = cmd.ExecuteReader();

                        if (reader != null)
                        {
                            while (reader.Read())
                            {
                                Dictionary<string, dynamic> item = new Dictionary<string, dynamic>();
                                item["SEMAIL"] = reader["SEMAIL"] == DBNull.Value ? string.Empty : (reader["SEMAIL"].ToString()).ToLower();
                                item["NOMBRECOMPLETO"] = reader["NOMBRECOMPLETO"] == DBNull.Value ? string.Empty : reader["NOMBRECOMPLETO"].ToString();
                                item["SCARGO"] = reader["SNOMPERFIL"] == DBNull.Value ? string.Empty : reader["SNOMPERFIL"].ToString();
                                item["SNOMPERFIL"] = reader["SDESCARGO"] == DBNull.Value ? string.Empty : reader["SDESCARGO"].ToString();
                                item["NIDPERFIL"] = reader["NIDPROFILE"] == DBNull.Value ? 0 : Int32.Parse(reader["NIDPROFILE"].ToString());
                                item["NIDPROFILE"] = reader["NIDPROFILE"] == DBNull.Value ? 0 : Int32.Parse(reader["NIDPROFILE"].ToString());
                                item["NIDGRUPOSENAL"] = reader["NIDGRUPOSENAL"] == DBNull.Value ? 0 : Int32.Parse(reader["NIDGRUPOSENAL"].ToString());
                                item["SPERIODO_PROCESO"] = reader["SPERIODO_PROCESO"];

                                complianceOfficerList.Add(item);
                            }
                        }
                        cn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Write("\nEl error : " + ex);
                throw ex;
            }
            Console.Write("\nLista de oficiales de cumplimiento obtenida");

            return complianceOfficerList;
        }
        //Método para obtener las alertas

        //public GlobalVariables insertAlert()
        public Dictionary<string, dynamic> insertAlert()
        {
            int Pcode = 0;
            string Pmessage = string.Empty;
            GlobalVariables gv = new GlobalVariables();
            Dictionary<string, dynamic> objRespuesta = new Dictionary<string, dynamic>();
            try
            {
                using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["Conexion"].ToString()))
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        cmd.Connection = cn;
                        cmd.CommandText = string.Format("{0}.{1}", PackageInUse, "SP_INS_MONITOREO_ALERTA");
                        cmd.CommandType = CommandType.StoredProcedure;
                        var P_SID = new OracleParameter("P_SID", OracleDbType.Varchar2, ParameterDirection.Output);
                        var P_NPERIODO = new OracleParameter("P_NPERIODO_PROCESO", OracleDbType.Int32, ParameterDirection.Output);
                        var P_DFEEJECUTAPROCINI = new OracleParameter("P_DFEEJECUTAPROCINI", OracleDbType.Varchar2, ParameterDirection.Output);
                        var P_DFEEJECUTAPROCFIN = new OracleParameter("P_DFEEJECUTAPROCFIN", OracleDbType.Varchar2, ParameterDirection.Output);
                        var P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, ParameterDirection.Output);
                        var P_MESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);
                        P_SID.Size = 2000;
                        P_NPERIODO.Size = 2000;
                        P_DFEEJECUTAPROCINI.Size = 2000;
                        P_DFEEJECUTAPROCFIN.Size = 2000;
                        P_NCODE.Size = 2000;
                        P_MESSAGE.Size = 2000;
                        cmd.Parameters.Add(P_SID);
                        cmd.Parameters.Add(P_NPERIODO);
                        cmd.Parameters.Add(P_DFEEJECUTAPROCINI);
                        cmd.Parameters.Add(P_DFEEJECUTAPROCFIN);
                        cmd.Parameters.Add(P_NCODE);
                        cmd.Parameters.Add(P_MESSAGE);
                        cn.Open();
                        cmd.ExecuteNonQuery();
                        Pcode = Convert.ToInt32(P_NCODE.Value.ToString());
                        gv.processStartDate = Convert.ToString(P_DFEEJECUTAPROCINI.Value.ToString());
                        gv.processEndDate = Convert.ToString(P_DFEEJECUTAPROCFIN.Value.ToString());
                        gv.alertId = Convert.ToString(P_SID.Value.ToString());
                        gv.periodId = Convert.ToInt32(P_NPERIODO.Value.ToString());
                        Pmessage = Convert.ToString(P_MESSAGE.Value.ToString());
                        cmd.Dispose();
                        objRespuesta["NCODE"] = Pcode;
                        objRespuesta["SMESSAGE"] = Pmessage;
                        objRespuesta["gv"] = gv;
                    }
                }
            }

            catch (Exception ex)
            {
                throw ex;
            }
            Console.Write("\nSe insertó la alerta a la tabla de monitoreo");



            //return gv;
            return objRespuesta;
        }
        //Método para obtener las alertas
        public List<Alerts> GetAlerts()
        {
            List<Alerts> alertsList = new List<Alerts>();

            try
            {
                using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["Conexion"].ToString()))
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        cmd.Connection = cn;
                        IDataReader reader = null;
                        cmd.CommandText = string.Format("{0}.{1}", PackageInUse, "SP_GET_GESTION_ALERTAS");
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("RC1", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                        cn.Open();
                        reader = cmd.ExecuteReader();

                        if (reader != null)
                        {
                            while (reader.Read())
                            {
                                Alerts item = new Alerts();
                                item.NIDALERTA = reader["NIDALERTA"] == DBNull.Value ? 0 : Int32.Parse(reader["NIDALERTA"].ToString());
                                item.NINDDETQUERY = reader["NINDDETQUERY"] == DBNull.Value ? 0 : Int32.Parse(reader["NINDDETQUERY"].ToString());
                                alertsList.Add(item);
                            }
                            Console.WriteLine("\nEL COUNT " + alertsList.Count);
                        }
                        cn.Close();
                    }
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine("\nLista de alertas obtenida "+ex);
                throw ex;

            }
            Console.WriteLine("\nLista de alertas obtenida : ", alertsList);

            return alertsList;
        }
        //Método para insertar información de las alertas del cliente
        public string GetClientInfo(string alertId, int periodId, string processStartDate, string processEndDate, List<Alerts> alertsInfoClient)
        {
            int Pcode = 0;
            string Pmessage = string.Empty;
            
            try
            {
                Console.WriteLine("\nInformación de item 1: " + alertsInfoClient);
                foreach (var item in alertsInfoClient)
                {
                    Console.WriteLine("\nInformación de item 2: "+ item);
                    Console.WriteLine("\nInformación de item item.NIDALERTA: " + item.NIDALERTA);
                    Console.WriteLine("\nInformación de item item.NINDDETQUERY: " + item.NINDDETQUERY);
                    Console.WriteLine("\nInformación de item processStartDate: " + processStartDate);
                    Console.WriteLine("\nInformación de item processEndDate: " + processEndDate);
                    using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["Conexion"].ToString()))
                    {
                        using (OracleCommand cmd = new OracleCommand())
                        {
                            cmd.Connection = cn;
                            cmd.CommandText = string.Format("{0}.{1}", PackageInUse, "SP_INS_INFO_CLIENTE_ALERTA");
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add("P_NPERIODO_PROCESO", OracleDbType.Int32).Value = periodId;
                            cmd.Parameters.Add("P_NIDALERTA", OracleDbType.Int32).Value = item.NIDALERTA;
                            cmd.Parameters.Add("P_NINDDETQUERY", OracleDbType.Int32).Value = item.NINDDETQUERY;
                            cmd.Parameters.Add("P_DFEEJECUTAPROCINI", OracleDbType.Varchar2).Value = processStartDate;
                            cmd.Parameters.Add("P_DFEEJECUTAPROCFIN", OracleDbType.Varchar2).Value = processEndDate;
                            var P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, ParameterDirection.Output);
                            var P_MESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);
                            P_NCODE.Size = 2000;
                            P_MESSAGE.Size = 2000;
                            cmd.Parameters.Add(P_NCODE);
                            cmd.Parameters.Add(P_MESSAGE);
                            cn.Open();
                            cmd.ExecuteNonQuery();
                            Pcode = Convert.ToInt32(P_NCODE.Value.ToString());
                            Pmessage = Convert.ToString(P_MESSAGE.Value.ToString());
                            if (Pcode == 1)
                            {
                                Console.WriteLine("\nInformación de ex 1: " + Pmessage);
                                throw new Exception(Pmessage);
                            }
                            cmd.Dispose();
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine("\nInformación de ex 2: " + ex);
                throw ex;
            }
            Console.Write("\nInformación de alertas insertadas");

            return Pmessage;
        }

        public Dictionary<string, dynamic> UpdateMonitoringAlert(string alertId, int status, string message)

        {
            int Pcode = 0;
            string Pmessage = string.Empty;
            Dictionary<string, dynamic> objRespuesta = new Dictionary<string, dynamic>();
            try
            {
                using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["Conexion"].ToString()))
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        cmd.Connection = cn;
                        cmd.CommandText = string.Format("{0}.{1}", PackageInUse, "SP_UPD_MONITOREO_ALERTA");
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("P_SID", OracleDbType.Varchar2).Value = alertId;
                        cmd.Parameters.Add("P_NSTATUSPROC", OracleDbType.Int32).Value = status;
                        cmd.Parameters.Add("P_SMENSAJE", OracleDbType.Varchar2).Value = message;
                        var P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, ParameterDirection.Output);
                        var P_MESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);
                        P_NCODE.Size = 2000;
                        P_MESSAGE.Size = 2000;
                        cmd.Parameters.Add(P_NCODE);
                        cmd.Parameters.Add(P_MESSAGE);
                        cn.Open();
                        cmd.ExecuteNonQuery();
                        Pcode = Convert.ToInt32(P_NCODE.Value.ToString());
                        Pmessage = Convert.ToString(P_MESSAGE.Value.ToString());
                        objRespuesta["NCODE"] = Pcode;
                        objRespuesta["SMESSAGE"] = Pmessage;
                        cmd.Dispose();
                    }
                }

               
            }

            catch (Exception ex)
            {
                throw ex;
            }
            //return Pmessage;
            return objRespuesta;
        }

        public List<Dictionary<string, dynamic>> getInsRiesgoCliente(int alertId, int periodo_alerta)

        {
            //int Pcode = 0;
            //string Pmessage = string.Empty;
            List<Dictionary<string, dynamic>> list = new List<Dictionary<string, dynamic>>();
            try
            {
                using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["ConexionTime"].ToString()))
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        cmd.Connection = cn;
                        IDataReader reader = null;
                        cmd.CommandText = string.Format("{0}.{1}", "PKG_BUSQ_COINCIDENCIAS_ALERTAS", "SP_GET_CLIENTE_ALERTA_RENTAS");
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("P_NIDALERTA", OracleDbType.Int32).Value = alertId;
                        cmd.Parameters.Add("P_NPERIODO_ALERTA", OracleDbType.Int32).Value = periodo_alerta;
                        cmd.Parameters.Add("RC1", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        cn.Open();
                        reader = cmd.ExecuteReader();
                        //Pcode = Convert.ToInt32(P_NCODE.Value.ToString());
                        //Pmessage = Convert.ToString(P_MESSAGE.Value.ToString());


                        if (reader != null)
                        {
                            while (reader.Read())
                            {
                                Dictionary<string, dynamic> item = new Dictionary<string,dynamic>();
                                item["NID"] = reader["NID"] == DBNull.Value ? 0 : Int32.Parse(reader["NID"].ToString());
                                item["ID_CLIENTE"] = reader["ID_CLIENTE"] == DBNull.Value ? string.Empty : reader["ID_CLIENTE"].ToString();
                                item["SCOD_PRODUCTO"] = reader["SCOD_PRODUCTO"] == DBNull.Value ? string.Empty : reader["SCOD_PRODUCTO"].ToString();
                                item["SPRODUCTO"] = reader["SPRODUCTO"] == DBNull.Value ? string.Empty : reader["SPRODUCTO"].ToString();
                                item["SNUM_POLIZA"] = reader["SNUM_POLIZA"] == DBNull.Value ? string.Empty : reader["SNUM_POLIZA"].ToString();
                                item["NCERTIFICADO"] = reader["NCERTIFICADO"] == DBNull.Value ? 0 : Int32.Parse(reader["NCERTIFICADO"].ToString());
                                item["NTIPO_DOCUMENTO"] = reader["NTIPO_DOCUMENTO"] == DBNull.Value ? 0 : Int32.Parse(reader["NTIPO_DOCUMENTO"].ToString());
                                item["SCOD_CLIENTE"] = reader["SCOD_CLIENTE"] == DBNull.Value ? string.Empty : reader["SCOD_CLIENTE"].ToString();
                                item["SNUM_DOCUMENTO"] = reader["SNUM_DOCUMENTO"] == DBNull.Value ? string.Empty : reader["SNUM_DOCUMENTO"].ToString();
                                item["SNOM_COMPLETO"] = reader["SNOM_COMPLETO"] == DBNull.Value ? string.Empty : reader["SNOM_COMPLETO"].ToString();
                                item["DFEC_INI_POLIZA"] = reader["DFEC_INI_POLIZA"] == DBNull.Value ? string.Empty : reader["DFEC_INI_POLIZA"].ToString();
                                item["DFEC_FIN_POLIZA"] = reader["DFEC_FIN_POLIZA"] == DBNull.Value ? string.Empty : reader["DFEC_FIN_POLIZA"].ToString();
                                item["DINICIO_VIG_CERT"] = reader["DINICIO_VIG_CERT"] == DBNull.Value ? string.Empty : reader["DINICIO_VIG_CERT"].ToString();
                                item["DFIN_VIG_CERT"] = reader["DFIN_VIG_CERT"] == DBNull.Value ? string.Empty : reader["DFIN_VIG_CERT"].ToString();
                                item["DFEC_ANU"] = reader["DFEC_ANU"] == DBNull.Value ? string.Empty : reader["DFEC_ANU"].ToString();
                                item["REGIMEN"] = reader["REGIMEN"] == DBNull.Value ? 0 : Int32.Parse(reader["REGIMEN"].ToString());
                                item["NIDALERTA"] = reader["NIDALERTA"] == DBNull.Value ? 0 : Int32.Parse(reader["NIDALERTA"].ToString());
                                item["NPERIODO_ALERTA"] = reader["NPERIODO_PROCESO"] == DBNull.Value ? 0 : Int32.Parse(reader["NPERIODO_PROCESO"].ToString());
                                list.Add(item);
                            }
                        }
                        cn.Close();


                    }
                }
            }

            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }


        public string UpdateRiesgoClienteRenta(int nid, int nRiskType, string riesgo)

        {
            int Pcode = 0;
            string Pmessage = string.Empty;

            try
            {
                using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["ConexionTime"].ToString()))
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        cmd.Connection = cn;
                        cmd.CommandText = string.Format("{0}.{1}", "PKG_BUSQ_COINCIDENCIAS_ALERTAS", "SP_UPD_CLIENTE_RIESGO_RENTA");
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("P_NID", OracleDbType.Int32).Value = nid;
                        cmd.Parameters.Add("P_NRISKTYPE", OracleDbType.Int32).Value = nRiskType;
                        cmd.Parameters.Add("P_SRIESGO_FINANCIERO", OracleDbType.Varchar2).Value = riesgo;
                        var P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, ParameterDirection.Output);
                        var P_MESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);
                        P_NCODE.Size = 2000;
                        P_MESSAGE.Size = 2000;
                        cmd.Parameters.Add(P_NCODE);
                        cmd.Parameters.Add(P_MESSAGE);
                        cn.Open();
                        cmd.ExecuteNonQuery();
                        Pcode = Convert.ToInt32(P_NCODE.Value.ToString());
                        Pmessage = Convert.ToString(P_MESSAGE.Value.ToString());
                        cmd.Dispose();
                    }
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine("error 2?? " + ex);

                throw ex;
            }
            return Pmessage;
        }

        public string insertRiesgoClienteRenta(int periodo, int alerta)

        {
            int Pcode = 0;
            string Pmessage = string.Empty;

            try
            {
                using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["ConexionTime"].ToString()))
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        Console.WriteLine("\n un comentario antes del stored");
                        cmd.Connection = cn;
                        cmd.CommandText = string.Format("{0}.{1}", "PKG_BUSQ_COINCIDENCIAS_ALERTAS", "SP_INS_CLIENTE_ALERTA_RENTAS");
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("P_NPERIODO_ALERTA", OracleDbType.Int32).Value = periodo;
                        cmd.Parameters.Add("P_NIDALERTA", OracleDbType.Int32).Value = alerta;
                        var P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, ParameterDirection.Output);
                        var P_MESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);
                        P_NCODE.Size = 2000;
                        P_MESSAGE.Size = 2000;
                        cmd.Parameters.Add(P_NCODE);
                        cmd.Parameters.Add(P_MESSAGE);
                        cn.Open();
                        cmd.ExecuteNonQuery();
                        Pcode = Convert.ToInt32(P_NCODE.Value.ToString());
                        Pmessage = Convert.ToString(P_MESSAGE.Value.ToString());
                        cmd.Dispose();
                    }
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine("error 2?? " + ex);

                throw ex;
            }
            return Pmessage;
        }


        public string procesoClienteRentas(int alertId, int periodo_alerta)

        {
            //int Pcode = 0;
            //string Pmessage = string.Empty;

            try
            {

                var lista = getInsRiesgoCliente(alertId, periodo_alerta);//array
                Console.WriteLine("LONGITUD : " + lista.Count);
                foreach (var elemento in lista)
                {
                    Dictionary<string,dynamic> aRentaReq = new Dictionary<string, dynamic>();
                    aRentaReq["documentId"] = elemento["SNUM_DOCUMENTO"];//"10086068";
                    aRentaReq["documenType"] = elemento["NTIPO_DOCUMENTO"];//.ToString();//"2";
                    aRentaReq["sclient"] = elemento["ID_CLIENTE"];//"10086068";//el numero de documento es igual al cod_cliente
                    aRentaReq["userCode"] = 174;
                    aRentaReq["lastName"] = "VASQUEZ";////VASQUEZ GUEVARA LUIS JUAN";//elemento["SNOM_COMPLETO"];//.Trim();
                    //Console.WriteLine("\n el elemento['SNOM_COMPLETO']: " + elemento["SNOM_COMPLETO"]);
                    /*if (!(elemento.SNOM_COMPLETO == null || elemento.SNOM_COMPLETO == ""))
                    {
                        aRentaReq.lastName = elemento.SNOM_COMPLETO.ToString();//"QUISPE JIMENEZ FRANKLIN";
                    }*/

                    /*Console.WriteLine("EL OBJETO sclient : " + aRentaReq.sclient);
                    Console.WriteLine("EL OBJETO documentId : " + aRentaReq.documentId);
                    Console.WriteLine("EL OBJETO documenType : " + aRentaReq.documenType);
                    Console.WriteLine("EL OBJETO userClass : " + aRentaReq.userClass);
                    Console.WriteLine("EL OBJETO userCode : " + aRentaReq.userCode);
                    Console.WriteLine("EL OBJETO userCode : " + aRentaReq.lastName);*/

                    WebClient webClient = new WebClient();
                    byte[] resByte;
                    string resString;
                    byte[] reqString;

                    webClient.Headers["content-type"] = "application/json";
                    //Console.WriteLine("\n el aRentaReq: " + aRentaReq);
                    reqString = Encoding.Default.GetBytes(JsonConvert.SerializeObject(aRentaReq, Formatting.Indented));
                    //Console.WriteLine("\n el reqString: " + reqString);
                    //Console.WriteLine("reqString : ", reqString);

                    string url = "https://localhost:5001/api/sbsReport/experianInvoker";

                    resByte = webClient.UploadData(url, "post", reqString);

                    resString = Encoding.Default.GetString(resByte);

                    ExperianRiesgoResponse respObj = JsonConvert.DeserializeObject<ExperianRiesgoResponse>(resString);

                    webClient.Dispose();

                    Console.WriteLine("LA respuesta : " + resString);
                    UpdateRiesgoClienteRenta(elemento["NID"], respObj.nRiskType, respObj.sDescript);

                }
            }

            catch (Exception ex)
            {
                Console.WriteLine("error?? " + ex);
                Console.ReadLine();
                throw ex;
            }
            return "";
        }

        public string insertRescatesApi()
        {
            try
            {
                WebClient webClient = new WebClient();
                byte[] resByte;
                string resString;
                //byte[] reqString;

                webClient.Headers["content-type"] = "application/json";
                //reqString = Encoding.Default.GetBytes(JsonConvert.SerializeObject(aRentaReq, Formatting.Indented));

                //Console.WriteLine("reqString : ", reqString);

                string url = "http://localhost:3000/carga-rescates";

                resByte = webClient.DownloadData(url);

                resString = Encoding.Default.GetString(resByte);

                //var resp = JsonConvert.DeserializeObject<ExperianRiesgoResponse>(resString);

                webClient.Dispose();

                Console.WriteLine("LA respuesta : " + resString);

            }

            catch (Exception ex)
            {
                Console.WriteLine("error?? " + ex);

                throw ex;
            }
            return "";
        }
        public string insertSiniestrosApi()
        {
            try
            {
                WebClient webClient = new WebClient();
                byte[] resByte;
                string resString;
                //byte[] reqString;

                webClient.Headers["content-type"] = "application/json";
                //reqString = Encoding.Default.GetBytes(JsonConvert.SerializeObject(aRentaReq, Formatting.Indented));

                //Console.WriteLine("reqString : ", reqString);

                string url = "http://localhost:3000/carga-siniestros";

                resByte = webClient.DownloadData(url);

                resString = Encoding.Default.GetString(resByte);

                //var resp = JsonConvert.DeserializeObject<ExperianRiesgoResponse>(resString);

                webClient.Dispose();

                Console.WriteLine("LA respuesta : " + resString);

            }

            catch (Exception ex)
            {
                Console.WriteLine("error?? " + ex);

                throw ex;
            }
            return "";
        }

    }
}
