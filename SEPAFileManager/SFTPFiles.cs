using Ionic.Zip;
using log4net;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Tamir.SharpSsh;

namespace SEPAFileManager
{
    class SFTPFiles
    {
        internal static readonly log4net.ILog log = log4net.LogManager.GetLogger("CusopFileManager");
        private static readonly ILog email = LogManager.GetLogger("EmailAlerter");
        private Sftp _sftp;
        public SFTPFiles()
        {
            CreateSftp();
        }

        private void CreateSftp()
        {
            try
            {
                _sftp = new Sftp(Settings.Connection.HostAddress, Settings.Connection.Username);

                log.Debug(DateTime.Now + ", SFTP - Attempting connection");
                if (Settings.Connection.Key != "")
                {
                    _sftp.AddIdentityFile(Settings.Connection.Key, Settings.Connection.SFTPPass);
                }
                else
                {
                    log.Debug(DateTime.Now + ", SFTP - Key not found.");
                    _sftp.Password = Settings.Connection.SFTPPass;
                }                
            }
            catch (Exception ex)
            {
                log.Error(DateTime.Now + ", SFTP - Connection failed");
                throw ex;
            }
        }

        public void Close()
        {
            _sftp.Close();
        }

        public void Upload()
        {
            try
            {
                string[] filePaths;
                filePaths = System.IO.Directory.GetFiles(Settings.UploadFolder, "*.*", System.IO.SearchOption.TopDirectoryOnly);

                if (filePaths.Length == 0)
                    return;

                _sftp.Connect(Settings.Connection.Port);             

                foreach (string file in filePaths)
                {
                    string[] Filename = file.Split('\\');

                    if ((File.Exists(string.Concat(Settings.UploadFolder, "Archive\\", Filename[Filename.Length - 1], ".old")))
                        || (File.Exists(string.Concat(Settings.UploadFolder, "Archive\\", Filename[Filename.Length - 1], ".DUPLICATE"))))
                    {
                        File.Move(file, string.Concat(Settings.UploadFolder, "Archive\\", Filename[Filename.Length - 1], ".DUPLICATE"));
                        log.Debug(DateTime.Now + ", UPLOAD ERROR - Duplicate file " + file + " moved, " + Settings.UploadFolder + "Archive\\");
                        continue;
                    }

                    _sftp.Put(file, "/OUTBOX/"); // Outbox on SFTP server.
                    log.Info(DateTime.Now + ", UPLOAD - " + file + " uploaded.");

                    File.Move(file, string.Concat(Settings.UploadFolder, "Archive\\", Filename[Filename.Length - 1], ".old"));
                    log.Debug(string.Concat(DateTime.Now, ", UPLOAD - ", file, " moved, ", Settings.UploadFolder));
                }
            }
            catch (Exception ex)
            {
                _sftp.Cancel();
                log.Error(string.Concat(DateTime.Now, ", UPLOAD ERROR - Upload failed ", ex.ToString()));
                throw new Exception(ex.ToString());
            }
            finally
            {
                _sftp.Close();
            }
        }

