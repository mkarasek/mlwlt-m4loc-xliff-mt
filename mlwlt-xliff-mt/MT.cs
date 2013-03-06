using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Net;
using System.IO;

namespace mlwlt_xliff_mt
{
    public class MT
    {
        /* ************************************************************************************* */

        const Int32 _RequestTimeout = 3600000;
        

        /* ************************************************************************************* */

        public MT()
        { }


        /* ************************************************************************************* */
        /// <summary>
        ///     Process machine translation on the input file. This file is in in-line format (sentence per line), 
        ///     already prepared by Okapi Tikal. <n> tags for non-translatable fragments of text are present in the file.
        /// </summary>
        /// <param name="inline_input_path">Input file path</param>
        /// <param name="inline_output_path">Output file path</param>
        /// <param name="mt_engine_url">URL of the Moses service</param>
        /// <param name="mt_engine_port">Name of the engine on the service</param>
        public void process_mt_on_inLine_text(string inline_input_path, string inline_output_path, 
            string mt_engine_url, string mt_engine_port)
        {
            string httpResponseText = call_moses_http_request(inline_input_path, mt_engine_url, mt_engine_port);
            string urlWithTranslations = test_for_errors_and_get_output_file_url(httpResponseText);
            DownloadFile(urlWithTranslations, inline_output_path);
        }


        /* ************************************************************************************* */
        /// <summary>
        ///     Function calls Moses engine (POST HTTP request) and returns plain text response from the
        ///     server.
        /// </summary>
        /// <param name="file_to_be_translated">Path to the in-line text file to be machine translated</param>
        /// <param name="url_of_the_service">
        ///     URL to the service (usually perl script which takes care of 
        ///     Moses MT at the server side.
        /// </param>
        /// <param name="engine_name">Nema of the MT engine instance (taken from the configuration)</param>
        /// <returns>Plain text form of the web response. Empty in case of error.</returns>
        public static string call_moses_http_request(string file_to_be_translated, string url_of_the_service, string engine_name)
        {
            // Define a boundary string (used for POST header)
            string boundary = "----------" + DateTime.Now.Ticks.ToString("x");

            // Create a web request to given URL 
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(new Uri(url_of_the_service));
            // Fill-in the common parapeters for the request
            webRequest.ContentType = "multipart/form-data; boundary=" + boundary;
            webRequest.UserAgent = "MLW-LT XLIFF-MT Round tripping web-service";
            webRequest.Method = "POST";
            webRequest.KeepAlive = true;
            webRequest.ServicePoint.Expect100Continue = false;
            ServicePointManager.MaxServicePointIdleTime = 2000;

            // Build up the POST message header
            StringBuilder sb = new StringBuilder();
            // POST header: Name of the engine (engine parameter)
            sb.Append("--" + boundary + "\r\n");
            sb.Append("Content-Disposition: form-data; name=\"engine\"\r\n\r\n");
            sb.Append(engine_name);
            sb.Append("\r\n");
            // POST header: File contents
            sb.Append("--" + boundary + "\r\n");
            sb.Append("Content-Disposition: form-data; name=\"input\";");
            sb.Append(" filename=\"" + Path.GetFileName(file_to_be_translated) + "\"" + "\r\n");
            sb.Append("Content-Type: application/octet-stream\r\n\r\n");
            string postHeader = sb.ToString();
            byte[] postHeaderBytes = Encoding.UTF8.GetBytes(postHeader);

            // Build the trailing boundary string as a byte array ensuring the boundary appears on a line by itself
            byte[] boundaryBytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            // Count and fill-in the request length
            FileStream fileStream = new FileStream(file_to_be_translated, FileMode.Open, FileAccess.Read);
            long length = postHeaderBytes.Length + fileStream.Length + boundaryBytes.Length;
            webRequest.ContentLength = length;

            // Request Stream
            Stream requestStream = webRequest.GetRequestStream();
            // Write out our post header
            requestStream.Write(postHeaderBytes, 0, postHeaderBytes.Length);
            // Write out the file contents
            byte[] buffer = new Byte[checked((uint)Math.Min(4096, (int)fileStream.Length))];
            int bytesRead = 0;
            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                requestStream.Write(buffer, 0, bytesRead);
            // Write out the trailing boundary
            requestStream.Write(boundaryBytes, 0, boundaryBytes.Length);

            // Call the web server and get the response
            WebResponse webResponse = webRequest.GetResponse();
            Stream responseStream = webResponse.GetResponseStream();
            StreamReader sr = new StreamReader(responseStream);
            return sr.ReadToEnd();
        }        


        /* ************************************************************************************* */
        /// <summary>
        ///     Tests occurence of ERROR string under latest Tikal call in response text. 
        ///     Also returns URL address of latest <a href=""/>
        /// </summary>
        /// <param name="html_source">Entire HTML response from the Moses call.</param>
        /// <returns>Address to the file with translations. Returns an empty string in case of error(s).</returns>
        private string test_for_errors_and_get_output_file_url(String html_source)
        {
            string result = "-1";
            int endPosition;
            // Detect an occurence error in the process
            if (html_source.IndexOf("ERROR:") >= 0)
            {
                result = "";
            }

            // If previous tests succeded, try to find latest <a href=""/> for the link to the output translations
            if (result =="-1")
            {
                int startPosition = html_source.LastIndexOf("<a href=\"");
                if (startPosition >= 0)
                {
                    endPosition = html_source.IndexOf(">", startPosition);
                    if ((endPosition >= 0) && ((startPosition + 9) < (endPosition - 1)))
                    {
                        result = html_source.Substring(startPosition + 9, endPosition - startPosition - 10);
                    }
                }
            }
            return result;
        }


        /* ************************************************************************************* */
        /// <summary>
        ///     Downloads file from 'URL' and saves it in 'SaveAsFilePath'
        /// </summary>
        /// <param name="URL">URL to download from</param>
        /// <param name="SaveAsFilePath">Output file path</param>
        /// <returns></returns>
        private void DownloadFile(String URL, String SaveAsFilePath)
        {
            Stream dataStream;
            WebRequest request = WebRequest.Create(URL);
            request.Method = "GET";
            request.AuthenticationLevel = System.Net.Security.AuthenticationLevel.MutualAuthRequested;

            WebResponse response = request.GetResponse();
            dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            StreamWriter writer = new StreamWriter(SaveAsFilePath, false, Encoding.UTF8);
            String line = reader.ReadLine();
            while (line != null)
            {
                writer.WriteLine(line);
                line = reader.ReadLine();
            }
            reader.Close();
            writer.Flush();
            writer.Close();
            dataStream.Close();
            response.Close();
        }

        /* ************************************************************************************* */

    }
}
