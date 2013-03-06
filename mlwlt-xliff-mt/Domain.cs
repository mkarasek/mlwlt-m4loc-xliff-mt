using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

namespace mlwlt_xliff_mt
{
    /// <summary>
    ///     Domain class holds functions and mechanisms supporting its-domain category mapped in the XLIFF file.
    ///     It also implements re-mapping such domains into MT domains installed at service side (based on configuration.xml file).
    /// </summary>
    public class Domain
    {

        /* ************************************************************************************* */
        public struct DomainEntry
        {
            public List<string> ITSdomains;
            public string DomainName;
            public string FileName_xliff_source;              // SampleFile.xlf.Classical Studies.xlf
            public string FileName_inlineText;                  //  SampleFile.xlf.Classical Studies.xlf.english
            public string FileName_inlineText_itsTranslate;    // SampleFile.xlf.Classical Studies.xlf.english.itsTranslate
            public string FileName_inlineText_mtAdapted;     // SampleFile.xlf.Classical Studies.xlf.english.mtAdapted
            public string FileName_inlineText_mtApplied;    // SampleFile.xlf.Classical Studies.xlf.english.mtApplied
            public string FileName_inlineText_mtPostprocess;    // SampleFile.xlf.Classical Studies.xlf.english.mtPost
            public string FileName_xliff_translated;                 // SampleFile.xlf.Classical Studies.xlf.english.output.xlf
            public string Language_Source;
            public string Language_Target;
            public string MTengine_Url;
            public string MTengine_Port;
        }

        /* ************************************************************************************* */
        const string default_domain_name = "Default";
        const string okp_namespace = "okapi-framework:xliff-extensions";
        const string xlf_namespace = "urn:oasis:names:tc:xliff:document:1.2";
        const string its_namespace = "http://www.w3.org/2005/11/its";
        const string itsx_namespace = "http://www.w3.org/2005/11/itsx";

        public List<DomainEntry> mapped_domain_list = null;

        /* ************************************************************************************* */
        private string _configuration_file_path = "";
        private string _xliff_input_path = "";

        /* ************************************************************************************* */
        private string _log_file_path = "";
        Log mlwltLog;

        /* ************************************************************************************* */
        /// <summary>
        ///     Domain class takes care of domains in the XLIFF file, their splitting into domain XLIFFs and merging back
        ///     to the final XLIFF.
        /// </summary>
        /// <param name="xliff_input_path">Path to the original XLIFF file.</param>
        /// <param name="configuration_file_path">Path to the configuration (containing domain mapping information).</param>
        public Domain(string xliff_input_path, string configuration_file_path, string log_file_path)
        {
            _xliff_input_path = xliff_input_path;
            _configuration_file_path = configuration_file_path;
            _log_file_path = log_file_path;
            mlwltLog = new Log(_log_file_path);
            mapped_domain_list = get_domain_list_mapped();
        }

        /* ************************************************************************************* */
        /// <summary>
        ///     Splits the original XLIFF file into pieces, based on domains implemented by the service 
        ///     (not by its-domains).
        /// </summary>
        public void Split()
        {
            foreach (DomainEntry de in mapped_domain_list)
            {
                split_separate_one_domain(de);
                mlwltLog.save_to_log("XLIFF Split", de.DomainName, "XLIFF has been splitted for given domain.");
            }
        }

        /* ************************************************************************************* */
        /// <summary>
        ///     Splitted XLIFF files are merged back into one XLIFF (same XLIFF structure as on input).
        /// </summary>
        public void Merge(string xliff_output_path)
        {
            // Copy original XLIFF as a template for an output file
            System.IO.File.Copy(_xliff_input_path, xliff_output_path, true);
            // Get target language of the original XLIFF file
            Language mlwltLanguage = new Language();
            string target_language = mlwltLanguage.get_xliff_target_language(_xliff_input_path);
            // Merge the output domain XLIFF file with the template (which is copied original XLIFF file)
            foreach (DomainEntry de in mapped_domain_list)
            {
                process_domain_merge(xliff_output_path, de.FileName_xliff_translated, target_language);
                mlwltLog.save_to_log("XLIFF Merge", de.DomainName, "Domain XLIFF has been merged to the original XLIFF file.");
            }
        }

