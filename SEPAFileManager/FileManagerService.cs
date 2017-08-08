using System;
using System.Net;
using System.Timers;

namespace SEPAFileManager
{
    public class FileManagerService
    {
        internal static readonly log4net.ILog Logger = log4net.LogManager.GetLogger("SEPAFileManager");
        private Timer timerCreateFile = new Timer();
        private Timer timerUpload = new Timer();
        private Timer timerDownload = new Timer();
        private Timer timerReadFiles = new Timer();
        private Timer timerPingMon = new Timer();
        private Timer timerAckChecker = new Timer();
        private Timer timerCTUnsentChecker = new Timer();
        private Timer timerVFChecker = new Timer();
        public static DateTime NextFileCreate;
        private DateTime CheckerRunTime;

        public FileManagerService()
        {
            Logger.Info("FileManagerService started");
        }

        public void Start()
        {
            System.Reflection.AssemblyName AssName = System.Reflection.Assembly.GetExecutingAssembly().GetName();
            string AppVersionNumber = "{0}.{1}.{2}";
            AppVersionNumber = string.Format(AppVersionNumber, AssName.Version.Major, AssName.Version.Minor, AssName.Version.Build);
            //double PatchNo = 0;
            //if (PatchNo > 0)
            AppVersionNumber = String.Concat(AppVersionNumber, "-", "");

            Logger.Info(string.Concat("Service Version  - Ack & CT Checks", AppVersionNumber));

            timerCreateFile.Elapsed += new ElapsedEventHandler(timerCreateFile_Elapsed);
            timerUpload.Interval = Settings.UploadInterval;
            timerUpload.Elapsed += new ElapsedEventHandler(timerUpload_Elapsed);
            timerUpload.Start();
            timerDownload.Interval = Settings.UploadInterval;
            timerDownload.Elapsed += new ElapsedEventHandler(timerDownload_Elapsed);
            timerDownload.Start();

            timerReadFiles.Elapsed += new ElapsedEventHandler(timerReadFiles_Elapsed);
            timerReadFiles.Interval = 60000;
            timerReadFiles.Start();

            timerAckChecker.Elapsed += new ElapsedEventHandler(timerAckChecker_Elapsed);
            timerCTUnsentChecker.Elapsed += new ElapsedEventHandler(timerCTUnsentChecker_Elapsed);
            timerVFChecker.Elapsed += new ElapsedEventHandler(timerVFChecker_Elapsed);

            SetNextFileCreateTime();

            Logger.Info(String.Concat(DateTime.Now, ", 1st File Creation at: ", NextFileCreate.ToShortDateString(), ":", NextFileCreate.ToShortTimeString()));
            SetTimer(timerCreateFile, NextFileCreate);

            string[] Time = Settings.CheckerTime.Split(':');
            CheckerRunTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Convert.ToInt32(Time[0]), Convert.ToInt32(Time[1]), 0);

            Logger.Info(String.Concat(DateTime.Now, ", Next Ack check at: ", Settings.CheckerTime));
            SetTimer(timerAckChecker, CheckerRunTime);

            Logger.Info(String.Concat(DateTime.Now, ", Next VF check at: ", CheckerRunTime.AddHours(1).ToShortDateString(), ":", CheckerRunTime.AddHours(1).ToShortTimeString()));
            SetTimer(timerVFChecker, CheckerRunTime.AddHours(1));

            StartPingMon();
            Logger.Info("Service Started");
        }

        private static void SetTimer(Timer timer, DateTime due)
        {
            try
            {
                var ts = due - DateTime.Now;
                timer.Interval = ts.TotalMilliseconds;
                timer.AutoReset = false;
                timer.Start();
            }
            catch (Exception ex)
            {
                Logger.Debug(String.Concat(DateTime.Now, ", SetTimer", ex.Message));
            }
        }

        public static void SetNextFileCreateTime()
        {
            string[] Times = Settings.CreateFilesCutoff.Split(',');
            DateTime NextTime;
            string[] Minutes;

            //default to first time
            Minutes = Times[0].Split(':');
            NextFileCreate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Convert.ToInt32(Minutes[0]), Convert.ToInt32(Minutes[1]), 0);

            for (int i = 0; i < Times.Length; i++)
            {
                Minutes = Times[i].Split(':');
                NextTime = new DateTime(NextFileCreate.Year, NextFileCreate.Month, NextFileCreate.Day, Convert.ToInt32(Minutes[0]), Convert.ToInt32(Minutes[1]), 0);

                if (NextTime.CompareTo(DateTime.Now.AddMinutes(2)) > 0)
                {
                    NextFileCreate = NextTime;
                    break;
                }
            }

            var ts = NextFileCreate - DateTime.Now;

