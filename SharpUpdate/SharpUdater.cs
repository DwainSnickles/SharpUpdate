using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace SharpUpdate
{
    public class SharpUdater
    {
        private ISharpUpdatable applicationInfo;
        private BackgroundWorker bgWorker;

        public SharpUdater(ISharpUpdatable applicationInfo)
        {
            this.applicationInfo = applicationInfo;

            this.bgWorker = new BackgroundWorker();
            this.bgWorker.DoWork += BgWorker_DoWork;
            this.bgWorker.RunWorkerCompleted += BgWorker_RunWorkerCompleted;
        }

        public void DoUpdate()
        {
            if (!this.bgWorker.IsBusy)
                this.bgWorker.RunWorkerAsync(this.applicationInfo);
        }
           
        private void BgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                SharpUpdateXml update = (SharpUpdateXml)e.Result;

                if (update != null && update.IsNewerThan(this.applicationInfo.ApplicationAssembly.GetName().Version))
                {
                    if (new SharpUpdateAcceptForm(this.applicationInfo, update).ShowDialog(this.applicationInfo.Context) == DialogResult.Yes)
                        this.DownloadUpdate(update);

                    //restart app
                    //Application.Restart();

                }
            }
        }

        private void DownloadUpdate(SharpUpdateXml update)
        {
            //Form f = new Form1(update.Uri, update.MD5, this.applicationInfo.ApplicationIcon);
            //f.ShowDialog();
            SharpUpdateDownloadForm form = new SharpUpdateDownloadForm(update.Uri, update.MD5, this.applicationInfo.ApplicationIcon);
            DialogResult result = form.ShowDialog(this.applicationInfo.Context);
            string appPath = Path.GetDirectoryName(Application.ExecutablePath) + "\\TestApp1.exe";

            if (result == DialogResult.OK || result == DialogResult.Cancel)
            {
                // Download file using webclient and add the file to bin folder
                WebClient downloader = new WebClient();
                
                using (downloader)
                {
                    try
                    {
                        downloader.DownloadFile("https://sourcecodedepot.com/update/TestApp1.exe", appPath);
                    }
                    catch (ArgumentException ae)
                    {
                        Debug.WriteLine("{0} - {1}", ae.GetType(), ae.Message);
                    }
                    catch (WebException webEx)
                    {
                        Debug.WriteLine("{0} - {1}", webEx.GetType(), webEx.Message);
                        Debug.WriteLine("Destination not found!");
                    }
                    catch (NotSupportedException supportEx)
                    {
                        Debug.WriteLine("{0} - {1}", supportEx.GetType(), supportEx.Message);
                        Debug.WriteLine(supportEx.Message);
                    }
                    catch (Exception allExp)
                    {
                        Debug.WriteLine("{0} - {1}", allExp.GetType(), allExp.Message);
                    }
                }

                string currentPath = this.applicationInfo.ApplicationAssembly.Location;
                string newPath = Path.GetDirectoryName(currentPath) + "\\" + update.Filename;

                UpdateApplication(appPath, currentPath, newPath);

                Application.Exit();
            }
            else if (result == DialogResult.Abort)
            {
                MessageBox.Show("The update download was cancellled.\nThis Program has not been updated", "Updated Cancelled", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("There was problem trying to download update.\nUpload download error", "Updated Cancelled", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void UpdateApplication(string tempFilePath, string currentPath, string newPath)
        {
            //Alow program to quit 4 sec delay & Delete file quietly   &  allow a 2 sec delay before moving & /Y overwites    & start new file
            string argument = "/D /C \"Choice /C Y /N /D Y /T 4 & Del /A /F /Q \"{0}\" & Choice /C Y /N /D Y /T 2 & Move /Y \"{1}\" \"{2}\" & Start \"\" /D \"{3}\" \"{4}\"\"";

            // only showed window to troubleshoot Cant Delete file access denied
            //in real app you should hide the cmd window
            ProcessStartInfo info = new ProcessStartInfo();
            info.Arguments = string.Format(argument, currentPath, tempFilePath, newPath, Path.GetDirectoryName(newPath), Path.GetFileName(newPath));
            info.WindowStyle = ProcessWindowStyle.Normal;
            info.CreateNoWindow = true;
            info.FileName = "cmd.exe";
            Process.Start(info);
        }



        private void BgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            ISharpUpdatable application = (ISharpUpdatable)e.Argument;

            if (!SharpUpdateXml.ExistsOnServer(application.UpdateXmlLocation))
                e.Cancel = true;
            else
                e.Result = SharpUpdateXml.Parse(application.UpdateXmlLocation, application.ApplicationID);
        }
    }
}
