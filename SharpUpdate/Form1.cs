using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace SharpUpdate
{
    public partial class Form1 : Form
    {
        private WebClient webClient;
        private BackgroundWorker bgWorker;
        private String tempFile;
        private String md5;
        internal string TempFilePath { get { return this.tempFile; } }

        public Form1(Uri location, string md5, Icon programIcon)
        {
            InitializeComponent();
            if (programIcon != null)
                this.Icon = programIcon;

            tempFile = Path.GetTempFileName();
            this.md5 = md5;

            webClient = new WebClient();
            webClient.DownloadProgressChanged += WebClient_DownloadProgressChanged;
            webClient.DownloadFileCompleted += WebClient_DownloadFileCompleted;

            bgWorker = new BackgroundWorker();
            bgWorker.DoWork += BgWorker_DoWork;
            bgWorker.RunWorkerCompleted += BgWorker_RunWorkerCompleted;

            try { webClient.DownloadFileAsync(location, this.tempFile); this.DialogResult = DialogResult.OK; }
            catch { this.DialogResult = DialogResult.No; this.Close(); }

            //MessageBox.Show(this.DialogResult.ToString());
        }

        private void BgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
           
        }

        private void BgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
        }

        private void WebClient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
        }

        private void WebClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
        }
    }
}
