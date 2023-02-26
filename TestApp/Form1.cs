using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using SharpUpdate;

namespace TestApp
{
    public partial class Form1 : Form, ISharpUpdatable
    {
        public Form1()
        {
            InitializeComponent();
            this.label1.Text = ApplicationAssembly.GetName().Version.ToString();
            updater = new SharpUdater(this);
        }

        private SharpUdater updater;

        public string ApplicationName { get { return "TestApp"; } }
        //public string ApplicationName => "TestApp";
        //public string ApplicationName => throw new NotImplementedException();

        public string ApplicationID { get { return "TestApp"; } }
        //public string ApplicationID => throw new NotImplementedException();

        public Assembly ApplicationAssembly { get { return  Assembly.GetExecutingAssembly(); } }
        //public Assembly ApplicationAssembly => throw new NotImplementedException();

        public Icon ApplicationIcon { get { return this.Icon; } }
        //public Icon ApplicationIcon => throw new NotImplementedException();

        public Uri UpdateXmlLocation { get { return new Uri("https://Sourcecodedepot.com/update/updateTest.xml"); } }
        //public Uri UpdateXmlLocation => throw new NotImplementedException();

        public Form Context { get { return this; } }
        //public Form Context => throw new NotImplementedException();

        private void button1_Click(object sender, EventArgs e)
        {
            //ISharpUpdate applicationInfo;
            //SharpUdater updater = new SharpUdater();
            updater.DoUpdate();
        }

        public string checkMD5(string filename)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    return Encoding.Default.GetString(md5.ComputeHash(stream));
                }
            }
        }

        //Try to download file it is proper bytes
        private void button2_Click(object sender, EventArgs e)
        {
            WebClient downloader = new WebClient();
            using (downloader)
            {
                try
                {
                    string appPath = Path.GetDirectoryName(Application.ExecutablePath) + "\\TestApp1.exe";
                    downloader.DownloadFile("https://sourcecodedepot.com/update/TestApp.exe", appPath);
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

            Application.Exit();
           // File.Delete(Path.GetDirectoryName(Application.ExecutablePath) + "\\TestApp.exe");
           
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            //Thread.Sleep(6);
            //File.Delete(Path.GetDirectoryName(Application.ExecutablePath) + "\\TestApp.exe");
            //File.Move(Path.GetDirectoryName(Application.ExecutablePath) + "\\TestApp1.exe", Path.GetDirectoryName(Application.ExecutablePath) + "\\TestApp.exe");
            //Application.Restart();
        }
    }
}