            if (ts.TotalMilliseconds < 0)
                NextFileCreate = NextFileCreate.AddDays(1);
        }

        private bool _stopping = false;
        public void Stop()
        {
            _stopping = true;
        }

        #region timer events
        private void timerVFChecker_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                Abacus.DAL.DAO objDAO = new Abacus.DAL.DAO();
                Checker.CVFCheck(objDAO);

                if (Settings.CreateDDFile)
                    Checker.DVFCheck(objDAO);
            }
            catch (Exception ex)
            {
                Logger.Debug(string.Concat(DateTime.Now, ", VF Checker Failed, ", ex.Message));
            }
            finally
            {
                SetTimer(timerVFChecker, DateTime.Now.AddDays(1));
                Logger.Info(String.Concat(DateTime.Now, ", Next VF check at: ", DateTime.Now.AddDays(1).ToShortDateString(), ":", DateTime.Now.AddDays(1).ToShortTimeString()));
            }
        }

        private void timerCTUnsentChecker_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                Abacus.DAL.DAO objDAO = new Abacus.DAL.DAO();
                Checker.CT_UnsentCheck(objDAO);
            }
            catch (Exception ex)
            {
                Logger.Debug(string.Concat(DateTime.Now, ", CT Unsent Checker Failed, ", ex.Message));
            }
            finally
            {
                SetTimer(timerCTUnsentChecker, DateTime.Now.AddDays(1));
                Logger.Info(String.Concat(DateTime.Now, ", Next CT Unsent check at: ", DateTime.Now.AddDays(1).ToShortDateString(), ":", DateTime.Now.AddDays(1).ToShortTimeString()));
            }
        }

        private void timerAckChecker_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                Abacus.DAL.DAO objDAO = new Abacus.DAL.DAO();

                Checker.ACKCheck("SEPA_CTLog", objDAO);

                if (Settings.CreateDDFile)
                    Checker.ACKCheck("SEPA_DDLog", objDAO);
            }
            catch (Exception ex)
            {
                Logger.Debug(string.Concat(DateTime.Now, ", ACK Checker Failed, ", ex.Message));
            }
            finally
            {
                SetTimer(timerAckChecker, DateTime.Now.AddDays(1));
                Logger.Info(String.Concat(DateTime.Now, ", Next Ack check at: ", DateTime.Now.AddDays(1).ToShortDateString(), ":", DateTime.Now.AddDays(1).ToShortTimeString()));
            }
        }

        private bool _reading = false;
        private void timerReadFiles_Elapsed(object sender, ElapsedEventArgs e)
        {
            {
                try
                {
                    if (_stopping) return;

                    if (_reading) return;
                    _reading = true;

                    FileProcessing fp = new FileProcessing();
                    fp.ReadFiles();
                    fp.ReadACK();
                    _reading = false;
                }
                catch (Exception ex)
                {
                    _reading = false;
                    Logger.Error("ReadFilesError", ex);
                }
            }
        }

        private bool _downloading = false;
        private void timerDownload_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                if (_stopping) return;

                if (_downloading) return;
                _downloading = true;

                SFTPFiles sftp = new SFTPFiles();
                sftp.Download();

                _downloading = false;
            }
            catch (Exception ex)
            {
                _downloading = false;
                Logger.Error("DownloadError", ex);
            }
            finally
            {
            }
        }        

        private bool _uploading = false;
        private void timerUpload_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                //to ensure timerUpload called at expected interval
                //Logger.Info(string.Concat(DateTime.Now, "IN timerUpload _stopping = ", _stopping, " _uploading = ", _uploading));

                if (_stopping) return;

                if (_uploading) return;
                _uploading = true;

                // Send files via SFTP
                SFTPFiles sftp = new SFTPFiles();
                sftp.Upload();
                _uploading = false;
            }
            catch (Exception ex)
            {
                _uploading = false;
                Logger.Error("UploadError", ex);
            }
        }

        private bool _creatingfile = false;
        private void timerCreateFile_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                if (_stopping) return;

                if (_creatingfile) return;
                _creatingfile = true;

                FileProcessing fp = new FileProcessing();
                fp.CreateFile();
                _creatingfile = false;
            }
            catch (Exception ex)
            {
                _creatingfile = false;
                Logger.Error(String.Concat(DateTime.Now, ", CreateFile failed, ", ex.Message));
            }
            finally
            {
                // Generate the time files will next be created.
                SetNextFileCreateTime();
                Logger.Info(String.Concat(DateTime.Now, ", Next File Creation at: ", NextFileCreate.ToShortDateString(), ":", NextFileCreate.ToShortTimeString()));
                SetTimer(timerCreateFile, NextFileCreate);
            }
        }
        #endregion timer events        

        #region Pingmon
        private void StartPingMon()
        {
            if (!Settings.Pingmon.MonitorOn) return;

            timerPingMon.Elapsed += timerPingMon_Elapsed;
            timerPingMon.Interval = Settings.Pingmon.Interval;
            timerPingMon.Start();

            Logger.Info("Pingmon started. Pinging " + Settings.Pingmon.Url);
        }

        private void timerPingMon_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                if (_stopping) return;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Settings.Pingmon.Url);
                request.Timeout = Settings.Pingmon.Timeout;
                request.GetResponse();

                Console.WriteLine("Pinging:- " + Settings.Pingmon.Url);
            }
            catch (Exception ex)
            {
                Logger.Error("PINGMON Ping Exception: " + ex.Message);
            }
        }
        #endregion PingMon
    }
}
