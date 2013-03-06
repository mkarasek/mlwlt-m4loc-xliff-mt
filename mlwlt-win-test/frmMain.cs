using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Collections.Specialized;
using System.Net;
using System.Text.RegularExpressions;

namespace mlwlt_win_test
{
    public partial class frmMain : Form
    {

        const string configurationPath = "C:\\Projects\\Internal - .NET\\mlwlt\\configuration.xml";
        const string TikalJARfile = "C:\\Projects\\Internal - .NET\\mlwlt\\mlwlt-service-xliff-mt\\okapi_lib\\tikal.jar";

        public frmMain()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {


            Test_Job();

            //Test_MT();
            //Test_Domain();
            //Test_Language();

            //Test_Provenance();

            txtOut.Select(txtOut.Text.Length, 0);
        }


        /* ************************************************************************************* */
        private void Test_Provenance()
        {
            txtOut.Text += "******** TEST PROVENANCE:\r\n";

            mlwlt_xliff_mt.Provenance mlwltProvenance = new mlwlt_xliff_mt.Provenance();
            string newID = mlwltProvenance.get_provenance_record_id("C:\\Projects\\Internal - .NET\\mlwlt\\_xlf_inputs_from_UL\\Round-trip\\EX-xliff-prov-rt-1-post-PE-v2.xlf");
            mlwltProvenance.insert_provenance_records("C:\\Projects\\Internal - .NET\\mlwlt\\_xlf_inputs_from_UL\\Round-trip\\EX-xliff-prov-rt-1-post-PE-v2.xlf", newID);
            txtOut.Text += "New ID: " + newID + "\r\n";
        }

        /* ************************************************************************************* */
        private void Test_MT()
        {
            txtOut.Text += "******** TEST MT:\r\n";

            string FilePath_in = "C:\\Projects\\Internal - .NET\\mlwlt\\_repository\\2013-02-11 16.23.47.917\\input_UL2_MK.xlf.xlf.Classical_Studies.xlf.english.mtAdapted";
            string FilePath_out = "C:\\Projects\\Internal - .NET\\mlwlt\\_repository\\2013-02-11 16.23.47.917\\input_UL2_MK.xlf.xlf.Classical_Studies.xlf.english.mtApplied";
            mlwlt_xliff_mt.MT mlwltMT = new mlwlt_xliff_mt.MT();

            mlwltMT.process_mt_on_inLine_text(FilePath_in, FilePath_out, "http://dorchadas.moravia.com/cgi-bin/moses_run.pl", "EN_FR_europarl-1000-no");
        }

        /* ************************************************************************************* */
        private void Test_Domain()
        {
            txtOut.Text += "******** TEST DOMAIN:\r\n";

            //string FilePath = "C:\\Projects\\Internal - .NET\\mlwlt\\_xlf_inputs_from_UL\\input_UL2_MK.xlf";
            string FilePath = "C:\\Projects\\Internal - .NET\\mlwlt\\_xlf_inputs_from_UL\\Round-trip\\EX-xliff-prov-rt-1-post-term.xlf";
            
            mlwlt_xliff_mt.Domain mlwltDomain = new mlwlt_xliff_mt.Domain(FilePath, configurationPath, "");

            List<mlwlt_xliff_mt.Domain.DomainEntry> deList = mlwltDomain.mapped_domain_list;
            foreach (mlwlt_xliff_mt.Domain.DomainEntry de in deList)
            {
                foreach (string itsDomain in de.ITSdomains)
                {
                    txtOut.Text += "Domain Mapped: \"" + de.DomainName + "\" <- \"" + itsDomain + "\" [" + de.FileName_xliff_source + "] \r\n";
                }
                txtOut.Text += "src: " + de.Language_Source + "\r\n";
                txtOut.Text += "tar: " + de.Language_Target + "\r\n";
                txtOut.Text += "MT engine URL: " + de.MTengine_Url + "\r\n";
                txtOut.Text += "MT engine Port: " + de.MTengine_Port + "\r\n";
                txtOut.Text += "---\r\n";
            }

            mlwltDomain.Split();
            //mlwltDomain.Merge(FilePath + ".output.xlf");

        }

        /* ************************************************************************************* */
        private void Test_Language()
        {
            txtOut.Text += "******** TEST LANGUAGE:\r\n";

            string FilePath = "C:\\Projects\\Internal - .NET\\mlwlt\\_xlf_inputs_from_UL\\input_UL2.xlf";
            mlwlt_xliff_mt.Language mlwltLanguage = new mlwlt_xliff_mt.Language();
            txtOut.Text += "get_language_name('fr'): " + mlwltLanguage.get_language_name("fr") + "\r\n";
            txtOut.Text += "SrcLang: " + mlwltLanguage.get_xliff_source_language(FilePath) + "\r\n";
            txtOut.Text += "TarLang: " + mlwltLanguage.get_xliff_target_language(FilePath) + "\r\n";
            txtOut.Text += "SrcLang+: " + mlwltLanguage.get_language_name(mlwltLanguage.get_xliff_source_language(FilePath)) + "\r\n";
            txtOut.Text += "TarLang+: " + mlwltLanguage.get_language_name(mlwltLanguage.get_xliff_target_language(FilePath)) + "\r\n";
        }

        /* ************************************************************************************* */

