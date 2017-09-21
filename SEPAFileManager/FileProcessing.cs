using Abacus.BusinessRules.CU.Accounts;
using Abacus.BusinessRules.CU.SEPA;
using Abacus.DataSets;
using Abacus.SEPA;
using Abacus.SEPA.DataSets;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace SEPAFileManager
{
    class FileProcessing
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly ILog email = LogManager.GetLogger("EmailAlerter");
        //Generate Read & Post
        #region Process Downloaded files
        public void ReadFiles()
        {
            try
            {
                string[] fileList = System.IO.Directory.GetFiles(Settings.DownloadFolder, "*", SearchOption.TopDirectoryOnly);
                string Type = string.Empty;
                string FileType = string.Empty;
                string ProcessType = "SCT";
                string FileRef = string.Empty;
                string OrigFRef = string.Empty;
                string FileRjctRsn = string.Empty;
                bool SCF = false;
                bool CVF = false;
                bool DVF = false;
                bool DNF = false;
                bool RSF = false;

                foreach (string file in fileList)
                {
                    string[] Extension = file.Split('.');

                    // Don't do ACKs here
                    if (Extension[Extension.Length - 1] == "ACK")
                        continue;
                    if (Extension[Extension.Length - 1] == "NAK")
                        continue;

                    FileInfo f = new FileInfo(file);
                    if (f.Length == 0)
                    {
                        // Move empty file.
                        string[] moveaddr = file.Split('\\');
                        File.Move(file, string.Concat(Settings.DownloadFolder, "Archive\\", moveaddr[moveaddr.Length - 1], ".old"));
                        log.Info(DateTime.Now + ", READFILES - " + moveaddr[moveaddr.Length - 1] + " Empty file moved to: " + string.Concat(Settings.DownloadFolder, "Archive\\"));
                        continue;
                    }

                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(file);
                    XmlNode node = xmlDoc.FirstChild;
                    
                    DateTime FileBusDt = DateTime.Now;
                    DateTime ReceivedDate = File.GetCreationTime(file);

                    foreach (XmlNode Node in xmlDoc.ChildNodes)
                    {
                        if (Node.Name == "S2SCTScf:SCTScfBlkCredTrf")
                        {
                            // SCT
                            foreach (XmlNode Node1 in Node.ChildNodes)
                            {
                                if (Node1.Name == "S2SCTScf:FType")
                                    FileType = Node1.InnerText;
                                else if (Node1.Name == "S2SCTScf:FileRef")
                                    FileRef = Node1.InnerText;
                                else if (Node1.Name == "S2SCTScf:FileBusDt")
                                    FileBusDt = Convert.ToDateTime(Node1.InnerText);
                                else if (Node1.Name == "S2SCTScf:OrigFRef")
                                    OrigFRef = Node1.InnerText;
                            }

                            SCF = true;
                        }
                        else if (Node.Name == "S2SCTCcf:SCTCcfBlkCredTrf")
                        {
                            // SCT - CCF
                            foreach (XmlNode Node1 in Node.ChildNodes)
                            {
                                if (Node1.Name == "S2SCTCcf:FType")
                                    FileType = Node1.InnerText;
                                else if (Node1.Name == "S2SCTCcf:FileRef")
                                    FileRef = Node1.InnerText;
                                else if (Node1.Name == "S2SCTCcf:FileBusDt")
                                    FileBusDt = Convert.ToDateTime(Node1.InnerText);
                                else if (Node1.Name == "S2SCTCcf:OrigFRef")
                                    OrigFRef = Node1.InnerText;
                            }
                        }
                        else if (Node.Name == "S2SCTCvf:SCTCvfBlkCredTrf")
                        {
                            // SCT - CVF
                            foreach (XmlNode Node1 in Node.ChildNodes)
                            {
                                if (Node1.Name == "S2SCTCvf:FType")
                                    FileType = Node1.InnerText;
                                else if (Node1.Name == "S2SCTCvf:FileRef")
                                    FileRef = Node1.InnerText;
                                else if (Node1.Name == "S2SCTCvf:FileBusDt")
                                    FileBusDt = Convert.ToDateTime(Node1.InnerText);
                                else if (Node1.Name == "S2SCTCvf:OrigFRef")
                                    OrigFRef = Node1.InnerText;
                                else if (Node1.Name == "S2SCTCvf:FileRjctRsn")
                                    FileRjctRsn = Node1.InnerText;
                            }
                            if (FileRjctRsn == "A00") //If file is accepted, trigger posting it
                                CVF = true;
                        }
                        else if (Node.Name == "S2SDDDnf:MPEDDDnfBlkDirDeb")
                        {
                            // SDD
                            foreach (XmlNode Node1 in Node.ChildNodes)
                            {
                                if (Node1.Name == "S2SDDDnf:FType")
                                    FileType = Node1.InnerText;
                                else if (Node1.Name == "S2SDDDnf:FileRef")
                                    FileRef = Node1.InnerText;
                                else if (Node1.Name == "S2SDDDnf:FileBusDt")
                                    FileBusDt = Convert.ToDateTime(Node1.InnerText);
                                else if (Node1.Name == "S2SDDDnf:OrigFRef")
                                    OrigFRef = Node1.InnerText;
                            }
                            DNF = true;
                            ProcessType = "SDD";
                        }
                        else if (Node.Name == "S2SDDDvf:MPEDDDvfBlkDirDeb")
                        {
                            // SDD - DVF
                            foreach (XmlNode Node1 in Node.ChildNodes)
                            {
                                if (Node1.Name == "S2SDDDvf:FType")
                                    FileType = Node1.InnerText;
                                else if (Node1.Name == "S2SDDDvf:FileRef")
                                    FileRef = Node1.InnerText;
                                else if (Node1.Name == "S2SDDDvf:FileBusDt")
                                    FileBusDt = Convert.ToDateTime(Node1.InnerText);
                                else if (Node1.Name == "S2SDDDvf:OrigFRef")
                                    OrigFRef = Node1.InnerText;
                                else if (Node1.Name == "S2SDDDvf:FileRjctRsn")
                                    FileRjctRsn = Node1.InnerText;
                            }
                            DVF = true;
                            ProcessType = "SDD";
                        }
                        else if (Node.Name == "S2SDDRsf:MPEDDRsfBlkDirDeb")
                        {
                            // SDD - RSF
                            foreach (XmlNode Node1 in Node.ChildNodes)
                            {
                                if (Node1.Name == "S2SDDRsf:FType")
                                    FileType = Node1.InnerText;
                                else if (Node1.Name == "S2SDDRsf:FileRef")
                                    FileRef = Node1.InnerText;
                                else if (Node1.Name == "S2SDDRsf:FileBusDt")
                                    FileBusDt = Convert.ToDateTime(Node1.InnerText);
                                else if (Node1.Name == "S2SDDRsf:OrigFRef")
                                    OrigFRef = Node1.InnerText;
                            }
                            RSF = true;
                            ProcessType = "SDD";
                        }
                    }

                    StringWriter sw = new StringWriter();
                    XmlTextWriter tw = new XmlTextWriter(sw);
                    xmlDoc.WriteTo(tw);

                    string CommandString = @"SELECT @Returned = -1
                        INSERT INTO SEPA_Files (Type, FileType, FileRef, FileBusDt, ReceivedDate, [XML]) Values (@Type, @FileType, @FileRef, @FileBusDt, @ReceivedDate, @XML)
                        IF (@@ERROR = 0)
	                    SELECT @Returned = @@Identity";

                    System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand();

                    cmd.Parameters.Add("@Returned", System.Data.SqlDbType.BigInt);
                    cmd.Parameters.Add("@Type", System.Data.SqlDbType.NVarChar, 3);
                    cmd.Parameters.Add("@FileType", System.Data.SqlDbType.NVarChar, 3);
                    cmd.Parameters.Add("@FileRef", System.Data.SqlDbType.NVarChar, 16);
                    cmd.Parameters.Add("@FileBusDt", System.Data.SqlDbType.SmallDateTime);
                    cmd.Parameters.Add("@ReceivedDate", System.Data.SqlDbType.SmallDateTime);
                    cmd.Parameters.Add("@XML", System.Data.SqlDbType.NVarChar);

                    cmd.Parameters["@Returned"].Direction = System.Data.ParameterDirection.Output;
                    cmd.Parameters["@Type"].Value = ProcessType;
                    cmd.Parameters["@FileType"].Value = FileType;
                    cmd.Parameters["@FileRef"].Value = FileRef;
                    cmd.Parameters["@FileBusDt"].Value = FileBusDt;
                    cmd.Parameters["@ReceivedDate"].Value = ReceivedDate;
                    cmd.Parameters["@XML"].Value = sw.ToString();

                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = CommandString;

                    cmd.Connection = new System.Data.SqlClient.SqlConnection(Settings.Connection.ConnectionString);
                    cmd.Connection.Open();

                    try
                    {
                        cmd.ExecuteNonQuery();

                        long fileID = Convert.ToInt64(cmd.Parameters["@Returned"].Value);

                        string[] moveaddr = file.Split('\\');
                        log.Info(DateTime.Now + ", READFILE - " + moveaddr[moveaddr.Length - 1] + " has been added.");

                        File.Move(file, string.Concat(Settings.DownloadFolder, "Archive\\", moveaddr[moveaddr.Length - 1], ".old"));
                        log.Debug(string.Concat(DateTime.Now, ", READFILE - ", moveaddr[moveaddr.Length - 1], " moved to ", Settings.DownloadFolder, "Archive\\"));

                        email.Info(string.Concat(DateTime.Now, ", READFILE - File ", file, " read."));

                        if (FileType == "CVF")
                        {
                            PostCVF(fileID);
                        }


                    }
                    catch (Exception ee)
                    {
                        log.Error(string.Concat(DateTime.Now + ", READFILE ERROR, File Read Failed, " + ee.Message));
                        string[] moveaddr = file.Split('\\');
                        File.Move(file, string.Concat(Settings.DownloadFolder, "Archive\\", moveaddr[moveaddr.Length - 1], ".ERR"));
                        log.Error(string.Concat(DateTime.Now, ", ERROR READFILE - ", moveaddr[moveaddr.Length - 1], " moved to ", Settings.DownloadFolder, "Archive\\"));
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(string.Concat(DateTime.Now + ", READFILE ERROR - File Processing Failed, ", ex.Message));
                throw;
            }
        }        

        public void ReadACK()
        {
            try
            {
                string[] fileList = System.IO.Directory.GetFiles(Settings.DownloadFolder, "*.*", SearchOption.TopDirectoryOnly);

                foreach (string file in fileList)
                {
                    string[] Extension = file.Split('.');
                    // Ignore if not an ACK/NAK
                    if ((Extension[Extension.Length - 1] != "ACK") && (Extension[Extension.Length - 1] != "NAK"))
                        continue;

                    if (file.Split('.').Length > 3)
                        continue;

                    string fileContents = string.Empty;
                    using (StreamReader sr = new StreamReader(file, System.Text.Encoding.UTF8))
                    {
                        fileContents = sr.ReadToEnd();
                    }

                    string[] splitContents = fileContents.Split(';');

                    string FIL_NAME = splitContents[2];
                    string FIL_REF = splitContents[1];
                    string ACK = Path.GetExtension(file);
                    if (ACK.Length > 1)
                        ACK = ACK.Substring(1, ACK.Length - 1);
                    string FIL_REJ_CODE = splitContents[4]; // 1 for OK
                    string FIL_REJ_MSG = splitContents[3]; // OK or error of some desc.

                    string CommandString = @"UPDATE SEPA_CTLog SET ACK = @ACK, FIL_REJ_CODE = @FIL_REJ_CODE, FIL_REJ_MSG = @FIL_REJ_MSG WHERE FileName = @FIL_NAME";
                    if (FIL_NAME.EndsWith("QD1")) // SDD ACK
                        CommandString = @"UPDATE SEPA_DDLog SET ACK = @ACK, FIL_REJ_CODE = @FIL_REJ_CODE, FIL_REJ_MSG = @FIL_REJ_MSG WHERE FileName = @FIL_NAME";

                    System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand();

                    cmd.Parameters.Add("@ACK", System.Data.SqlDbType.NVarChar, 4);
                    cmd.Parameters.Add("@FIL_REJ_CODE", System.Data.SqlDbType.NVarChar);
                    cmd.Parameters.Add("@FIL_REJ_MSG", System.Data.SqlDbType.NVarChar);
                    cmd.Parameters.Add("@FIL_REF", System.Data.SqlDbType.NVarChar, 16);
                    cmd.Parameters.Add("@FIL_NAME", System.Data.SqlDbType.NVarChar);

                    cmd.Parameters["@ACK"].Value = ACK;
                    cmd.Parameters["@FIL_REJ_CODE"].Value = FIL_REJ_CODE;
                    cmd.Parameters["@FIL_REJ_MSG"].Value = FIL_REJ_MSG;
                    cmd.Parameters["@FIL_REF"].Value = FIL_REF;
                    cmd.Parameters["@FIL_NAME"].Value = FIL_NAME;

                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = CommandString;

                    cmd.Connection = new System.Data.SqlClient.SqlConnection(Settings.Connection.ConnectionString);
                    cmd.Connection.Open();

                    try
                    {
                        int RowsUpdated = cmd.ExecuteNonQuery();
                        string[] moveaddr = file.Split('\\');

                        if (RowsUpdated > 0)
                        {
                            if (ACK == "ACK")
                                log.Info(string.Format("{0}, {1}, {2}", DateTime.Now, FIL_NAME, ACK));
                            else
                                log.Info(string.Format("{0}, {1}, {2}, {3}, {4}", DateTime.Now, FIL_NAME, ACK, FIL_REJ_CODE, FIL_REJ_MSG));

                            log.Info(string.Concat(DateTime.Now, ", READACK - ", moveaddr[moveaddr.Length - 1], " has been added."));

                            File.Move(file, string.Concat(Settings.DownloadFolder, "ArchiveLogs\\", moveaddr[moveaddr.Length - 1], ".old"));
                            log.Info(string.Concat(DateTime.Now + ", READACK - ", moveaddr[moveaddr.Length - 1], " moved to ", Settings.DownloadFolder, "ArchiveLogs\\"));
                        }
                        else
                        {
                            log.Error(DateTime.Now + ",READACK ERROR - Could not find Log entry for " + FIL_NAME);
                            File.Move(file, string.Concat(Settings.DownloadFolder, moveaddr[moveaddr.Length - 1], ".notfound"));
                        }
                    }
                    catch (Exception ee)
                    {
                        if (ee.Message.Contains("Cannot create a file when that file already exists"))
                        {
                            File.Delete(file);
                            log.Error(DateTime.Now + ",READACK ERROR - Duplicate Ack deleted " + FIL_NAME);
                        }
                        log.Error(DateTime.Now + ", READACK ERROR - ACK File Read Failed, " + ee.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(string.Concat(DateTime.Now, ", READACK ERROR - ACK File Processing Failed ", ex.Message));
                throw;
            }
        }
        #endregion

        #region Post
        public void PostCVF(long FileID)
        {
            try
            {
                SCT_Validation Validation = new SCT_Validation(Settings.Connection.ConnectionString);
                Validation.CVF_Accepted(FileID, CreditFiles.Providers.CUSOP, Settings.BIC, true);

                log.Info(string.Concat(DateTime.Now, ", POST CVF - File Processing Completed for ", FileID));
            }
            catch (Exception Ex)
            {
                log.Error(string.Concat(DateTime.Now, ", POST ERROR - CVF File Processing Failed for ", FileID, " ", Ex.Message));
            }
        }
        #endregion

        #region Generate files
        internal string CreateFile()
        {
            string FileName = string.Empty;
            try
            {
                string OutputFolder = Settings.UploadFolder;

                CreditFiles CT = new CreditFiles(Settings.Connection.ConnectionString, CreditFiles.Providers.CUSOP, Settings.BIC, true);
                DsSCT ds = new Abacus.SEPA.DataSets.DsSCT();

                DateTime dtFrom = DateTime.Today;

                if (dtFrom.DayOfWeek != DayOfWeek.Saturday)//needed to return any CTs created with saturday settle date (temp fix)
                    dtFrom = CreditFiles.GetSettlementDate(dtFrom, SEPA.ClosedDays());

                CT.SEPA_CT_SendFetch(CreditFiles.CT_Status.AnyOutStatus,
                    dtFrom, CreditFiles.GetSettlementDate(dtFrom.AddDays(1), SEPA.ClosedDays()), ds);

                if (ds.SEPA_CT_File.Rows.Count > 0)
                {
                    CreditFiles.ICFReturn ReturnInfo = CT.CreateICF(ds, (CreditFiles.CT_Status)3, OutputFolder, Settings.PayPeriods, Settings.User, Settings.Terminal,
                        CreditFiles.GetSettlementDateByTime(DateTime.Today, Settings.MiddayCutoff, SEPA.ClosedDays()),  //check this
                        Settings.SettlementAccID,
                        Settings.MiddayCutoff, SEPA.ClosedDays());

                    log.Info(String.Concat(DateTime.Now, ", CREATEICF - FileName: ", ReturnInfo.FileName));
                }

                return FileName;
            }
            catch (Exception ex)
            {
                log.Error(string.Concat(DateTime.Now, ", CreateFile error - ", ex.ToString()));
                throw ex;
            }
        }
        #endregion
    }
}
