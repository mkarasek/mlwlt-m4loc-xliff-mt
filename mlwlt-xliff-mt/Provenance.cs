using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Text.RegularExpressions;

namespace mlwlt_xliff_mt
{
    public class Provenance
    {

        const string okp_namespace = "okapi-framework:xliff-extensions";
        const string xlf_namespace = "urn:oasis:names:tc:xliff:document:1.2";
        const string its_namespace = "http://www.w3.org/2005/11/its";
        const string itsx_namespace = "http://www.w3.org/2005/11/itsx";

        /* ************************************************************************************* */

        public Provenance () {}

        /* ************************************************************************************* */
        /// <summary>
        ///     Inserts a provenance records to the header of the XLIFF file
        /// </summary>
        /// <param name="xliff_input_path">Path to the XLIFF file</param>
        /// <param name="newID">A new ID for current provenance record</param>
        public void insert_provenance_records(string xliff_input_path, string newID)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xliff_input_path);
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
            nsmgr.AddNamespace("okp", okp_namespace);
            nsmgr.AddNamespace("xlf", xlf_namespace);
            nsmgr.AddNamespace("its", its_namespace);
            nsmgr.AddNamespace("itsx", itsx_namespace);

            // Construct a new provenance record
            XmlNode eleNewProvenanceRecords = xmlDoc.CreateElement("its:provenanceRecords", its_namespace);
            XmlAttribute atr;
            atr = xmlDoc.CreateAttribute("xml:id");
            atr.Value = newID;
            eleNewProvenanceRecords.Attributes.Append(atr);
            XmlNode eleNewProvenanceRecord = xmlDoc.CreateElement("its:provenanceRecord", its_namespace);
            atr = xmlDoc.CreateAttribute("its:tool", its_namespace);
            atr.Value = "mosesmt";
            eleNewProvenanceRecord.Attributes.Append(atr);
            atr = xmlDoc.CreateAttribute("its:orgRef", its_namespace);
            atr.Value = "http://www.moravia.com";
            eleNewProvenanceRecord.Attributes.Append(atr);
            atr = xmlDoc.CreateAttribute("its:provRef", its_namespace);
            atr.Value = "";
            eleNewProvenanceRecord.Attributes.Append(atr);
            
            eleNewProvenanceRecords.AppendChild(eleNewProvenanceRecord);

            // Insert Provenance records into the XLIFF file (for each <file>). 
            // In case there are already provenance records available, the new one is added right after the latest one.
            foreach (XmlNode eleFile in xmlDoc.SelectNodes("//xlf:file", nsmgr))
            {
                // Missing XLIFF file header
                XmlNode eleHeader = eleFile.SelectSingleNode("xlf:header", nsmgr);
                if (eleHeader == null)
                {
                    eleHeader = xmlDoc.CreateElement("header", xlf_namespace);
                    eleFile.PrependChild(eleHeader);
                }

                XmlNodeList ProvenanceRecords = eleHeader.SelectNodes("its:provenanceRecords", nsmgr);
                if (ProvenanceRecords.Count > 0)
                {
                    eleHeader.InsertAfter(eleNewProvenanceRecords.Clone(), ProvenanceRecords[ProvenanceRecords.Count - 1]);
                }
                else
                {
                    eleHeader.PrependChild(eleNewProvenanceRecords.Clone());
                }

            }
            xmlDoc.Save(xliff_input_path);
        }

        /* ************************************************************************************* */
        /// <summary>
        ///     Gets a new provenance record id (looking for a max ID in the provenance records and 
        ///     incremented by 1)
        /// </summary>
        /// <param name="xliff_input_path">Path to the XLIFF file</param>
        /// <returns>A new provenance ID</returns>
        public string get_provenance_record_id(string xliff_input_path)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xliff_input_path);
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
            nsmgr.AddNamespace("okp", okp_namespace);
            nsmgr.AddNamespace("xlf", xlf_namespace);
            nsmgr.AddNamespace("its", its_namespace);
            nsmgr.AddNamespace("itsx", itsx_namespace);

            string strID = "";
            int maxID = -1;
            string maxIDstring = "";
            int currentID = 0;

            foreach (XmlNode eleProvRecords in xmlDoc.SelectNodes("//its:provenanceRecords[@xml:id]", nsmgr))
            {
                strID = eleProvRecords.Attributes["xml:id"].Value;
                currentID = get_last_number_from_id(strID);
                if (currentID > maxID) { maxID = currentID; maxIDstring = strID; }
            }
            if (maxID >= 0)
            {
                return maxIDstring.Replace(maxID.ToString(), (maxID + 1).ToString());
            }
            else
            {
                return maxIDstring + "1";
            }
        }


        /* ************************************************************************************* */

        private int get_last_number_from_id(string id)
        {
            Match m = Regex.Match(id, "\\d+", RegexOptions.RightToLeft);
            if (!String.IsNullOrEmpty(m.Value))
            {
                return Int32.Parse(m.Value);
            }
            else
            {
                return 0;
            }
        }

        /* ************************************************************************************* */

    }
}
