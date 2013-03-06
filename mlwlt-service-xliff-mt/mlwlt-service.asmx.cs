using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.IO;
using System.Configuration;
using System.Text;
using System.Reflection;
using System.Xml;
using System.Xml.Xsl;


namespace mlwlt_service_xliff_mt
{
    /// <summary>
    ///     Moravia MLWLT XLIFF - MT Roundtrip Web-service
    /// </summary>
    [WebService(Namespace = "http://mlwlt.moravia.com/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class mlwlt_service : System.Web.Services.WebService
    {

        [WebMethod]
        public byte[] mlwlt_xliff_mt_echo(byte[] xliff_input, string fileName)
        {
            try
            {
                return xliff_input;
            }
            catch (Exception)
            {
                return null;
            }
        }


        [WebMethod]
        public byte[] mlwlt_xliff_mt_prepare(byte[] xliff_input, string xliff_input_file_name)
        {
            try
            {
                if (String.IsNullOrEmpty(xliff_input_file_name)) { xliff_input_file_name = "untitled"; }
                // Call mltlt-xliff-mt library, create a new job and process it
                mlwlt_xliff_mt.Job mlwltJob = new mlwlt_xliff_mt.Job(
                    xliff_input_file_name,
                    xliff_input,
                    Properties.Settings.Default.RepositoryRoot,
                    Properties.Settings.Default.ConfigurationFile,
                    Properties.Settings.Default.TikalJAR);
                return mlwltJob.Process();
            }
            catch (Exception)
            {
                return null;
            }
        }


        [WebMethod]
        public XmlDocument mlwlt_web_service_information()
        {
            System.Reflection.Assembly asm = Assembly.GetExecutingAssembly();
            System.IO.Stream xslStream = asm.GetManifestResourceStream("mlwlt_service_xliff_mt.configuration.xsl");

            var xslt = new XslCompiledTransform();
            xslt.Load(XmlReader.Create(xslStream));

            XmlDocument xmlDoc = new XmlDocument();
            var stm = new MemoryStream();
            xslt.Transform(Properties.Settings.Default.ConfigurationFile, null, stm);
            var sr = new StreamReader(stm);
            stm.Position = 3;
            string xmlStr = sr.ReadToEnd();
            xmlDoc.LoadXml(xmlStr);
            return xmlDoc;
        }


        [WebMethod]
        public XmlDocument mlwlt_job_list()
        {
            string strXML = "";

            DirectoryInfo dir = new DirectoryInfo(Properties.Settings.Default.RepositoryRoot);
            foreach (DirectoryInfo d in dir.GetDirectories())
            {
                strXML += String.Format("<job>{0}</job>", d.Name);
            }
            strXML = String.Format("<jobs>{0}</jobs>", strXML);
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(strXML);
            return xmlDoc;
        }


        [WebMethod]
        public XmlDocument mlwlt_job_log(string job)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(Properties.Settings.Default.RepositoryRoot + "\\" + job + "\\log.xml");
            return xmlDoc;
        }


    }
}