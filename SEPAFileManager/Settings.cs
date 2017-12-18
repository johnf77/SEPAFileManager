using System;
using System.Configuration;
using System.Management;
using System.Data.SqlClient;
using System.Data;

namespace SEPAFileManager
{
    internal class Settings
    {
        public static string DownloadFolder = ConfigurationManager.AppSettings["DownloadFolder"].ToString();
        public static string UploadFolder = ConfigurationManager.AppSettings["UploadFolder"].ToString();
        public static string NSC = ConfigurationManager.AppSettings["NSC"].ToString();
        public static string BIC = ConfigurationManager.AppSettings["BIC"].ToString();
        public static string CreditorIdentifier = ConfigurationManager.AppSettings["CreditorIdentifier"].ToString();
        public static string CreateFilesCutoff = ConfigurationManager.AppSettings["CreateFilesCutoff"].ToString().ToUpper();
        public static string MiddayCutoff = ConfigurationManager.AppSettings["MiddayCutoff"].ToString().ToUpper();
        public static string CheckerTime = ConfigurationManager.AppSettings["CheckerTime"].ToString().ToUpper();
        public static int UploadInterval = Int32.Parse(ConfigurationManager.AppSettings["UploadInterval"]) * 1000 * 60;
        public static int DownloadInterval = Int32.Parse(ConfigurationManager.AppSettings["DownloadInterval"]) * 1000 * 60;
        public static bool GenerateDDs = (ConfigurationManager.AppSettings["GenerateDDs"].ToString().ToUpper() == "TRUE");
        public static bool CreateDDFile = (ConfigurationManager.AppSettings["CreateDDFile"].ToString().ToUpper() == "TRUE");
        public static string DDCreateFilesCutoff = ConfigurationManager.AppSettings["DDCreateFilesCutoff"].ToString().ToUpper();

        public static Abacus.BusinessRules.PaymentPeriods PayPeriods;
        public static Abacus.BusinessRules.AbacusUser User;
        public static Abacus.BusinessRules.Terminal Terminal;
        public static Abacus.BusinessRules.GL.Currency HomeCurrency;

        public static int SettlementAccID;

        public class Connection
       {
            public static string ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ToString();
            public static string HostAddress = ConfigurationManager.ConnectionStrings["HostAddress"].ToString();
            public static string Username = ConfigurationManager.ConnectionStrings["Username"].ToString();
            public static int Port = Int32.Parse(ConfigurationManager.ConnectionStrings["Port"].ToString());
            public static string Key = ConfigurationManager.ConnectionStrings["Key"].ToString();
            public static string SFTPPass = ConfigurationManager.ConnectionStrings["SFTPPass"].ToString();
            public static string Login = ConfigurationManager.ConnectionStrings["User"].ToString();
            public static string Password = ConfigurationManager.ConnectionStrings["Password"].ToString();            
       }

       public class Pingmon
       {
            public static bool MonitorOn = Convert.ToBoolean(ConfigurationManager.AppSettings["PingmonOn"]);
            public static string Url = ConfigurationManager.AppSettings["PingmonURL"].ToString();
            public static int Timeout = Int32.Parse(ConfigurationManager.AppSettings["PingmonTimeout"].ToString());
            public static int Interval = Convert.ToInt32(ConfigurationManager.AppSettings["PingmonInterval"]) * 1000 * 60;
        }

       public Settings()
       {
            Abacus.Constant.ConnectionString = Settings.Connection.ConnectionString;
            if(ConfigurationManager.AppSettings["SettlementAcc"].Length > 0)
                Settings.SettlementAccID = FetchGLAccount(ConfigurationManager.AppSettings["SettlementAcc"].ToString(), Settings.Connection.ConnectionString);

            Settings.PayPeriods = new Abacus.BusinessRules.PaymentPeriods();
            Settings.User = new Abacus.BusinessRules.AbacusUser(Settings.Connection.Login, Cryption.Decrypt(Settings.Connection.Password), Settings.Connection.ConnectionString);
            Settings.Terminal = Abacus.BusinessRules.Terminal.Find(Settings.GetMACAddress(), "");
            Settings.HomeCurrency = new Abacus.BusinessRules.GL.Currency(true);
        }

        private static string GetMACAddress()
        {
            ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection moc = mc.GetInstances();
            string MACAddress = String.Empty;

            int ActiveNetworkCount = 0;

            foreach (ManagementObject mo in moc)
            {
                if (mo["MacAddress"] == null)
                    continue;

                if ((bool)mo["IPEnabled"])
                {
                    ActiveNetworkCount++;

                    if (MACAddress != String.Empty)
                        MACAddress += ",";

                    MACAddress += mo["MacAddress"].ToString();
                }

                mo.Dispose();
            }

            if (ActiveNetworkCount == 0)
            {
                MACAddress = "";

                foreach (ManagementObject mo in moc)
                {
                    if (mo["MacAddress"] == null)
                        continue;

                    if (MACAddress != String.Empty)
                        MACAddress += ",";

                    MACAddress += mo["MacAddress"].ToString();

                    mo.Dispose();
                }
            }

            return MACAddress;
        }

        public static int FetchGLAccount(string Number, string ConnectionString)
        {
            int AccountID = 0;
            SqlConnection con = new SqlConnection(ConnectionString);
            try
            {
                SqlCommand cmd = new SqlCommand("AccountFetchByNumber");
                cmd.Parameters.Add("@AccountID", SqlDbType.Int);
                cmd.Parameters.Add("@Number", SqlDbType.NVarChar, 50);
                cmd.Parameters.Add("@CategoryID", SqlDbType.TinyInt);
                cmd.Parameters.Add("@AccountTypeID", SqlDbType.TinyInt);

                cmd.Parameters[0].Direction = ParameterDirection.Output;
                cmd.Parameters[1].Value = Number;

                cmd.Parameters[2].Value = 0;
                cmd.Parameters[3].Value = 0;

                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                cmd.Connection = con;
                cmd.ExecuteNonQuery();

                AccountID = (int)cmd.Parameters["@AccountID"].Value;
                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (con.State != ConnectionState.Closed)
                {
                    con.Close();
                }
            }
            return AccountID;
        }
    }
}