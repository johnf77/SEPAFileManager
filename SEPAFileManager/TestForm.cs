using System;
using System.Data;
using System.Windows.Forms;
using Tamir.SharpSsh;

namespace SEPAFileManager
{
    public partial class TestForm : Form
    {
        public TestForm()
        {
            InitializeComponent();
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            SFTPFiles sftp = new SFTPFiles();
            try
            {
                sftp.Download();
            }
            catch (Exception ex)
            {
                txtOut.Text += "\r\n" + ex.ToString();
            }
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            SFTPFiles sftp = new SFTPFiles();
            try
            {
                sftp.Upload();
            }
            catch (Exception ex)
            {
                txtOut.Text += "\r\n" + ex.ToString();
            }
        }


        private void btnSFTPConnect_Click(object sender, EventArgs e)
        {
            SFTPFiles sftp = new SFTPFiles();
            try
            {
                this.txtOut.AppendText("SFTP Connected\r\n");
            }
            catch (Exception ex)
            {
                this.txtOut.AppendText(string.Concat("Error: ", ex.Message, "\r\n", ex.Source));
            }
            finally
            {
                sftp.Close();
            }
        }

        private void btnDatabaseConnect_Click(object sender, EventArgs e)
        {
            try
            {
                using (System.Data.SqlClient.SqlConnection Connection = new System.Data.SqlClient.SqlConnection(Settings.Connection.ConnectionString))
                {
                    Connection.ConnectionString = Settings.Connection.ConnectionString;
                    Connection.Open();

                    if (Connection.State == ConnectionState.Open)
                    {
                        this.txtOut.AppendText("DB Connected\r\n");
                    }
                    else
                    {
                        this.txtOut.AppendText("DB NOT CONNECTED\r\n");
                    }
                }
            }
            catch (Exception ex)
            {
                this.txtOut.AppendText(string.Concat("Error: ", ex.Message, "\r\n", ex.Source));
            }
        }

        private void btnReadFiles_Click(object sender, EventArgs e)
        {
            FileProcessing fp = new FileProcessing();
            fp.ReadFiles();
        }

        private void btnReadAck_Click(object sender, EventArgs e)
        {
            FileProcessing fp = new FileProcessing();
            fp.ReadACK();
        }

        private void btnCreateFile_Click(object sender, EventArgs e)
        {
            FileProcessing fp = new FileProcessing();
            FileManagerService.SetNextFileCreateTime();
            this.txtOut.AppendText(string.Concat("File Created: ", fp.CreateFile()));
        }

        private void btnOnStart_Click(object sender, EventArgs e)
        {
            FileManagerService fms = new FileManagerService();
            fms.Start();
        }

        private void btnPostCVF_Click(object sender, EventArgs e)
        {
            FileProcessing fp = new FileProcessing();
            fp.PostCVF(3352);
        }
    }
}
