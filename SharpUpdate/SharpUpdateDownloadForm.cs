using System;
using System.IO;
using System.ComponentModel;
using System.Net;
using System.Drawing;
using System.Windows.Forms;

namespace SharpUpdate
{
    internal partial class SharpUpdateDownloadForm : Form
    {

        private WebClient webClient;
        private BackgroundWorker bgWorker;
        private String tempFile;
        private String md5;

        internal string TempFilePath{ get { return this.tempFile; }}

        internal SharpUpdateDownloadForm(Uri location, string md5, Icon programIcon)
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

        }

        private void BgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.DialogResult = (DialogResult)e.Result;
            this.Close();
        }

        private void BgWorker_DoWork(object sender, DoWorkEventArgs e)
        {

            string file = ((string[])e.Argument)[0];
            string updateMd5 = ((string[])e.Argument)[1];

            if (Hasher.HashFile(file, HashType.MD5) != updateMd5)
                e.Result = DialogResult.No;

            else
                e.Result = DialogResult.OK;

        }

        private void WebClient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                this.Close();
            }
            else if (e.Cancelled)
            {
                this.Close();
            }
            else
            {
                lblProgress.Text = "Verifying Download...";
                progressBar.Style = ProgressBarStyle.Marquee;
                bgWorker.RunWorkerAsync(new string[] { this.tempFile, this.md5 });
            }
        }

        private void WebClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            this.progressBar.Value = e.ProgressPercentage;
            this.lblProgress.Text = String.Format("Downloaded {0} of {1}", Formatbytes(e.BytesReceived, 1, true), Formatbytes(e.TotalBytesToReceive, 1, true));
        }


        private string Formatbytes(long bytes, int decimalplaces, bool showByteType)
        {
            double newBytes = bytes;
            string formatString = "{0";
            string byteType = "b";

            if (newBytes > 1024 && newBytes < 1048756)
            {
                newBytes /= 1024; byteType = "KB";
            }
            else if (newBytes > 1048756 && newBytes < 1073741824)
            {
                newBytes /= 1048756; byteType = "MB";
            }
            else
            {
                newBytes /= 1073741824; byteType = "GB";
            }

            if (decimalplaces > 0)
                formatString += ":0.";

            for (int i = 0; i < decimalplaces; i++)
                formatString += "0";

            formatString += "}";

            if (showByteType)
                formatString += byteType;

            return string.Format(formatString, newBytes);

        }

        private void SharpUpdateDownloadForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (webClient.IsBusy)
            {
                webClient.CancelAsync();
                this.DialogResult = DialogResult.Abort;
            }

            if (bgWorker.IsBusy)
            {
                bgWorker.CancelAsync();
                this.DialogResult = DialogResult.Abort;
            }
        }
    }
}
