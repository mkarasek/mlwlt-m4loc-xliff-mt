using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace mlwlt_xliff_mt
{
    class Log
    {
        string _log_file_path = "";

        /* ************************************************************************************* */
        public Log(string log_file_path)
        {
            _log_file_path = log_file_path;
        }


        /* ************************************************************************************* */
        public void save_to_log(string Phase, string Domain, string Message)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(_log_file_path);
                XmlNode newLog = xmlDoc.CreateElement("entry");
                XmlAttribute atrDate = xmlDoc.CreateAttribute("date");
                XmlAttribute atrPhase = xmlDoc.CreateAttribute("phase");
                XmlAttribute atrDomain = xmlDoc.CreateAttribute("domain");
                atrDomain.Value = Domain;
                atrPhase.Value = Phase;
                atrDate.Value = DateTime.Now.ToString("o");
                newLog.Attributes.Append(atrDate);
                newLog.Attributes.Append(atrDomain);
                newLog.Attributes.Append(atrPhase);
                newLog.InnerText = Message;
                xmlDoc.DocumentElement.AppendChild(newLog);
                xmlDoc.Save(_log_file_path);
            }
            catch (Exception) { }
        }

        
        /* ************************************************************************************* */
        public void create_new_log()
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml("<?xml version=\"1.0\" encoding=\"UTF-8\"?><log/>");
                xmlDoc.Save(_log_file_path);
                save_to_log("Start", "All", "Log created.");
            }
            catch (Exception) { }
        }
    }
}