        private void Test_Job()
        {
            txtOut.Text += "******** TEST JOB:\r\n";

            //string FilePath = "C:\\Projects\\Internal - .NET\\mlwlt\\_xlf_inputs_from_UL\\input_UL2_MK.xlf";
            //string FileName = "input_UL2_MK.xlf";

            //string FilePath = "C:\\Projects\\Internal - .NET\\mlwlt\\_xlf_inputs_from_UL\\input_UL2.xlf";
            //string FileName = "input_UL2.xlf";

            //string FilePath = "C:\\Projects\\Internal - .NET\\mlwlt\\_xlf_inputs_from_UL\\input_UL1.xlf";
            //string FileName = "input_UL1.xlf";

            //string FilePath = "C:\\Projects\\Internal - .NET\\mlwlt\\_xlf_inputs_from_UL\\Round-trip\\EX-xliff-prov-rt-1-post-PE-v2.xlf";
            //string FileName = "EX-xliff-prov-rt-1-post-PE-v2.xlf";

            string FilePath = "C:\\Projects\\Internal - .NET\\mlwlt\\_xlf_inputs_from_UL\\Round-trip\\EX-xliff-prov-rt-1-post-term.xlf";
            string FileName = "EX-xliff-prov-rt-1-post-term.xlf";


            string RepoPath = "C:\\Projects\\Internal - .NET\\mlwlt\\_repository\\";
            byte[] BinFile = GetBytesFromTestingFile(FilePath);


            mlwlt_xliff_mt.Job mlwltJob = new mlwlt_xliff_mt.Job(FileName, BinFile, RepoPath, configurationPath, TikalJARfile);
            mlwltJob.Process();

            txtOut.Text += "Done.\r\n";

        }
        
        /* ************************************************************************************* */

        private byte[] GetBytesFromTestingFile(string FilePath)
        {
            //Open File and read all bytes from it
            FileStream fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read);
            // Create a byte array of file stream length
            byte[] bin = new byte[fs.Length];
            //Read block of bytes from stream into the byte array
            fs.Read(bin, 0, System.Convert.ToInt32(fs.Length));
            //Close the File Stream
            fs.Close();
            return bin;
        }


#region "Moses Call snippet" 

        /* ************************************************************************************* */
        //string uploadfile = "C:\\Projects\\Internal - .NET\\mlwlt\\_repository\\2013-02-11 16.23.47.917\\input_UL2_MK.xlf.xlf.Classical_Studies.xlf.english.mtAdapted";
        //string outdata = CallMosesHTTPRequest(uploadfile, "http://dorchadas.moravia.com/cgi-bin/moses_run.pl", "EN_FR_europarl-1000-no");
        //System.IO.File.WriteAllText("C:\\Users\\milank\\Desktop\\response.htm", outdata);
        //txtOut.Text = outdata;
        /* ************************************************************************************* */
        public static string CallMosesHTTPRequest(string file_to_be_translated, string url_of_the_service, string engine_name)
        {
            //Define a boundary string (used for POST header)
            string boundary = "----------" + DateTime.Now.Ticks.ToString("x");

            //Create a web request to given URL 
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(new Uri(url_of_the_service));
            //Fill-in the common parapeters for the request
            webRequest.ContentType = "multipart/form-data; boundary=" + boundary;
            webRequest.UserAgent = "MLW-LT XLIFF-MT Round tripping web-service";
            webRequest.Method = "POST";
            webRequest.KeepAlive = true;
            webRequest.ServicePoint.Expect100Continue = false;
            ServicePointManager.MaxServicePointIdleTime = 2000;

            //Build up the POST message header
            StringBuilder sb = new StringBuilder();
            //POST header: Name of the engine (engine parameter)
            sb.Append("--" + boundary + "\r\n");
            sb.Append("Content-Disposition: form-data; name=\"engine\"\r\n\r\n");
            sb.Append(engine_name);
            sb.Append("\r\n");
            //POST header: File contents
            sb.Append("--" + boundary + "\r\n");
            sb.Append("Content-Disposition: form-data; name=\"input\";");
            sb.Append(" filename=\"" + Path.GetFileName(file_to_be_translated) + "\"" + "\r\n");
            sb.Append("Content-Type: application/octet-stream\r\n\r\n");
            string postHeader = sb.ToString();
            byte[] postHeaderBytes = Encoding.UTF8.GetBytes(postHeader);

            //Build the trailing boundary string as a byte array
            //ensuring the boundary appears on a line by itself
            byte[] boundaryBytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            //Count and fill-in the request length
            FileStream fileStream = new FileStream(file_to_be_translated, FileMode.Open, FileAccess.Read);
            long length = postHeaderBytes.Length + fileStream.Length + boundaryBytes.Length;
            webRequest.ContentLength = length;

            //Request Stream
            Stream requestStream = webRequest.GetRequestStream();
            //Write out our post header
            requestStream.Write(postHeaderBytes, 0, postHeaderBytes.Length);
            //Write out the file contents
            byte[] buffer = new Byte[checked((uint)Math.Min(4096, (int)fileStream.Length))];
            int bytesRead = 0;
            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                requestStream.Write(buffer, 0, bytesRead);
            //Write out the trailing boundary
            requestStream.Write(boundaryBytes, 0, boundaryBytes.Length);

            //Call the web server and get the response
            WebResponse webResponse = webRequest.GetResponse();
            Stream responseStream = webResponse.GetResponseStream();
            StreamReader sr = new StreamReader(responseStream);
            return sr.ReadToEnd();
        }
#endregion


    }
}
