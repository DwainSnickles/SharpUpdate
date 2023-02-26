using System;
using System.Diagnostics;
using System.Net;
using System.Windows.Forms;
using System.Xml;

//https://www.youtube.com/watch?v=McLch8np-mI

namespace SharpUpdate
{
    internal class SharpUpdateXml
    {
        private Uri uri;
        private String filename;
        private String md5;
        private String description;
        private String launchargs;
        private Version version;

        internal Version Version
        {
            get { return this.version; }
        }

        internal string Filename
        {
            get { return this.filename; }
        }
       
        internal Uri Uri
        {
            get { return this.uri; }
        }

        internal string MD5
        {
            get { return this.md5; }
        }

        internal string Description
        {
            get { return this.description; }
        }

        //internal string Launchargs
        //{
        //    get { return this.Launchargs; }
        //}

        internal SharpUpdateXml(Version version, Uri uri, string filename, string md5, string description, string launchargs)
        {
            this.version = version;
            this.uri = uri;
            this.md5 = md5;
            this.launchargs = launchargs;
            this.filename = filename;
            this.description = description;
        }

        internal bool IsNewerThan(Version version)
        {
            return this.version > version;
        }

        internal static bool ExistsOnServer(Uri location)
        {
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(location.AbsoluteUri);
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                resp.Close();
                return resp.StatusCode == HttpStatusCode.OK;
            }
            catch (Exception)
            {

                return false;
            }
        }

        internal static SharpUpdateXml Parse(Uri location, string appID)
        {
            Version version = null;
            string url = "", filename = "", md5 = "", description = "", launchargs = "";

            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(location.AbsoluteUri);

                XmlNode node = doc.DocumentElement.SelectSingleNode("//update[@appId='" + appID + "']");

                if (node == null) return null;

                version = Version.Parse(node["version"].InnerText);
                url = node["url"].InnerText;
                filename = node["filename"].InnerText;
                md5 = node["md5"].InnerText;
                description = node["description"].InnerText;
                //launchargs = node["launchArgs"].InnerText;
                return new SharpUpdateXml(version, new Uri(url), filename, md5, description, launchargs);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
                //throw;
            }

        }

    }
}


