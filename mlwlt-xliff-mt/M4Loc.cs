using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace mlwlt_xliff_mt
{
    public class M4Loc
    {
        /* ************************************************************************************* */

        const string okp_namespace = "okapi-framework:xliff-extensions";
        const string xlf_namespace = "urn:oasis:names:tc:xliff:document:1.2";
        const string its_namespace = "http://www.w3.org/2005/11/its";
        const string itsx_namespace = "http://www.w3.org/2005/11/itsx";

        /* ************************************************************************************* */

        private string _path_to_tikal_jar_file = "";

        /* ************************************************************************************* */

        public M4Loc(string path_to_tikal_jar_file)
        {
            _path_to_tikal_jar_file = path_to_tikal_jar_file;
        }

        /* ************************************************************************************* */
        /// <summary>
        ///     Function converting input XLIFF file into an in-line text format, suitable for M4Loc process. In-line text
        ///     is simple plain-text file format with segment per line, in-line tags of the segments are included as well.
        /// </summary>
        /// <param name="xliff_input_path">Input XLIFF file</param>
        public void convert_xliff_to_inline_text(string xliff_input_path)
        {
            //Get the source and target languages (used later as parameters for Tikal)
            Language mlwltLanguage = new Language();
            string srcLang = mlwltLanguage.get_xliff_source_language(xliff_input_path);
            string tarLang = mlwltLanguage.get_xliff_target_language(xliff_input_path);
            
            //Calling Java applet
            // java -jar tikal.jar -xm Lion_Sample1.xlf -sl English -tl Spanish -to Lion_Sample1.xlf.txt
            String cmd = String.Format("-jar \"{0}\" -xm \"{1}\" -ie utf-8 -sl {2} -tl {3}", 
                _path_to_tikal_jar_file, 
                xliff_input_path, 
                srcLang, 
                tarLang);
            run_a_jar(cmd);
        }


        /* ************************************************************************************* */
        public void convert_inline_to_xliff_text(string xliff_input_path, string inline_input_path, string xliff_output_path)
        {
            //Get the source and target languages (used later as parameters for Tikal)
            Language mlwltLanguage = new Language();
            string srcLang = mlwltLanguage.get_xliff_source_language(xliff_input_path);
            string tarLang = mlwltLanguage.get_xliff_target_language(xliff_input_path);

            //Calling Java applet
            // java -jar tikal.jar -xm Lion_Sample1.xlf -sl English -tl Spanish -to Lion_Sample1.xlf.txt
            String cmd = String.Format("-jar \"{0}\" -lm \"{1}\" -sl {2} -tl {3} -from \"{4}\" -to \"{5}\"",
                _path_to_tikal_jar_file, 
                xliff_input_path, 
                srcLang, 
                tarLang, 
                inline_input_path, 
                xliff_output_path);
            run_a_jar(cmd);
        }


        /* ************************************************************************************* */
        /// <summary>
        ///     Runs a Java JAR package (for Tikal calling)
        /// </summary>
        /// <param name="command">
        ///     The ommand line of the Java.exe application 
        ///     (everything succeeded Java.exe on the command line).
        /// </param>
        private void run_a_jar(String command)
        {
            System.Diagnostics.Process pProcess;
            pProcess = new System.Diagnostics.Process();
            pProcess.StartInfo.FileName = "java.exe";
            pProcess.StartInfo.Arguments = command;
            pProcess.StartInfo.UseShellExecute = false;
            pProcess.StartInfo.RedirectStandardInput = true;
            pProcess.StartInfo.CreateNoWindow = true;
            pProcess.StartInfo.RedirectStandardOutput = true;
            pProcess.StartInfo.WorkingDirectory = "";
            pProcess.Start();
            String strOutput = pProcess.StandardOutput.ReadToEnd();
            pProcess.WaitForExit();
            if (strOutput.IndexOf("Done") < 0)
            {
                //error  
            }
        }

        /* ************************************************************************************* */
        /// <summary>
        ///     Preparation of the XLIFF file for M4Loc process.
        /// </summary>
        /// <param name="xliff_file_path">Path to the xliff file</param>
        public void prepare_xliff_for_moses(string xliff_file_path)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xliff_file_path);
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
            nsmgr.AddNamespace("okp", okp_namespace);
            nsmgr.AddNamespace("xlf", xlf_namespace);
            nsmgr.AddNamespace("its", its_namespace);
            nsmgr.AddNamespace("itsx", itsx_namespace);

            // mtype of "protected" is changed to a different value because of functionality of Tikal
            foreach (XmlNode eleMrk in xmlDoc.SelectNodes("//xlf:mrk[@mtype='protected']", nsmgr))
            {
                eleMrk.Attributes["mtype"].Value = "x-DNT";
            }

            // In case there is a segmentation using <seg-source>, re-copy segments 
            // in a segmented form to the <source> element (for better Tikal functionality)
            foreach (XmlNode eleTUsegmented in xmlDoc.SelectNodes("//xlf:trans-unit[xlf:seg-source]", nsmgr))
            {
                XmlNode eleSegSource = eleTUsegmented.SelectSingleNode("xlf:seg-source", nsmgr);
                XmlNode eleSource = eleTUsegmented.SelectSingleNode("xlf:source", nsmgr);
                eleSource.InnerXml = "";
                foreach (XmlNode eleMrk in eleSegSource.SelectNodes("xlf:mrk[@mtype='seg']", nsmgr))
                {
                    eleSource.InnerXml += " " + eleMrk.InnerXml;
                }
            }

            // Remove existing <alt-trans> elements in the source file.
            foreach (XmlNode eleAltTrans in xmlDoc.SelectNodes("//xlf:alt-trans", nsmgr))
            {
                eleAltTrans.ParentNode.RemoveChild(eleAltTrans);
            }

            xmlDoc.Save(xliff_file_path);
        }

        /* ************************************************************************************* */
        /// <summary>
        ///     There are still some issues in generated xliff files after Tikal "-lm" call. This function fixes following:
        ///     - x-dnt value of <mrk mtype=""/> is recovered back to "protected".
        ///     - in-line tags on the output. Condition: Remove <g><x><ept><bpt><bx><ex> which has id and this 
        ///       id is NOT present in the <source> element.
        ///     - target language is fixed as well
        /// </summary>
        /// <param name="xliff_file_path">Path to the xliff file</param>
        public void fix_xliff_after_moses(string xliff_file_path, string provenance_id)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xliff_file_path);

            XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
            nsmgr.AddNamespace("okp", okp_namespace);
            nsmgr.AddNamespace("xlf", xlf_namespace);
            nsmgr.AddNamespace("its", its_namespace);
            nsmgr.AddNamespace("itsx", itsx_namespace);

            // Add an its: namespace to the document element
            XmlAttribute ns = xmlDoc.CreateAttribute("xmlns", "its", "http://www.w3.org/2000/xmlns/");
            ns.Value = its_namespace;
            xmlDoc.DocumentElement.Attributes.Append(ns);

            // Fix target language names
            Language mlwltLanguage = new Language();
            string targetLanguage = mlwltLanguage.get_xliff_target_language(xliff_file_path);
            foreach (XmlNode eleTarget in xmlDoc.SelectNodes("//xlf:target", nsmgr))
            {
                if (eleTarget.Attributes["xml:lang"] != null)
                {
                    eleTarget.Attributes["xml:lang"].Value = targetLanguage;
                }
            }
            
            // Remove in-line tags which doesn't exists in <source>
            foreach (XmlNode eleAltTransTarget in xmlDoc.SelectNodes("//xlf:alt-trans[@origin='Moses-MT']/xlf:target", nsmgr))
            {
                foreach (XmlNode eleInLine in eleAltTransTarget.SelectNodes(
                    "xlf:*[(name(.)='g' or name(.)='x' or name(.)='ept' or name(.)='bpt' or name(.)='bx' or name(.)='ex') and (.='')]", nsmgr))
                {
                    if (eleInLine.Attributes["id"] != null)
                    {
                        string inLineID = eleInLine.Attributes["id"].Value;
                        XmlNode xmlSource = eleAltTransTarget.ParentNode.ParentNode.SelectSingleNode("xlf:source", nsmgr);
                        if (xmlSource != null)
                        {
                            if (xmlSource.SelectNodes(
                                "xlf:*[(name(.)='" + eleInLine.Name + "') and (@id='" + inLineID + "')]", 
                                nsmgr).Count == 0)
                            {
                                eleAltTransTarget.RemoveChild(eleInLine);
                            }
                        }
                    }
                    
                }
                // Fix the &lt;mrk -> <mrk
                eleAltTransTarget.InnerXml = eleAltTransTarget.InnerXml.Replace("&lt;mrk ", "<mrk ").Replace("&lt;/mrk", "</mrk").Replace("&gt;", ">");
            }

            
            foreach (XmlNode eleAltTrans in xmlDoc.SelectNodes("//xlf:alt-trans[@origin='Moses-MT']", nsmgr))
            {
                if (eleAltTrans.Attributes["okp:matchType"] != null)
                {
                    eleAltTrans.Attributes.Remove(eleAltTrans.Attributes["okp:matchType"]);
                    eleAltTrans.Attributes.Remove(eleAltTrans.Attributes["xmlns:okp"]);
                }

                // Fix the @origin and @match-quality values
                eleAltTrans.Attributes["origin"].Value = "MT";
                if (eleAltTrans.Attributes["match-quality"] != null)
                {
                    eleAltTrans.Attributes["match-quality"].Value = "0.749";
                }
                else
                {
                    XmlAttribute matchQuality = xmlDoc.CreateAttribute("match-quality", xlf_namespace);
                    matchQuality.Value = "0.749";
                    eleAltTrans.Attributes.Append(matchQuality);
                }

                // Add a provenance information to the alt-trans <target>
                XmlNode eleAltTransTarget = eleAltTrans.SelectSingleNode("xlf:target", nsmgr);
                if (eleAltTransTarget != null)
                {
                    XmlAttribute atrProvID = xmlDoc.CreateAttribute("its:provenanceRecordsRef", its_namespace);
                    atrProvID.Value = "#" + provenance_id;
                    eleAltTransTarget.Attributes.Append(atrProvID);
                }

                XmlNode eleSource = eleAltTrans.ParentNode.SelectSingleNode("xlf:source", nsmgr);
                if ((eleSource != null) && (eleAltTransTarget != null))
                {
                    if (char.IsLower(eleSource.InnerXml.Trim()[0]) && char.IsLetter(eleAltTransTarget.InnerXml.Trim()[0]))
                    {
                        eleAltTransTarget.InnerXml = char.ToLower(eleAltTransTarget.InnerXml.Trim()[0]) + eleAltTransTarget.InnerXml.Trim().Substring(1);
                    }
                    if (char.IsUpper(eleSource.InnerXml.Trim()[0]) && char.IsLetter(eleAltTransTarget.InnerXml.Trim()[0]))
                    {
                        eleAltTransTarget.InnerXml = char.ToUpper(eleAltTransTarget.InnerXml.Trim()[0]) + eleAltTransTarget.InnerXml.Trim().Substring(1);
                    }
                }

                // Add <source> element to <alt-trans> in case of segmented content.
                if (eleAltTrans.Attributes["mid"] != null)
                {
                    string srcLang = "";
                    if (eleSource != null)
                    {
                        if (eleSource.Attributes["xml:lang"] != null)
                        {
                            srcLang = eleSource.Attributes["xml:lang"].Value;
                        }
                    }
                    XmlNode eleSegSourceMrk = eleAltTrans.ParentNode.SelectSingleNode("xlf:seg-source/xlf:mrk[@mid='" + eleAltTrans.Attributes["mid"].Value + "']", nsmgr);
                    if (eleSegSourceMrk != null)
                    {
                        XmlNode eleAltTransSource = xmlDoc.CreateElement("source",xlf_namespace);
                        eleAltTransSource.InnerXml = eleSegSourceMrk.InnerXml.Replace(" xmlns=\"" + xlf_namespace + "\"", "");
                        if (srcLang != "")
                        {
                            XmlAttribute atrLang = xmlDoc.CreateAttribute("xml:lang");
                            atrLang.Value = srcLang;
                            eleAltTransSource.Attributes.Append(atrLang);
                        }
                        eleAltTrans.PrependChild(eleAltTransSource);
                    }
                }
            }

            // Restore mtype value for <mrk> to "protected"
            foreach (XmlNode eleMrk in xmlDoc.SelectNodes("//xlf:mrk[@mtype='x-dnt' or @mtype='x-DNT']", nsmgr))
            {
                eleMrk.Attributes["mtype"].Value = "protected";
            }
            
            // Save XLIFF
            xmlDoc.Save(xliff_file_path);

        }

        /* ************************************************************************************* */
        /* ************************************************************************************* */

    }
}
