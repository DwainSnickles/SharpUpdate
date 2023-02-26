using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SharpUpdate
{
    public partial class SharpUpdateInfoForm : Form
    {
        internal SharpUpdateInfoForm(ISharpUpdatable applicationInfo, SharpUpdateXml updateInfo)
        {
            InitializeComponent();

            if (applicationInfo.ApplicationIcon != null)
                this.Icon = applicationInfo.ApplicationIcon;

            this.Text = string.Format("Current Version = " + applicationInfo.ApplicationAssembly.GetName().Version.ToString());
            //+ " Update version = " + updateInfo.Version.ToString()); ;
            //this.Text = String.Format("Current Version: {0}\nUpdate Version[1}", applicationInfo.ApplicationAssembly.GetName().Version.ToString(),
            //    updateInfo.Version.ToString());

            //updateInfo.Description = "Update version = " + updateInfo.Version.ToString();

            this.txtDescription.Text = "Update version = " + updateInfo.Version.ToString();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
