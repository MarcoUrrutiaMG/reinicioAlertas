using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadMasiveSCTR.Util
{
    public static class LogHelper
    {
        public enum Paso
        {
            GenerateFile = 10,
            Start = 1,
            CreateDirectory = 2,
            CopyZip = 3,
            ExtractZip = 4,
            ValidateFiles = 5,
            CreateCTL = 6,
            ExecuteBat = 7,
            ExecuteStore = 8,
            ComplementInformation = 9,
            ParametersIncomplete = 11
        }

        private static void WriteEntry(string message, EventLogEntryType type)
        {
            using (EventLog eventLog = new EventLog("Application"))
            {
                eventLog.Source = "Application";
                eventLog.WriteEntry(message, type, 101, 1);
            }
        }

        public static void Exception(string process, Paso paso, string message)
        {
            WriteEntry(string.Format("ServicioCargaMasiva ({0}). Paso: {1}. Metodo: {2}. Error: {3}", process, (int)paso, paso.ToString(), message), EventLogEntryType.Error);
        }

        public static void Information(string process, string key, int mails, string[] fileEntries)
        {
            string message1 = string.Format("String({0}). Key: {1}. Start OK.", process, key);
            string message2 = " End OK.";
            string message3 = " End NOK(File List -> Count = 0).";
            string message4 = " End NOK(File List -> Null).";
            string message5 = " End NOK(Mail List -> Count = 0).";

            if (mails > 0)
            {
                if (fileEntries != null)
                {
                    if (fileEntries.Length > 0)
                    {
                        message1 += message2;
                    }
                    else
                    {
                        message1 += message3;
                    }
                }
                else
                {
                    message1 += message4;
                }
            }
            else
            {
                message1 += message5;
            }

            WriteEntry(message1, EventLogEntryType.Information);
        }
    }
}
