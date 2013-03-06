using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Reflection;

namespace mlwlt_xliff_mt
{
    public class Language
    {
        /* ************************************************************************************* */
        /// <summary>
        ///     Returns source language from XLIFF File.
        /// </summary>
        /// <param name="xliff_input_path">Path to the XLIFF file</param>
        /// <returns>Language defined in @source-language attribute of &lt;file&gt; element</returns>
        public String get_xliff_source_language(String xliff_input_path)
        {
            return get_xliff_language(xliff_input_path);
        }

        /* ************************************************************************************* */
        /// <summary>
        ///     Returns target language from XLIFF File.
        /// </summary>
        /// <param name="xliff_input_path">Path to the XLIFF file</param>
        /// <returns>Language defined in @source-language attribute of &lt;file&gt; element</returns>
        public string get_xliff_target_language(string xliff_input_path)
        {
            return get_xliff_language(xliff_input_path, true);
        }

        /* ************************************************************************************* */
        // Returns language from XLIFF File.
        // By default returns Source Language. In case Target=True, target language is retured.
        private string get_xliff_language(string xliff_input_path, bool Target=false)
        {
            string Result = "";
            string AttributeName = Target ? "target-language" : "source-language";
            XmlDocument xliffDoc = new XmlDocument();
            xliffDoc.Load(xliff_input_path);
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(xliffDoc.NameTable);
            //nsmgr.AddNamespace("okp", "okapi-framework:xliff-extensions");
            nsmgr.AddNamespace("xlf", "urn:oasis:names:tc:xliff:document:1.2");
            XmlElement xliffFile = (XmlElement)xliffDoc.SelectSingleNode("/xlf:xliff/xlf:file", nsmgr);
            if (xliffFile != null)
            {
                if (xliffFile.Attributes[AttributeName] != null)
                {
                    Result = xliffFile.Attributes[AttributeName].Value;
                }
            }
            return Result;
        }

        /* ************************************************************************************* */
        /// <summary>
        ///     Get Language Name based on (a fragment of) text indicating the language
        /// </summary>
        /// <param name="language_text">Language name to identify</param>
        /// <returns>Full language name</returns>
        public string get_language_name(string language_text)
        {
            System.Reflection.Assembly asm = Assembly.GetExecutingAssembly();
            System.IO.Stream xmlStream = asm.GetManifestResourceStream("mlwlt_xliff_mt.Language.xml");
            XmlDocument xmlLang = new XmlDocument();
            xmlLang.Load(xmlStream);
            XmlNodeList foundNodes = null;
            //search in <name> elements
            foundNodes = xmlLang.SelectNodes(String.Format("/langs/lang[normalize-space(translate(name, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'))='{0}']", language_text.ToLower().Trim()));
            if (foundNodes.Count > 0)
            {
                return foundNodes[0].SelectSingleNode("name").InnerText;
            }
            else
            {
                //search in <code> elements
                foreach (XmlElement eleLang in xmlLang.SelectNodes("/langs/lang"))
                {
                    foreach (XmlElement eleCode in eleLang.SelectNodes("code"))
                    {
                        if (language_text.ToLower().Trim().IndexOf(eleCode.InnerText.ToLower().Trim()) == 0)
                        {
                            return eleLang.SelectSingleNode("name").InnerText;
                        }
                    }
                }
                return "";
            }
        }


        /* ************************************************************************************* */
        /// <summary>
        ///     Get Language code based on language name and engine code
        /// </summary>
        /// <param name="language_name">Language name</param>
        /// <param name="engine_name">Engine Name (AO, LW, GT, DBpedia)</param>
        /// <returns>Language code for given engine</returns>
        public string get_language_code_for_engine(string language_name, string engine_name)
        {
            System.Reflection.Assembly asm = Assembly.GetExecutingAssembly();
            System.IO.Stream xmlStream = asm.GetManifestResourceStream("mlwlt_xliff_mt.Language.xml");
            XmlDocument xmlLang = new XmlDocument();
            xmlLang.Load(xmlStream);
            //search in <name> elements
            XmlNode foundNode = xmlLang.SelectSingleNode(String.Format("//lang[name='{0}']/code[@engine='{1}']", language_name, engine_name));
            if (foundNode != null)
            {
                return foundNode.InnerText.Trim();
            }
            else
            {
                return "";
            }
        }

        /* ************************************************************************************* */

    }
}