        /* ************************************************************************************* */
        /// <summary>
        ///     Analyses input XLIFF file and looking for all its domains used in it.
        /// </summary>
        /// <returns>List of strings containing its domains used in the input XLIFF file.</returns>
        private List<string> get_list_of_its_domains_in_XLIFF()
        {
            List<string> result = new List<string>();
            result.Add(default_domain_name.Trim().ToUpper());
            XmlDocument xliffDoc = new XmlDocument();
            xliffDoc.Load(_xliff_input_path);
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(xliffDoc.NameTable);
            nsmgr.AddNamespace("okp", okp_namespace);
            nsmgr.AddNamespace("xlf", xlf_namespace);
            nsmgr.AddNamespace("its", its_namespace);
            nsmgr.AddNamespace("itsx", itsx_namespace);

            foreach (XmlNode xmlNod in xliffDoc.SelectNodes("//*/@okp:itsDomain | //*/@its:domain | //*/@itsx:domain", nsmgr))
            {
                if (result.IndexOf(xmlNod.Value.Trim().ToUpper()) < 0)
                {
                    result.Add(xmlNod.Value.Trim().ToUpper());
                }
            }

            return result;
        }

        /* ************************************************************************************* */
        /// <summary>
        ///     Analyses input XLIFF file and looking for all its domains used in it. Trying to find a general domain 
        ///     implemented on the service. As an output, all identified domains are matched with implemented ones
        ///     together with sugested file name for XLIFF file splitting at the further phase of the workflow.
        /// </summary>
        /// <returns>
        ///     List of strictured object, containing its-domain name, matched implemented domain and sugested
        ///     path of the XLIFF file (for splitting purposes).
        /// </returns>
        private List<DomainEntry> get_domain_list_mapped()
        {
            List<DomainEntry> result = new List<DomainEntry>();

            // Get the list of its domains as they are present in the original XLIFF file
            List<string> its_domains_in_XLIFF = get_list_of_its_domains_in_XLIFF();

            // Get the name of source and target languages in the source XLIFF file (for constructing file name purposes)
            Language mlwltLanguage = new mlwlt_xliff_mt.Language();
            string srcLang = mlwltLanguage.get_xliff_source_language(_xliff_input_path).ToLower();
            string tarLang = mlwltLanguage.get_xliff_target_language(_xliff_input_path).ToLower();
            // Get source and target language names (standard names used in configuration.xml and in language class)
            string detected_source_language = mlwltLanguage.get_language_name(srcLang);
            string detected_target_language = mlwltLanguage.get_language_name(tarLang);
            string mtEngine_Url = "";
            string mtEngine_Port = "";

            foreach (string its_domain in its_domains_in_XLIFF)
            {
                string domain_name = get_mapped_domain_from_configuration(its_domain,
                    detected_source_language, detected_target_language, out mtEngine_Url, out mtEngine_Port);
                DomainEntry foundItem = result.Find(de => de.DomainName == domain_name);

                if (foundItem.DomainName != null)
                {
                    foundItem.ITSdomains.Add(its_domain);
                }
                else
                {
                    DomainEntry de = new DomainEntry();
                    de.ITSdomains = new List<string>();
                    de.ITSdomains.Add(its_domain);
                    de.DomainName = domain_name;
                    de.FileName_xliff_source = _xliff_input_path + "." + domain_name.Replace(" ","_") + ".xlf";
                    de.FileName_inlineText = de.FileName_xliff_source + "." + srcLang;
                    de.FileName_inlineText_itsTranslate = de.FileName_xliff_source + "." + srcLang + ".itsTranslate";
                    de.FileName_inlineText_mtAdapted = de.FileName_xliff_source + "." + srcLang + ".mtAdapted";
                    de.FileName_inlineText_mtApplied = de.FileName_xliff_source + "." + srcLang + ".mtApplied";
                    de.FileName_inlineText_mtPostprocess = de.FileName_xliff_source + "." + srcLang + ".mtPostprocess";
                    de.FileName_xliff_translated = de.FileName_xliff_source + "." + srcLang + ".output.xlf";
                    de.Language_Source = detected_source_language;
                    de.Language_Target = detected_target_language;
                    de.MTengine_Url = mtEngine_Url;
                    de.MTengine_Port = mtEngine_Port;
                    result.Add(de);
                    // Log entry
                    mlwltLog.save_to_log("Domain Mapping Detection", de.DomainName, "Detected domain for ITS domain name: " + its_domain);
                }
            }
            return result;
        }