        public void Download()
        {
            try
            {
                _sftp.Connect(Settings.Connection.Port);

                var prop = _sftp.GetType().GetProperty("SftpChannel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var methodInfo = prop.GetGetMethod(true);
                var sftpChannel = methodInfo.Invoke(_sftp, null);

                System.Collections.ArrayList InboxFiles = _sftp.GetFileList("/INBOX"); // Inbox on SFTP server
                System.Collections.ArrayList AckFiles = _sftp.GetFileList("/LOGS");
                string DownloadFolder = Settings.DownloadFolder;
                int count = 0;

                foreach (string file in InboxFiles)
                {
                    if ((file.StartsWith(".")) || (file.Split('.').Length == 1))
                        continue;
                    _sftp.Get(string.Concat("/INBOX/", file), DownloadFolder); //gets files from SFTP server

                    try
                    {
                        ((Tamir.SharpSsh.jsch.ChannelSftp)sftpChannel).rename(string.Concat("/INBOX/", file),//Archive on server
                        string.Concat("/INBOX/Archive/", file, ".old"));
                    }
                    catch
                    {
                        ((Tamir.SharpSsh.jsch.ChannelSftp)sftpChannel).rename(string.Concat("/INBOX/", file),
                            string.Concat("/INBOX/Archive/", file, DateTime.Today.ToString("ddMMyy"), ".old"));
                    }

                    using (ZipFile zip = ZipFile.Read(DownloadFolder + file))
                    {
                        foreach (ZipEntry e in zip)
                        {
                            e.Extract(DownloadFolder, ExtractExistingFileAction.OverwriteSilently);  // overwrite == true
                        }
                    }

                    string[] moveaddr = file.Split('\\');
                    if (file.EndsWith(@"\"))
                    {
                        if (!File.Exists(string.Concat(DownloadFolder, "Archive\\", moveaddr[moveaddr.Length - 1], ".old")))
                            File.Move(string.Concat(DownloadFolder, file), string.Concat(DownloadFolder, "Archive\\", moveaddr[moveaddr.Length - 1], ".old"));
                        else
                            File.Move(string.Concat(DownloadFolder, file), string.Concat(DownloadFolder, "Archive\\", moveaddr[moveaddr.Length - 1], DateTime.Today.ToString("ddMMyy"), ".old"));
                    }
                    else
                        if (!File.Exists(string.Concat(DownloadFolder, "\\Archive\\", moveaddr[moveaddr.Length - 1], ".old")))
                        File.Move(string.Concat(DownloadFolder, file), string.Concat(DownloadFolder, "\\Archive\\", moveaddr[moveaddr.Length - 1], ".old"));
                    else
                        File.Move(string.Concat(DownloadFolder, file), string.Concat(DownloadFolder, "\\Archive\\", moveaddr[moveaddr.Length - 1], DateTime.Today.ToString("ddMMyy"), ".old"));

                    count++;
                }

                foreach (string file in AckFiles)
                {
                    if ((file.StartsWith(".")) || (file.Split('.').Length == 1))
                        continue;

                    string[] Extension = file.Split('.');
                    // Ignore if not an ACK/NAK
                    if ((Extension[Extension.Length - 1] != "ACK") && (Extension[Extension.Length - 1] != "NAK"))
                        continue;

                    _sftp.Get(string.Concat("/Logs/", file), DownloadFolder);

                    try
                    {
                        ((Tamir.SharpSsh.jsch.ChannelSftp)sftpChannel).rename(string.Concat("/Logs/", file),
                            string.Concat("/Logs/Archive/", file, ".old"));
                    }
                    catch
                    {
                        log.Error(DateTime.Now + ",DOWNLOAD ERROR - Duplicate Ack deleted " + file);
                        File.Delete(file);
                    }
                }

                if (count > 0)
                {
                    email.Info(string.Concat(DateTime.Now, string.Format(", DOWNLOAD  - {0} files downloaded", count)));
                }
                log.Debug(string.Concat(DateTime.Now, string.Format(", DOWNLOAD  - {0} Downloaded files processed", (InboxFiles.Count + AckFiles.Count))));
            }
            catch (Exception ex)
            {
                log.Error(DateTime.Now + ", DOWNLOAD ERROR - Download Failed, ", ex);
                throw new Exception(ex.ToString());
            }
            finally
            {
                _sftp.Close();
                log.Debug(DateTime.Now + ", DOWNLOAD - Connection closed");
            }
        }

        private bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null;
            try
            {
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException ex)
            {
                log.Info(string.Concat("OUTPUT - ", file.Name, " - IO Error - ", ex.Message));
                //the file is unavailable because it is: still being written to
                //or being processed by another thread or does not exist (has already been processed)
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            return false;
        }

        private bool MatchFilter(string[] filters, string filename)
        {
            foreach (string filter in filters)
            {
                if (filename.ToUpper().Contains(filter))
                    return true;
            }
            return false;
        }
    }
}
