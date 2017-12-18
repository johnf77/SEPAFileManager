using log4net;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Topshelf;

namespace SEPAFileManager
{
    static class Program
    {
        internal static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            string path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            log4net.Config.XmlConfigurator.Configure(new FileInfo(string.Concat(path, @"\log4net.config")));

            AssemblyName assName = Assembly.GetExecutingAssembly().GetName();

            Settings settings = new Settings();

            Logger.Info("Service Version: 1");

            if (Debugger.IsAttached)
            {
                TestForm form = new TestForm();
                form.ShowDialog();
            }
            else
            {
                HostFactory.Run(x =>                                    //1
                {
                    x.UseLog4Net("log4net.config");

                    x.Service<FileManagerService>(s =>                    //2
                    {
                        s.ConstructUsing(name => new FileManagerService());
                        s.WhenStarted(tc => tc.Start());
                        s.WhenStopped(tc => tc.Stop());                 //5
                    });

                    x.RunAsLocalSystem();                               //6

                    x.SetDescription("AbacusSEPAFileManager");          //7
                    x.SetDisplayName("AbacusSEPAFileManager");          //8
                    x.SetServiceName("AbacusSEPAFileManager");          //9

                    x.StartAutomatically();                             //10

                    x.EnableServiceRecovery(r =>
                    {
                        r.RestartService(1);
                        r.RestartService(1);
                        r.RestartService(1);

                        //should this be true for crashed or non-zero exits
                        r.OnCrashOnly();

                        //number of days until the error count resets
                        r.SetResetPeriod(1);
                    });
                });
            }
        }
    }
}
