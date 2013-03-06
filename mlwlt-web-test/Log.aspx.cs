using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

namespace mlwlt_web_test
{
    public partial class Log : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            mlwlt_web_test.mlwlt_service.mlwlt_service mlwlt = new mlwlt_web_test.mlwlt_service.mlwlt_service();
            mlwlt.Credentials = System.Net.CredentialCache.DefaultCredentials;
            mlwlt.Timeout = 3600000;
            XmlNode xmlDoc = mlwlt.mlwlt_job_list();
            string lastDate = "";
            string strLogName = "";
            string strDate = "";
            string strTime = "";

            foreach (XmlNode log in xmlDoc.SelectNodes("job"))
            {
                strLogName = log.InnerText;
                if (strLogName.IndexOf(" ")>=0)
                {
                    strDate = strLogName.Split(' ')[0];
                    strTime = strLogName.Split(' ')[1];
                    if (strTime.IndexOf(".") >= 0)
                    {
                        strTime = strTime.Remove(strTime.LastIndexOf("."));
                        strTime = strTime.Replace(".", ":");
                    }
                }
                if (strDate != lastDate)
                {
                    if (lastDate != "")
                    {
                        phLogList.Controls.Add(new LiteralControl("</ul></li>"));
                    }
                    phLogList.Controls.Add(new LiteralControl("<li class=\"dropdown\" data-role=\"dropdown active\"><a><i class=\"icon-list\"></i> " + strDate + "</a><ul class=\"sub-menu light sidebar-dropdown-menu open\">"));
                    phLogList.Controls.Add(new LiteralControl("<li style=\"padding-left: 10px;\"><a href=\"javascript:displayLog('" + strLogName + "', this);\">" + strTime + "</a></li>"));
                    lastDate = strDate;
                }
                else
                {
                    phLogList.Controls.Add(new LiteralControl("<li style=\"padding-left: 10px;\"><a href=\"javascript:displayLog('" + strLogName + "', this);\">" + strTime + "</a></li>"));
                }
            }
            phLogList.Controls.Add(new LiteralControl("</ul></li>"));
            

        }
    }
}