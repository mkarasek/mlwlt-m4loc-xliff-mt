using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Xml;
using System.Xml.Xsl;

namespace mlwlt_web_test
{
    public partial class _Default : System.Web.UI.Page
    {

        /* ************************************************************************************* */
        
        protected void Page_Load(object sender, EventArgs e) { }


        /* ************************************************************************************* */

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            string filename = Path.GetTempPath() + Guid.NewGuid().ToString() + ".tmp";
            file1.PostedFile.SaveAs(filename);
            
            BinaryReader br = new BinaryReader(File.Open(filename, FileMode.Open, FileAccess.Read));

            mlwlt_web_test.mlwlt_service.mlwlt_service mlwlt = new mlwlt_web_test.mlwlt_service.mlwlt_service();
            mlwlt.Credentials = System.Net.CredentialCache.DefaultCredentials;
            mlwlt.Timeout = 3600000;
            br.BaseStream.Position = 0;
            byte[] buffer = br.ReadBytes(Convert.ToInt32(br.BaseStream.Length));
            br.Close();
            byte[] wsresult = mlwlt.mlwlt_xliff_mt_prepare(buffer, file1.PostedFile.FileName.Substring(file1.PostedFile.FileName.LastIndexOf("\\") + 1));

            phOut.Visible = true;

            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                try
                {
                    xmlDoc.LoadXml(System.Text.Encoding.UTF8.GetString(wsresult));
                }
                catch (Exception)
                {
                    byte[] newArray = new byte[wsresult.Length - 3];
                    Array.Copy(wsresult, 3, newArray, 0, newArray.Length);
                    xmlDoc.LoadXml(System.Text.Encoding.UTF8.GetString(newArray));
                }
                XslCompiledTransform xslt = new XslCompiledTransform();
                xslt.Load(Server.MapPath("xml2html.xsl"));
                phOut.Controls.Add(new LiteralControl(
                    "<div style=\"margin:30px 0px 40px 0px; padding: 20px; width:90%; border:solid 1px #E0E0E0;background-color:#F8F8F8;\">" + 
                    TransformXML(xmlDoc, xslt, true) +
                    "</div>"));

            }
            catch (Exception ex)
            {
                phOut.Controls.Add(new LiteralControl("<pre style=\"color:Red\">" + ex.Message + "</pre>"));
            }

        }


        /* ************************************************************************************* */

        static public string TransformXML(XmlDocument doc, XslCompiledTransform xslt, bool isXMLcode = false)
        {
            MemoryStream stm = new System.IO.MemoryStream();
            xslt.Transform(doc, null, stm);
            StreamReader sr = new StreamReader(stm);
            stm.Position = 3;
            string strOut = sr.ReadToEnd();
            if (isXMLcode)
            {
                return strOut
                    .Replace("<div class=\"eleBody\"><span>", "<div class=\"eleBodyInLine\">")
                    .Replace("</span></div>", "</div>");
            }
            else
            {
                return strOut;
            }
        }

        /* ************************************************************************************* */

    }
}