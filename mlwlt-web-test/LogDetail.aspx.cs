using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

namespace mlwlt_web_test
{
    public partial class LogDetail : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Page.Controls.Clear();
                
                mlwlt_web_test.mlwlt_service.mlwlt_service mlwlt = new mlwlt_web_test.mlwlt_service.mlwlt_service();
                mlwlt.Credentials = System.Net.CredentialCache.DefaultCredentials;
                mlwlt.Timeout = 3600000;
                XmlNode xmlDoc = mlwlt.mlwlt_job_log(Request.QueryString["log"]);
                
                string strDate, strPhase, strDomain;
                DateTime dt;

                Page.Controls.Add(new LiteralControl("<table class=\"bordered\">"));
                Page.Controls.Add(new LiteralControl("<thead><tr class=\"tableHead\"><th>Time</th><th>Domain</th><th>Phase</th><th>Message</th></tr></thead><tbody>"));
                foreach (XmlNode entry in xmlDoc.SelectNodes("entry"))
                {
                    if (entry.Attributes["date"] != null) { strDate = entry.Attributes["date"].Value; } else {strDate = ""; }
                    if (entry.Attributes["domain"] != null) { strDomain = entry.Attributes["domain"].Value; } else {strDomain = ""; }
                    if (entry.Attributes["phase"] != null) { strPhase = entry.Attributes["phase"].Value;  } else { strPhase = ""; }

                    dt = DateTime.Parse(strDate);

                    Page.Controls.Add(new LiteralControl("<tr>"));
                    Page.Controls.Add(new LiteralControl("<td valign=\"top\">" + dt.ToLongTimeString() + "</td>"));
                    Page.Controls.Add(new LiteralControl("<td valign=\"top\">" + strDomain + "</td>"));
                    Page.Controls.Add(new LiteralControl("<td valign=\"top\">" + strPhase + "</td>"));
                    Page.Controls.Add(new LiteralControl("<td valign=\"top\">" + entry.InnerXml + "</td>"));
                    Page.Controls.Add(new LiteralControl("</tr>"));
                }
                Page.Controls.Add(new LiteralControl("</tbody></table>"));
            }
            catch (Exception) { Page.Controls.Clear(); Page.Controls.Add(new LiteralControl("<span style=\"color:Red;\">Error while retrieving log information.</span>")); }

        }
    }
}