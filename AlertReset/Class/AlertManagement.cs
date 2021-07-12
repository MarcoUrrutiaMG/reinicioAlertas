using AlertReset.Entities;
using AlertReset.Entities.AlertManagement.Parameters;
using AlertReset.Repositories.AlertsRepository;
using LoadMasiveSCTR.Util;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AlertReset.Entities.Statements;
using AlertReset.Entities.AlertManagement;
using AlertReset.Entities.AlertManagement.Response.ResponseControl;
using AppBackground.Services;

namespace AlertReset.Class
{
    public class AlertManagement : IApplication
    {
        public void Execute(object[] parameters)
        {
            if (parameters != null && parameters.Count() > 0)
            {
                AlertsProcessRepository repository = new AlertsProcessRepository();

                ExecuteProcess process = new ExecuteProcess()
                {
                    PeriodId = parameters[0].ToString()
                };
                string directory = ConfigurationManager.AppSettings.Get("FtpRoute").ToString();

                var fileCTL = ConfigurationManager.AppSettings.Get("CTLFile");

                var routeCTL = ConfigurationManager.AppSettings.Get("CTLRoute");

                List<Alerts> alertListOnLoad = new List<Alerts>();

                List<Dictionary<string,dynamic>> usersList = new List<Dictionary<string,dynamic>>();

                List<Dictionary<string, dynamic>> complianceOfficer = new List<Dictionary<string, dynamic>>();


                GlobalVariables gv = new GlobalVariables();

                var status = 2;

                var message = ConfigurationManager.AppSettings.Get("Success").ToString();

                try

                {
                    Console.Write("Proceso Iniciado");
                    Dictionary<string, dynamic>  respInsertAlert = repository.insertAlert();
                    gv = respInsertAlert["gv"];

                    if (respInsertAlert["NCODE"] != 0)
                    {
                        Console.WriteLine("\n el respInsertAlert[NCODE] : " + respInsertAlert["NCODE"]);
                        Console.WriteLine("\n el respInsertAlert[SMESSAGE] : " + respInsertAlert["SMESSAGE"]);
                        complianceOfficer = repository.getComplianceOfficer();
                        //repository.EmailSender(3, gv.periodId, gv.processEndDate, complianceOfficer, respInsertAlert["SMESSAGE"]);
                        Console.WriteLine("\n el proceso termino con una excepcion de la bd");
                        Console.ReadLine();
                        return;
                    }

                    //repository.MoveFile(directory);

                    //repository.ExecuteBatFile(fileCTL, routeCTL);

                    if (gv.alertId.Length != 0 && gv.periodId != 0)

                    {
                        try
                        {

                            new AlertProcessService().WCCoincidencias(gv);

                            alertListOnLoad = repository.GetAlerts();

                            Console.WriteLine("\n el alertListOnLoad Count : " + alertListOnLoad.Count);

                            repository.GetClientInfo(gv.alertId, gv.periodId, gv.processStartDate, gv.processEndDate, alertListOnLoad);
                            
                            //   procesos para cargar archivos .bat
                            //new AlertProcessService().coincidencias(gv.periodId);
                            // proceso para actualizar datos de worldcheckone
                            ///new AlertProcessService().WCCoincidencias(gv);
                            //new AlertProcessService().coincidencias();


                            Dictionary<string, dynamic> respInsertUseForm = repository.InsertUserForm(gv.alertId, gv.periodId);

                            /*if (respInsertUseForm["NCODE"] != 0)
                            {
                                repository.EmailSender(3, gv.periodId, gv.processEndDate, complianceOfficer, respInsertUseForm["SMESSAGE"]);
                            }*/

                            usersList = repository.GetProfileUsers();

                            repository.insertRescatesApi();

                            //repository.insertSiniestrosApi();
                
                            //repository.procesoClienteRentas(10, gv.periodId);

                            repository.EmailSender(status, gv.periodId, gv.processEndDate, usersList,"");

                            Dictionary<string, dynamic> respUpdMonitoring = repository.UpdateMonitoringAlert(gv.alertId, status, message);
                            /*if (respUpdMonitoring["NCODE"] != 0)
                            {
                                repository.EmailSender(3, gv.periodId, gv.processEndDate, complianceOfficer, respUpdMonitoring["SMESSAGE"]);
                            }*/

                            complianceOfficer = repository.getComplianceOfficer();

                            repository.EmailSender(status, gv.periodId, gv.processEndDate, complianceOfficer,"");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("\n el ex : " + ex);
                            Console.ReadLine();
                            var exc = ex.ToString();
                            
                            string error = exc.Substring(0,100)+ "...";

                            repository.UpdateMonitoringAlert(gv.alertId, 3, error);

                            //repository.EmailSender(3, gv.periodId, gv.processEndDate, complianceOfficer,"");
                        }
                    }

                    else
                    {
                        try
                        {
                            Console.Write("\nProceso Incorrecto");

                            throw new Exception("\nNo se obtuvo la alerta ni el periódo");
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                    Console.Write("\n Proceso Termino");
                    Console.ReadLine();
                }
                catch (Exception ex)
                {
                    Console.Write("\n Error: ");
                    Console.Write(ex);
                    Console.ReadLine();
                    ex.ToString();
                }
            }

            else
            {
                Console.Write("Faltan parámetros");
            }
        }
    }
}