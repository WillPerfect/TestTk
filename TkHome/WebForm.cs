﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DotNet4.Utilities;
using System.Diagnostics;

namespace TkHome
{
    public partial class WebForm : Form
    {
        public bool IsLogined = false;
        private string _loginURL = "https://login.taobao.com/member/login.jhtml?style=mini&newMini2=true&from=alimama&redirectURL=http%3A%2F%2Flogin.taobao.com%2Fmember%2Ftaobaoke%2Flogin.htm%3Fis_login%3d1&full_redirect=true";
        private bool _bClicked = false;
        private DateTime _lastLoginedTime = new DateTime(); // 上次登录上的时间
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
                    Debug.WriteLine("登录成功");
                    _lastLoginedTime = DateTime.Now;
                    IsLogined = true;
                    this.Hide();
                    detectLoginTimer.Start();
                }
            }
            else
            {
                if (!_bClicked)
                {
                    loginTimer.Start();
                }
            }
            Debug.WriteLine("OnDocumentComplete " + aliWebBrowser.Document.Url.ToString());
        }

        private void OnDetectLoginTimer(object sender, EventArgs e)
        {
            if (!Alimama.IsOnline())
            {
                Debug.WriteLine("检测到掉线 " + DateTime.Now.ToString());

                detectLoginTimer.Stop();
                this.Show();
                _bClicked = false;
                aliWebBrowser.Navigate(_loginURL);
            }
            else
            {
                TimeSpan delta = DateTime.Now - _lastLoginedTime;
                if (delta.TotalMinutes > 60)
                {
                    detectLoginTimer.Interval = 60 * 1000; // 1分钟                    
                }
                else
                {
                    detectLoginTimer.Interval = 10 * 60 * 1000; // 10分钟
                }
                Debug.WriteLine("依旧在线 " + DateTime.Now.ToString());
            }
        }

        private void OnLoginTimer(object sender, EventArgs e)
        {
            HtmlElement he = aliWebBrowser.Document.GetElementById("J_SubmitQuick");
            if (he != null)
            {
                he.InvokeMember("click");
                _bClicked = true;
                Debug.WriteLine("Click");
            }
            loginTimer.Stop();
        }
    }
}