        /* ************************************************************************************* */
        private string get_mapped_domain_from_configuration(string its_domain, 
            string source_language, string target_language, out string mt_engine_url, out string mt_engine_port)
        {
            // Load configuration file (with domain mapping information) into the memory
            XmlDocument xmlMap = new XmlDocument();
            xmlMap.Load(_configuration_file_path);

            // Get the <language-pair> element based on srcLang and tarLang parameters
            XmlElement eleLanguagePair = (XmlElement)xmlMap.SelectSingleNode(String.Format(
                "/configuration/domain-mapping/language-pair[@source-language='{0}' and @target-language='{1}']",
                source_language, target_language));

            XmlElement eleMTengineUrl = null;
            XmlElement eleMTenginePort = null;
            mt_engine_url = "";
            mt_engine_port = "";

            // Test whether the language pair is defined in the configuration file
            if (eleLanguagePair != null)
            {
                // Browse all domains defined for given language pair and look for its-domain existence
                foreach (XmlElement eleDomain in eleLanguagePair.SelectNodes("domain"))
                {
                    foreach (XmlElement eleITSDomain in eleDomain.SelectNodes("its-domains/its-domain"))
                    {
                        // In case its-domain is presend for given domain, domain name is returned
                        // together with information about MT engines
                        if (eleITSDomain.InnerText.Trim().ToUpper() == its_domain)
                        {
                            eleMTengineUrl = (XmlElement)eleDomain.SelectSingleNode("mt-engine/mt-engine-url");
                            eleMTenginePort = (XmlElement)eleDomain.SelectSingleNode("mt-engine/mt-engine-port");
                            if (eleMTengineUrl != null) { mt_engine_url = eleMTengineUrl.InnerText; }
                            if (eleMTenginePort != null) { mt_engine_port = eleMTenginePort.InnerText;  }
                            return eleDomain.Attributes["name"].Value;
                        }
                    }
                }
                // In case its-domain is not present in the configuration, return the default domain name
                // for given language pair
                if (eleLanguagePair.Attributes["default-domain"] != null)
                {
                    eleMTengineUrl = (XmlElement)eleLanguagePair.SelectSingleNode(
                        String.Format("domain[@name='{0}']/mt-engine/mt-engine-url",
                        eleLanguagePair.Attributes["default-domain"].Value));
                    eleMTenginePort = (XmlElement)eleLanguagePair.SelectSingleNode(
                        String.Format("domain[@name='{0}']/mt-engine/mt-engine-port",
                        eleLanguagePair.Attributes["default-domain"].Value));
                    if (eleMTengineUrl != null) { mt_engine_url = eleMTengineUrl.InnerText; }
                    if (eleMTenginePort != null) { mt_engine_port = eleMTenginePort.InnerText; }
                    return eleLanguagePair.Attributes["default-domain"].Value;
                }
            }
            // Selected <language-pair> doesn't exist or default domain is not defined
            return "";
        }


