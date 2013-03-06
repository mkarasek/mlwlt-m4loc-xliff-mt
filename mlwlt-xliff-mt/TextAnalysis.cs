using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

namespace mlwlt_xliff_mt
{
    public class TextAnalysis
    {
        /* ************************************************************************************* */

        private String _language_name = "";
        private Language mlwltLanguage = new Language();

        /* ************************************************************************************* */

        public void process_textanalysis_in_inLine_text(string inline_input_path, string inline_output_path, string language_name)
        {
            _language_name = language_name;
            FileInfo inline_file_in = new FileInfo(inline_input_path);
            FileInfo inline_file_out = new FileInfo(inline_output_path);
            StreamReader reader = inline_file_in.OpenText();
            StreamWriter writer = new StreamWriter(inline_output_path, false, Encoding.UTF8);

            XmlDocument xmlDoc;

            string text;
            do
            {
                text = reader.ReadLine();

                xmlDoc = new XmlDocument();
                xmlDoc.LoadXml("<root>" + text + "</root>");

                foreach (XmlNode eleMrk in xmlDoc.SelectNodes("//mrk[@mtype='phrase' or @mtype='x-its']"))
                {
                    XmlElement eleN = xmlDoc.CreateElement("n");
                    XmlAttribute atrTrans = xmlDoc.CreateAttribute("translation");
                    atrTrans.Value = get_disambig_mrk_node(eleMrk).OuterXml
                        .Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;"); //eleMrk.InnerText
                    eleN.Attributes.Append(atrTrans);
                    eleN.InnerXml = atrTrans.Value;
                    xmlDoc.DocumentElement.InsertBefore(eleN, eleMrk);
                    xmlDoc.DocumentElement.RemoveChild(eleMrk);
                }
                writer.WriteLine(xmlDoc.DocumentElement.InnerXml);
            } while (text != null);

            writer.Flush();
            writer.Close();
        }


        private XmlNode get_disambig_mrk_node(XmlNode eleMrk)
        {
            XmlNode result = eleMrk.Clone();
            String translation = "";

            if (result.Attributes["its:taIdentRef"] != null)
            {
                if (result.Attributes["its:taIdentRef"].Value.Trim() != "")
                {
                    translation = get_disambig_translation(result.Attributes["its:taIdentRef"].Value.Trim());
                    if (translation != "")
                    {
                        result.InnerText = translation;
                    }
                }
            }

            if (result.Attributes["comment"] != null)
            {
                if (result.Attributes["comment"].Value.Trim() != "")
                {
                    translation = get_disambig_translation(result.Attributes["comment"].Value.Trim());
                    if (translation != "")
                    {
                        result.InnerText = translation;
                    }
                }
            }
            return result;
        }

        private String get_disambig_translation(String url)
        {
            string result = "";
            string url_translated = url;
            string language_code = "";

            if (url.IndexOf("dbpedia.org") >= 0)
            {
                url_translated = url.Replace(".org/page/", ".org/data/").Replace(".org/resource/", ".org/data/").Trim() + ".rdf";
                // Get the dbpedia language code
                language_code = mlwltLanguage.get_language_code_for_engine(_language_name, "dbpedia"); 
            }

            //return url_translated;

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(url_translated);

            try
            {
                foreach (XmlElement ele in xmlDoc.SelectNodes("//*"))
                {
                    if (ele.Name.IndexOf(":label") >= 0)
                    {
                        if (ele.Attributes["xml:lang"].Value.ToLower().Trim() == language_code.ToLower().Trim())
                        {
                            result = ele.InnerText;
                            return result;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                return "\n" + ex.Message + "\n";
            }
            return result;
        }

    }
}
