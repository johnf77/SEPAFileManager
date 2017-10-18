namespace SEPAFileManager
{
    partial class TestForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnDownload = new System.Windows.Forms.Button();
            this.txtOut = new System.Windows.Forms.TextBox();
            this.btnUpload = new System.Windows.Forms.Button();
            this.btnSFTPConnect = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.txtUp = new System.Windows.Forms.TextBox();
            this.btnDatabaseConnect = new System.Windows.Forms.Button();
            this.btnReadFiles = new System.Windows.Forms.Button();
            this.btnReadAck = new System.Windows.Forms.Button();
            this.btnCreateFile = new System.Windows.Forms.Button();
            this.btnPostCVF = new System.Windows.Forms.Button();
            this.btnOnStart = new System.Windows.Forms.Button();
            this.btnDDGen = new System.Windows.Forms.Button();
            this.btnCreateDDFile = new System.Windows.Forms.Button();
            this.btnPostDVF = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnDownload
            // 
            this.btnDownload.Location = new System.Drawing.Point(33, 75);
            this.btnDownload.Name = "btnDownload";
            this.btnDownload.Size = new System.Drawing.Size(96, 23);
            this.btnDownload.TabIndex = 5;
            this.btnDownload.Text = "Download";
            this.btnDownload.UseVisualStyleBackColor = true;
            this.btnDownload.Click += new System.EventHandler(this.btnDownload_Click);
            // 
            // txtOut
            // 
            this.txtOut.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtOut.Location = new System.Drawing.Point(33, 239);
            this.txtOut.Multiline = true;
            this.txtOut.Name = "txtOut";
            this.txtOut.Size = new System.Drawing.Size(516, 162);
            this.txtOut.TabIndex = 7;
            // 
            // btnUpload
            // 
            this.btnUpload.Location = new System.Drawing.Point(33, 104);
            this.btnUpload.Name = "btnUpload";
            this.btnUpload.Size = new System.Drawing.Size(96, 23);
            this.btnUpload.TabIndex = 8;
            this.btnUpload.Text = "Upload";
            this.btnUpload.UseVisualStyleBackColor = true;
            this.btnUpload.Click += new System.EventHandler(this.btnUpload_Click);
            // 
            // btnSFTPConnect
            // 
            this.btnSFTPConnect.Location = new System.Drawing.Point(33, 12);
            this.btnSFTPConnect.Name = "btnSFTPConnect";
            this.btnSFTPConnect.Size = new System.Drawing.Size(96, 23);
            this.btnSFTPConnect.TabIndex = 9;
            this.btnSFTPConnect.Text = "SFTP Connect";
            this.btnSFTPConnect.UseVisualStyleBackColor = true;
            this.btnSFTPConnect.Click += new System.EventHandler(this.btnSFTPConnect_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(30, 181);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(73, 13);
            this.label4.TabIndex = 33;
            this.label4.Text = "Upload Folder";
            // 
            // txtUp
            // 
            this.txtUp.Location = new System.Drawing.Point(128, 174);
            this.txtUp.Name = "txtUp";
            this.txtUp.Size = new System.Drawing.Size(264, 20);
            this.txtUp.TabIndex = 34;
            // 
            // btnDatabaseConnect
            // 
            this.btnDatabaseConnect.Location = new System.Drawing.Point(33, 41);
            this.btnDatabaseConnect.Name = "btnDatabaseConnect";
            this.btnDatabaseConnect.Size = new System.Drawing.Size(96, 23);
            this.btnDatabaseConnect.TabIndex = 35;
            this.btnDatabaseConnect.Text = "DB Connect";
            this.btnDatabaseConnect.UseVisualStyleBackColor = true;
            this.btnDatabaseConnect.Click += new System.EventHandler(this.btnDatabaseConnect_Click);
            // 
            // btnReadFiles
            // 
            this.btnReadFiles.Location = new System.Drawing.Point(167, 12);
            this.btnReadFiles.Name = "btnReadFiles";
            this.btnReadFiles.Size = new System.Drawing.Size(96, 23);
            this.btnReadFiles.TabIndex = 36;
            this.btnReadFiles.Text = "Read Files";
            this.btnReadFiles.UseVisualStyleBackColor = true;
            this.btnReadFiles.Click += new System.EventHandler(this.btnReadFiles_Click);
            // 
            // btnReadAck
            // 
            this.btnReadAck.Location = new System.Drawing.Point(167, 41);
            this.btnReadAck.Name = "btnReadAck";
            this.btnReadAck.Size = new System.Drawing.Size(96, 23);
            this.btnReadAck.TabIndex = 37;
            this.btnReadAck.Text = "Read Ack";
            this.btnReadAck.UseVisualStyleBackColor = true;
            this.btnReadAck.Click += new System.EventHandler(this.btnReadAck_Click);
            // 
            // btnCreateFile
            // 
            this.btnCreateFile.Location = new System.Drawing.Point(279, 12);
            this.btnCreateFile.Name = "btnCreateFile";
            this.btnCreateFile.Size = new System.Drawing.Size(96, 23);
            this.btnCreateFile.TabIndex = 38;
            this.btnCreateFile.Text = "Create CT File";
            this.btnCreateFile.UseVisualStyleBackColor = true;
            this.btnCreateFile.Click += new System.EventHandler(this.btnCreateFile_Click);
            // 
            // btnPostCVF
            // 
            this.btnPostCVF.Location = new System.Drawing.Point(394, 12);
            this.btnPostCVF.Name = "btnPostCVF";
            this.btnPostCVF.Size = new System.Drawing.Size(96, 23);
            this.btnPostCVF.TabIndex = 39;
            this.btnPostCVF.Text = "Post CVF";
            this.btnPostCVF.UseVisualStyleBackColor = true;
            this.btnPostCVF.Click += new System.EventHandler(this.btnPostCVF_Click);
            // 
            // btnOnStart
            // 
            this.btnOnStart.Location = new System.Drawing.Point(496, 12);
            this.btnOnStart.Name = "btnOnStart";
            this.btnOnStart.Size = new System.Drawing.Size(96, 23);
            this.btnOnStart.TabIndex = 40;
            this.btnOnStart.Text = "OnStart";
            this.btnOnStart.UseVisualStyleBackColor = true;
            this.btnOnStart.Click += new System.EventHandler(this.btnOnStart_Click);
            // 
            // btnDDGen
            // 
            this.btnDDGen.Location = new System.Drawing.Point(279, 75);
            this.btnDDGen.Name = "btnDDGen";
            this.btnDDGen.Size = new System.Drawing.Size(96, 23);
            this.btnDDGen.TabIndex = 41;
            this.btnDDGen.Text = "DD Gen";
            this.btnDDGen.UseVisualStyleBackColor = true;
            this.btnDDGen.Click += new System.EventHandler(this.btnDDGen_Click);
            // 
            // btnCreateDDFile
            // 
            this.btnCreateDDFile.Location = new System.Drawing.Point(279, 104);
            this.btnCreateDDFile.Name = "btnCreateDDFile";
            this.btnCreateDDFile.Size = new System.Drawing.Size(96, 23);
            this.btnCreateDDFile.TabIndex = 42;
            this.btnCreateDDFile.Text = "Create DD File";
            this.btnCreateDDFile.UseVisualStyleBackColor = true;
            this.btnCreateDDFile.Click += new System.EventHandler(this.btnCreateDDFile_Click);
            // 
            // btnPostDVF
            // 
            this.btnPostDVF.Location = new System.Drawing.Point(394, 75);
            this.btnPostDVF.Name = "btnPostDVF";
            this.btnPostDVF.Size = new System.Drawing.Size(96, 23);
            this.btnPostDVF.TabIndex = 43;
            this.btnPostDVF.Text = "Post DVF";
            this.btnPostDVF.UseVisualStyleBackColor = true;
            this.btnPostDVF.Click += new System.EventHandler(this.btnPostDVF_Click);
            // 
            // TestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 423);
            this.Controls.Add(this.btnPostDVF);
            this.Controls.Add(this.btnCreateDDFile);
            this.Controls.Add(this.btnDDGen);
            this.Controls.Add(this.btnOnStart);
            this.Controls.Add(this.btnPostCVF);
            this.Controls.Add(this.btnCreateFile);
            this.Controls.Add(this.btnReadAck);
            this.Controls.Add(this.btnReadFiles);
            this.Controls.Add(this.btnDatabaseConnect);
            this.Controls.Add(this.txtUp);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btnSFTPConnect);
            this.Controls.Add(this.btnUpload);
            this.Controls.Add(this.txtOut);
            this.Controls.Add(this.btnDownload);
            this.Name = "TestForm";
            this.Text = "TestForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnDownload;
        private System.Windows.Forms.TextBox txtOut;
        private System.Windows.Forms.Button btnUpload;
        private System.Windows.Forms.Button btnSFTPConnect;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtUp;
        private System.Windows.Forms.Button btnDatabaseConnect;
        private System.Windows.Forms.Button btnReadFiles;
        private System.Windows.Forms.Button btnReadAck;
        private System.Windows.Forms.Button btnCreateFile;
        private System.Windows.Forms.Button btnPostCVF;
        private System.Windows.Forms.Button btnOnStart;
        private System.Windows.Forms.Button btnDDGen;
        private System.Windows.Forms.Button btnCreateDDFile;
        private System.Windows.Forms.Button btnPostDVF;
    }
}