        /* ************************************************************************************* */
        /// <summary>
        ///     Splitting XLIFF file into domain XLIFF files.
        /// </summary>
        /// <param name="domain">
        ///     Domain Entry specifying domain name as well as mapped its domains 
        ///     and output XLIFF path
        /// </param>
        private void split_separate_one_domain(DomainEntry domain)
        {
            // Copy original XLIFF into a domain XLIFF file
            System.IO.File.Copy(_xliff_input_path, domain.FileName_xliff_source, true);

            XmlDocument xliffDoc = new XmlDocument();
            xliffDoc.Load(domain.FileName_xliff_source);

            XmlNamespaceManager nsmgr = new XmlNamespaceManager(xliffDoc.NameTable);
            nsmgr.AddNamespace("okp", okp_namespace);
            nsmgr.AddNamespace("xlf", xlf_namespace);
            nsmgr.AddNamespace("its", its_namespace);
            nsmgr.AddNamespace("itsx", itsx_namespace);

            foreach (XmlNode xmlNod in xliffDoc.SelectNodes("//xlf:trans-unit", nsmgr))
            {
                if (!is_node_in_its_domain(xmlNod, domain.ITSdomains))
                {
                    xmlNod.ParentNode.RemoveChild(xmlNod);
                }
            }
            xliffDoc.Save(domain.FileName_xliff_source);
        }

        /* ************************************************************************************* */

        private bool is_node_in_its_domain(XmlNode xmlNod, List<string> its_domains)
        {
            XmlNode atr = xmlNod.Attributes.GetNamedItem("itsDomain", "okapi-framework:xliff-extensions");
            if (atr == null) { atr = xmlNod.Attributes.GetNamedItem("domain", "http://www.w3.org/2005/11/its"); }

            if (atr != null)
            {
                if (its_domains.FindIndex(s => s.Equals(atr.Value.Trim(), StringComparison.OrdinalIgnoreCase)) != -1) { return true; }
                else { return false; }
            }
            else
            {
                if (xmlNod.Name != "file") { return is_node_in_its_domain(xmlNod.ParentNode, its_domains); }
                else { return (its_domains.FindIndex(s => s.Equals(default_domain_name, StringComparison.OrdinalIgnoreCase)) != -1); }
            }
        }

        /* ************************************************************************************* */
        private String process_domain_merge(String xliff_original_input_path, String xliff_domain_input_path, String target_language)
        {
            String result = "";

            XmlDocument xmlOrig = new XmlDocument();
            xmlOrig.Load(xliff_original_input_path);

            XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlOrig.NameTable);
            nsmgr.AddNamespace("okp", okp_namespace);
            nsmgr.AddNamespace("xlf", xlf_namespace);
            nsmgr.AddNamespace("its", its_namespace);
            nsmgr.AddNamespace("itsx", itsx_namespace);

            // Add an its: namespace to the document element
            XmlAttribute ns = xmlOrig.CreateAttribute("xmlns", "its", "http://www.w3.org/2000/xmlns/");
            ns.Value = its_namespace;
            xmlOrig.DocumentElement.Attributes.Append(ns);

            XmlDocument xmlDomain = new XmlDocument();
            xmlDomain.Load(xliff_domain_input_path);

            String fileOriginal = "";
            String tuID = "";

            XmlNode whereToCopy;

            foreach (XmlNode eleFile in xmlDomain.SelectNodes("//xlf:file", nsmgr))
            {
                fileOriginal = eleFile.Attributes["original"].Value;

                foreach (XmlNode eleTU in eleFile.SelectNodes("xlf:body/xlf:trans-unit", nsmgr))
                {
                    tuID = eleTU.Attributes["id"].Value;
                    whereToCopy = xmlOrig.SelectSingleNode("//xlf:file[@original=\"" + fileOriginal + "\"]/xlf:body/xlf:trans-unit[@id=\"" + tuID + "\"]", nsmgr);
                    foreach (XmlNode whatToCopy in eleTU.SelectNodes("xlf:alt-trans", nsmgr))
                    {
                        if (whatToCopy != null)
                        {
                            XmlNode copiedNode = xmlOrig.ImportNode(whatToCopy, true);
                            whereToCopy.AppendChild(copiedNode);
                        }
                    }
                }
            }

            xmlOrig.Save(xliff_original_input_path);

            return result;
        }

        /* ************************************************************************************* */


    }
}
