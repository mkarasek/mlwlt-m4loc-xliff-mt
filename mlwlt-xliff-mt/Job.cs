using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;

namespace mlwlt_xliff_mt
{
    /// <summary>
    ///     This class is driving a workflow of XLIFF MT pocessing.
    /// </summary>
    public class Job
    {
        private byte[] _InputFile;
        private string _InputFileName;
        private string _RepoRoot;
        private string _RepoFolder;
        private string _ConfigurationFilePath;
        private string _TikalJARfile;
        private string _file_Log;

        private string _file_Original;  // File in format as in came to the service
        private string _file_XLIFF;    // File in XLIFF format (extracted from BASE64)


        /* ************************************************************************************* */
        /// <summary>
        ///     Constructor creates a folder in the repository (defined by RepoRoot parameter) and put
        ///     the input file in the folder.
        /// </summary>
        /// <param name="InputFileName">Name of the input file</param>
        /// <param name="InputFile">Binary code of the input file</param>
        /// <param name="RepoRoot">Root path of the repository</param>
        /// <param name="ConfigurationFilePath">Path to the repository XML file</param>
        /// <param name="TikalJARfile">Path to "tikal.jar" file</param>
        public Job(string InputFileName, byte[] InputFile, string RepoRoot, string ConfigurationFilePath, string TikalJARfile)
        {
            _InputFile = InputFile;
            _InputFileName = InputFileName;
            _RepoRoot = NormalizePath(RepoRoot);
            _ConfigurationFilePath = ConfigurationFilePath;
            _TikalJARfile = TikalJARfile;
            _RepoFolder = _RepoRoot + "\\" + DateTime.Now.ToString("yyyy-MM-dd HH.mm.ss.fff");
            _file_Original = _RepoFolder + "\\" + _InputFileName + ".bin";
            _file_XLIFF = _RepoFolder + "\\" + _InputFileName + ".xlf";
            _file_Log = _RepoFolder + "\\log.xml";
        }

        /* ************************************************************************************* */
        /// <summary>
        ///     This function is called to start process the job workflow.
        /// </summary>
        /// <returns>Output file in the same format as the input file.</returns>
        public byte[] Process()
        {
            // Check root directory (exists?)
            System.IO.DirectoryInfo diRoot = new System.IO.DirectoryInfo(_RepoRoot);
            if (!diRoot.Exists)
            {
                throw new System.InvalidOperationException("Repository Root folder doesn't exist.");
            }

            // Create a job folder (named by actual date)
            try
            {
                System.IO.Directory.CreateDirectory(_RepoFolder);
            }
            catch (Exception)
            {
                throw new InvalidOperationException(string.Format("Can't create a job folder in the repository ({0}).", _RepoFolder));
            }

            // Create a log file
            Log mlwltLog = new Log(_file_Log);
            mlwltLog.create_new_log();

            // Store the input XLIFF file to the disk
            MemoryStream ms = new MemoryStream(_InputFile);
            FileStream fs = new FileStream(_file_Original, FileMode.Create);
            ms.WriteTo(fs);
            ms.Close();
            fs.Close();
            fs.Dispose();
            mlwltLog.save_to_log("Input File Upload", "All", String.Format("Input file uploaded ({0})", _InputFileName));

            //Detect input file format (XLIFF/BASE64/in <content> element)
            string Upload_Type = DetectInputFormat(_file_Original, _file_XLIFF);
            mlwltLog.save_to_log("Input File Format", "All", String.Format("Input file format detected ({0})", Upload_Type));
            if (Upload_Type == "")
            {
                throw new InvalidOperationException("Unknown input file format.");
            }

            //Provenance class
            Provenance mlwltProvenance = new Provenance();
            //Translate category class
            Translate mlwltTranslate = new Translate();
            //TextAnalysis category class
            TextAnalysis mlwltTextAnalysis = new TextAnalysis();
            //Machine Translation class
            MT mlwltMT = new MT();

            // Detect a new provenance id
            string provenanceID = mlwltProvenance.get_provenance_record_id(_file_XLIFF);
            mlwltLog.save_to_log("Provenance", "All", "Created a new provenance id: " + provenanceID);

            //Detect domains in the input file and make a copy for each of them
            Domain mlwltDomain = new Domain(_file_XLIFF, _ConfigurationFilePath, _file_Log);
            List<Domain.DomainEntry> deList = mlwltDomain.mapped_domain_list;
            mlwltDomain.Split();

            foreach (mlwlt_xliff_mt.Domain.DomainEntry de in deList)
            {
                //Convert domain XLIFF files to In-Line Text file format (using Tikal)
                M4Loc mlwltM4Loc = new M4Loc(_TikalJARfile);
                mlwltM4Loc.prepare_xliff_for_moses(de.FileName_xliff_source);
                mlwltM4Loc.convert_xliff_to_inline_text(de.FileName_xliff_source);

                //Detect and mark non-translateable strings in In-Line Text files (its-translate category)
                mlwltTranslate.process_translate_in_inLine_text(de.FileName_inlineText, 
                    de.FileName_inlineText_itsTranslate);

                //Detect and mark Text Analysis strings in In-Line Text files (its-textanalysis category)
                mlwltTextAnalysis.process_textanalysis_in_inLine_text(de.FileName_inlineText_itsTranslate, 
                    de.FileName_inlineText_mtAdapted, de.Language_Target);

                //Machine Translation on each In-Line Text file
                mlwltMT.process_mt_on_inLine_text(de.FileName_inlineText_mtAdapted, de.FileName_inlineText_mtApplied, 
                    de.MTengine_Url, de.MTengine_Port);

                //Convert In-Line Text Files back to domain XLIFF files
                mlwltM4Loc.convert_inline_to_xliff_text(
                    de.FileName_xliff_source, 
                    de.FileName_inlineText_mtApplied, 
                    de.FileName_xliff_translated);
                mlwltM4Loc.fix_xliff_after_moses(de.FileName_xliff_translated, provenanceID);
            }

            //Merge XLIFF files back into an original XLIFF file
            mlwltDomain.Merge(_file_XLIFF + ".output.xlf");

            //Insert a provenance information into a merged file
            mlwltProvenance.insert_provenance_records(_file_XLIFF + ".output.xlf", provenanceID);

            //Convert and correct an output file (BASE64/<content> element) 
            String finalOutputFile = "";
            if (Upload_Type == "XLFc" || Upload_Type == "B64" || Upload_Type == "B64c")
            {
                finalOutputFile = Process_Output_Base64(_file_XLIFF + ".output.xlf", Upload_Type);
            }
            if (Upload_Type == "XLF")
            {
                finalOutputFile = _file_XLIFF + ".output.xlf";
            }
            if (finalOutputFile != "")
            {
                return File.ReadAllBytes(finalOutputFile);
            }
            else
            {
                return null;
            }

        }


