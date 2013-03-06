using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

namespace mlwlt_xliff_mt
{
    public class Translate
    {

        /* ************************************************************************************* */
        
        const string okp_namespace = "okapi-framework:xliff-extensions";
        const string xlf_namespace = "urn:oasis:names:tc:xliff:document:1.2";
        const string its_namespace = "http://www.w3.org/2005/11/its";

        /* ************************************************************************************* */
        /// <summary>
        ///     Processing its-Translate category. Replaces <mrk/> encoded part of segment into a sentence 
        ///     using <n/> tag as this tag is treated by Moses as a piece of text which shouldn't be translated.
        /// </summary>
        /// <param name="inline_input_path">Input inline-text file</param>
        /// <param name="inline_output_path">Output inline-text file</param>
        public void process_translate_in_inLine_text(String inline_input_path, String inline_output_path)
        {
            FileInfo inline_file_in = new FileInfo(inline_input_path);
            FileInfo inline_file_out = new FileInfo(inline_output_path);
            StreamReader reader = inline_file_in.OpenText();
            StreamWriter writer = inline_file_out.CreateText();

            XmlDocument xmlDoc;

            string text;
            do
            {
                text = reader.ReadLine();

                xmlDoc = new XmlDocument();
                xmlDoc.LoadXml("<root xmlns:okp=\"okapi-framework:xliff-extensions\" xmlns:its=\"http://www.w3.org/2005/11/its\">" + text + "</root>");
                XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
                nsmgr.AddNamespace("okp", okp_namespace);
                nsmgr.AddNamespace("xlf", xlf_namespace);
                nsmgr.AddNamespace("its", its_namespace);

                foreach (XmlNode eleMrk in xmlDoc.SelectNodes("//mrk[@mtype='x-DNT' or @mtype='protected']"))
                {
                    XmlElement eleN = xmlDoc.CreateElement("n");
                    XmlAttribute atrTrans = xmlDoc.CreateAttribute("translation");
                    atrTrans.Value = eleMrk.OuterXml.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;"); //eleMrk.InnerText
                    eleN.Attributes.Append(atrTrans);
                    eleN.InnerXml = eleMrk.InnerXml;
                    xmlDoc.DocumentElement.InsertBefore(eleN, eleMrk);
                    xmlDoc.DocumentElement.RemoveChild(eleMrk);
                }
                writer.WriteLine(xmlDoc.DocumentElement.InnerXml);
            } while (text != null);

            writer.Flush();
            writer.Close();
        }

        /* ************************************************************************************* */

    }
}
