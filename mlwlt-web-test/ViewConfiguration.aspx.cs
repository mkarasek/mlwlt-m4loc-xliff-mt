using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Xml;
using System.Xml.Xsl;
using System.IO;
using System.Text.RegularExpressions;

namespace mlwlt_web_test
{
    public partial class ViewConfiguration : System.Web.UI.Page
    {

        /* ************************************************************************************* */
        
        protected void Page_Load(object sender, EventArgs e)
        {
            mlwlt_web_test.mlwlt_service.mlwlt_service frw = new mlwlt_web_test.mlwlt_service.mlwlt_service();
            frw.Credentials = System.Net.CredentialCache.DefaultCredentials;

            XmlNode node = frw.mlwlt_web_service_information();
            if (node != null)
            {
                System.Xml.XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(node.OuterXml);


                XslCompiledTransform xslt = new XslCompiledTransform();
                xslt.Load(Server.MapPath("ConfigurationView.xsl"));
                phOut.Controls.Add(new LiteralControl(TransformXML(xmlDoc, xslt)));


                xslt = new XslCompiledTransform();
                xslt.Load(Server.MapPath("xml2html.xsl"));
                phOutXML.Controls.Add(new LiteralControl(TransformXML(xmlDoc, xslt, true)));
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
    }
}