        /* ************************************************************************************* */
        // Returns a input file type (string): XLF|XLFc|B64|B64c
        private string DetectInputFormat(String originalPath, String XLIFFPath)
        {
            String Result = "";
            //Detect whether the file is XML(XLIFF) or not
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.Load(originalPath);
                Result = "XLF";
            }
            catch (Exception)
            {
                //No XML on input, try Base64
                string encodedText = System.IO.File.ReadAllText(originalPath);
                byte[] decodedBytes = Convert.FromBase64String(encodedText);
                string decodedText = Encoding.UTF8.GetString(decodedBytes);
                try
                {
                    xmlDoc.LoadXml(decodedText);
                    Result = "B64";
                }
                catch (Exception)
                {
                    Result = "";
                }
            }

            if (Result != "")
            {
                if (xmlDoc.DocumentElement.Name == "content")
                {
                    if (xmlDoc.DocumentElement.ChildNodes.Count == 1)
                    {
                        if (xmlDoc.DocumentElement.ChildNodes[0].Name == "xliff")
                        {
                            System.Xml.XmlDocument newDoc = new System.Xml.XmlDocument();
                            newDoc.LoadXml(xmlDoc.DocumentElement.InnerXml);
                            newDoc.Save(XLIFFPath);
                            Result = Result + "c";
                        }
                        else
                        {
                            Result = "";
                        }
                    }
                    else
                    {
                        Result = "";
                    }
                }
                else if (xmlDoc.DocumentElement.Name == "xliff")
                {
                    if (Result == "XLF")
                    {
                        FileInfo fi = new FileInfo(originalPath);
                        fi.CopyTo(XLIFFPath);
                    }
                    else
                    {
                        xmlDoc.Save(XLIFFPath);
                    }
                }
                else
                {
                    Result = "";
                }
            }
            return Result;
        }


        /* ************************************************************************************* */
        private String Process_Output_Base64(String newPath, String Type)
        {
            String Result = newPath + ".bin";

            byte[] bytesToEncode;
            string encodedText;

            System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
            xmlDoc.Load(newPath);

            switch (Type)
            {
                case "XLFc":
                case "B64c":
                    System.Xml.XmlDocument newDoc = new System.Xml.XmlDocument();
                    newDoc.LoadXml("<content></content>");
                    newDoc.DocumentElement.InnerXml = xmlDoc.OuterXml;
                    if (Type == "XLFc")
                    {
                        newDoc.Save(Result);
                    }
                    else
                    {
                        bytesToEncode = Encoding.UTF8.GetBytes(newDoc.OuterXml);
                        encodedText = Convert.ToBase64String(bytesToEncode);
                        System.IO.File.WriteAllText(Result, encodedText);
                    }
                    break;
                case "B64":
                    bytesToEncode = Encoding.UTF8.GetBytes(xmlDoc.OuterXml);
                    encodedText = Convert.ToBase64String(bytesToEncode);
                    System.IO.File.WriteAllText(Result, encodedText);
                    break;
                default:
                    break;
            }
            return Result;
        }


        /* ************************************************************************************* */
        // <summary>
        //     File path normalization (removes back-slash at the end)
        // </summary>
        // <param name="Path">File path to normalize</param>
        // <returns>Normalized file path (with no back-slash at the end)</returns>
        private string NormalizePath(string Path)
        {
            if (Path.EndsWith("\\") && Path.Length > 1)
            {
                return Path.Remove(Path.Length - 1);
            }
            else
            {
                return Path;
            }
        }


        /* ************************************************************************************* */
        /* ************************************************************************************* */

    }
}
