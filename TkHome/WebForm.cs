using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DotNet4.Utilities;

namespace TkHome
{
    public partial class WebForm : Form
    {
        public bool IsLogined = false;
        private string _loginURL = "https://login.taobao.com/member/login.jhtml?style=mini&newMini2=true&from=alimama&redirectURL=http%3A%2F%2Flogin.taobao.com%2Fmember%2Ftaobaoke%2Flogin.htm%3Fis_login%3d1&full_redirect=true";
        public WebForm()
        {
            InitializeComponent();
        }

        private void OnFormLoad(object sender, EventArgs e)
        {
            aliWebBrowser.Navigate(_loginURL);
        }

        private void OnDocumentComplete(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (Alimama.IsOnline())
            {
                string curURL = aliWebBrowser.Document.Url.ToString();

                if (curURL == Alimama._frontPage)
                {
                    Console.WriteLine("登录成功");
                    IsLogined = true;
                    this.Hide();
                }
            }
        }
    }
}
