using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using HttpHelper_namespace;
using System.Runtime.InteropServices;
using System.Web;
using System.Threading;

namespace WindowsFormsApplication1
{
    public delegate bool CallBack(int hwnd, int lParam); // 枚举窗口的回调函数

    public partial class Form1 : Form
    {
        private string current_cookie;
        private List<Site> SiteList = new List<Site>();
        public Form1()
        {
            InitializeComponent();
            webBrowser1.ScriptErrorsSuppressed = true;
            webBrowser1.Navigate("https://login.taobao.com/member/login.jhtml?style=mini&newMini2=true&css_style=alimama&from=alimama&redirectURL=http%3A%2F%2Fwww.alimama.com&full_redirect=true&disableQuickLogin=true");
        }

        private string getJson1()
        {
            string path = @"C:\Users\sky\Documents\json1.txt";
            StreamReader sr = new StreamReader(path, Encoding.Default);
            String line = sr.ReadLine();
            return line;
        }

        private string getJson2()
        {
            string path = @"C:\Users\sky\Documents\json2.txt";
            StreamReader sr = new StreamReader(path, Encoding.Default);
            String line = sr.ReadLine();
            return line;
        }

        private void getShortLinkURL()
        {
            string jsonText = getJson1();
            JObject jp1 = (JObject)JsonConvert.DeserializeObject(jsonText);
            textBox1.Text = jp1["data"]["shortLinkUrl"].ToString();
        }

        private string utf8_unicode(string utf8String)
        {
            byte[] buffer1 = Encoding.Default.GetBytes(utf8String);
            byte[] buffer2 = Encoding.Convert(Encoding.UTF8, Encoding.Default, buffer1, 0, buffer1.Length);
            string strBuffer = Encoding.Default.GetString(buffer2, 0, buffer2.Length);
            return strBuffer;
        }

        private void parseAdzone(String jsonText)
        {
            JObject jp2 = (JObject)JsonConvert.DeserializeObject(jsonText);
            JArray adzones = (JArray)jp2["data"]["otherAdzones"];
            foreach (var item in adzones)
            {
                var id = ((JObject)item)["id"];
                var name = ((JObject)item)["name"];

                Console.WriteLine("siteid : " + id + "\t" + name.ToString());

                Site site = new Site();
                site.id = id.ToString();
                site.name = name.ToString();
                site.zones = new List<Adzone>();

                try
                {
                    JArray subs = (JArray)((JObject)item)["sub"];
                    foreach (var subitem in subs)
                    {
                        var subid = ((JObject)subitem)["id"];
                        var subname = ((JObject)subitem)["name"];
                        Console.WriteLine("adzoneid : " + subid + "\t" + subname.ToString());
                        comboBox1.Items.Add(subname.ToString());
                        comboBox1.SelectedIndex = 0;

                        Adzone adzone = new Adzone();
                        adzone.id = subid.ToString();
                        adzone.name = subname.ToString();
                        site.zones.Add(adzone);
                    }
                }
                catch (Exception)
                {

                }

                SiteList.Add(site);
            }
        }

        private void getAdzone()
        {
            string jsonText = getJson2();
            parseAdzone(jsonText);
        }

        [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool InternetGetCookieEx(string pchURL, string pchCookieName, StringBuilder pchCookieData, ref System.UInt32 pcchCookieData, int dwFlags, IntPtr lpReserved);
        private static string GetCookies(string url)
        {
            uint datasize = 1024;
            StringBuilder cookieData = new StringBuilder((int)datasize);
            if (!InternetGetCookieEx(url, null, cookieData, ref datasize, 0x2000, IntPtr.Zero))
            {
                if (datasize < 0)
                    return null;

                cookieData = new StringBuilder((int)datasize);
                if (!InternetGetCookieEx(url, null, cookieData, ref datasize, 0x00002000, IntPtr.Zero))
                    return null;
            }
            return cookieData.ToString();
        }

        private void getAdzoneEx()
        {
            string cookie = GetCookies(webBrowser1.Document.Url.ToString());
            HttpItem GetJuItem = new HttpItem()
            {
                URL = "http://pub.alimama.com/common/adzone/newSelfAdzone2.json?tag=29&t=1433864026366",
                ContentType = "application/x-www-form-urlencoded",
                Referer = "http://pub.alimama.com/common/adzone/newSelfAdzone2.json?tag=29&t=1433864026366",
                Cookie = cookie,
                Accept = "*/*",
                UserAgent = "Mozilla/4.0 (compatible; MSIE 9.0; Windows NT 6.1)"
            };
            HttpHelper GetJuHelper = new HttpHelper();
            HttpResult GetJuResult = GetJuHelper.GetHtml(GetJuItem);
            string result = GetJuResult.Html;

            parseAdzone(result);
        }

        private string getIdFromURL(string url)
        {
            int pos = url.IndexOf("&id=");
            if (pos == -1)
            {
                pos = url.IndexOf("?id=");
            }
            if (pos == -1)
            {
                return "";
            }

            string id = url.Substring(pos + 4);
            pos = id.IndexOf("&");
            if (pos != -1)
            {
                id = id.Substring(0, pos);
            }
            return id;
        }

        // 获取下单链接
        private void getOrderURL(string product_url, out string order_url, out string coupon_token)
        {
            string itemid = getIdFromURL(product_url); // 商品id
            Console.WriteLine("id = " + itemid);

            // 得到当前选择的siteid和adzoneid
            string siteid = "", adzoneid = "";
            string selected_adzone = comboBox1.Text;
            foreach (Site site in SiteList)
            {
                bool bFind = false;
                foreach (Adzone adzone in site.zones)
                {
                    if (adzone.name == selected_adzone)
                    {
                        Console.WriteLine("site id : " + site.id + ", adzone id : " + adzone.id);
                        siteid = site.id;
                        adzoneid = adzone.id;
                        bFind = true;
                        break;
                    }
                }
                if (bFind)
                {
                    break;
                }
            }

            // 转链
            string newURL = "http://pub.alimama.com/common/code/getAuctionCode.json?";
            newURL += "auctionid=" + itemid;
            newURL += "&adzoneid=" + adzoneid;
            newURL += "&siteid=" + siteid;
            newURL += "&scenes=1&t=1445487172579&_input_charset=utf-8";

            string cookie = GetCookies(webBrowser1.Document.Url.ToString());
            HttpItem GetJuItem = new HttpItem()
            {
                URL = newURL,
                ContentType = "application/x-www-form-urlencoded",
                Cookie = cookie,
                Accept = "*/*",
                UserAgent = "Mozilla/4.0 (compatible; MSIE 9.0; Windows NT 6.1)"
            };
            HttpHelper GetJuHelper = new HttpHelper();
            HttpResult GetJuResult = GetJuHelper.GetHtml(GetJuItem);
            string result = GetJuResult.Html;

            JObject jp1 = (JObject)JsonConvert.DeserializeObject(result);
            order_url = jp1["data"]["shortLinkUrl"].ToString();
            coupon_token = jp1["data"]["taoToken"].ToString();
        }

        // 获取优惠券地址
        private void getCouponURL(string product_url, out string order_url, out string coupon_url, out string coupon_token)
        {
            string itemid = getIdFromURL(product_url); // 商品id

            // 得到当前选择的siteid和adzoneid
            string siteid = "", adzoneid = "";
            string selected_adzone = comboBox1.Text;
            foreach (Site site in SiteList)
            {
                bool bFind = false;
                foreach (Adzone adzone in site.zones)
                {
                    if (adzone.name == selected_adzone)
                    {
                        Console.WriteLine("site id : " + site.id + ", adzone id : " + adzone.id);
                        siteid = site.id;
                        adzoneid = adzone.id;
                        bFind = true;
                        break;
                    }
                }
                if (bFind)
                {
                    break;
                }
            }

            // 转链
            string newURL = "http://pub.alimama.com/common/code/getAuctionCode.json?";
            newURL += "auctionid=" + itemid;
            newURL += "&adzoneid=" + adzoneid;
            newURL += "&siteid=" + siteid;
            newURL += "&scenes=3&channel=tk_qqhd";

            string cookie = GetCookies(webBrowser1.Document.Url.ToString());
            HttpItem GetJuItem = new HttpItem()
            {
                URL = newURL,
                ContentType = "application/x-www-form-urlencoded",
                Cookie = cookie,
                Accept = "*/*",
                UserAgent = "Mozilla/4.0 (compatible; MSIE 9.0; Windows NT 6.1)"
            };
            HttpHelper GetJuHelper = new HttpHelper();
            HttpResult GetJuResult = GetJuHelper.GetHtml(GetJuItem);
            string result = GetJuResult.Html;

            JObject jp1 = (JObject)JsonConvert.DeserializeObject(result);
            coupon_url = jp1["data"]["couponLink"].ToString();
            order_url = jp1["data"]["shortLinkUrl"].ToString();
            coupon_token = jp1["data"]["couponLinkTaoToken"].ToString();
        }

        // 获取
        private void button1_Click(object sender, EventArgs e)
        {
            getAdzoneEx();
        }

        // 群发
        private void button2_Click(object sender, EventArgs e)
        {
            CallBack myCallBack = new CallBack(EnumWindowsProc);
            EnumWindows(myCallBack, 0);
        }

        // 引用3个API
        [DllImport("user32")]
        public static extern int EnumWindows(CallBack x, int y);

        [DllImport("user32.dll")]
        public static extern int GetClassName(int hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        public static extern int GetWindowText(int hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        public static extern int SendMessage(int hWnd, uint msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo);

        private bool EnumWindowsProc(int hwnd, int param)
        {
            StringBuilder className = new StringBuilder(200);
            int len;
            len = GetClassName(hwnd, className, 200);
            if(len > 0)
            {
                if(className.ToString() == "TXGuiFoundation")
                {
                    StringBuilder wndName = new StringBuilder(200);
                    int nameLen;
                    nameLen = GetWindowText(hwnd, wndName, 200);
                    if (nameLen > 0)
                    {
                        string strWndName = wndName.ToString();
                        if (strWndName != "QQ" && strWndName != "TXMenuWindow")
                        {
                            Clipboard.SetDataObject(pictureBox2.Image, true); // copy
                            uint WM_PASTE = 0x0302;
                            uint WM_KEYDOWN = 0x0100;
                            int VK_RETURN = 0x0D;
                            SendMessage(hwnd, WM_PASTE, 0, 0); // paste

                            Clipboard.SetDataObject(textBox2.Text); // copy
                            SendMessage(hwnd, WM_PASTE, 0, 0); // paste

                            SendMessage(hwnd, WM_KEYDOWN, VK_RETURN, 0); // send
                        }
                    }
                }
                else if (className.ToString() == "ChatWnd")
                {
                    uint WM_LBUTTONDOWN = 0x0201;
                    uint WM_LBUTTONUP = 0x0202;
                    uint WM_KEYDOWN = 0x0100;
                    uint WM_KEYUP = 0x0101;
                    int MK_LBUTTON = 1;
                    byte VK_CONTROL = 0x11;
                    uint KEYEVENTF_KEYUP = 2;
                    int pos = 0x023a0113; // 275, 570
                    int VK_RETURN = 0x0D;

                    Clipboard.SetDataObject(pictureBox2.Image, true); // copy

                    // paster
                    SendMessage(hwnd, WM_LBUTTONDOWN, MK_LBUTTON, pos);
                    Thread.Sleep(10);
                    SendMessage(hwnd, WM_LBUTTONUP, 0, pos);

                    keybd_event(VK_CONTROL, 0, 0, 0);
                    SendMessage(hwnd, WM_KEYDOWN, 'V', 0);
                    SendMessage(hwnd, WM_KEYUP, 'V', 0);
                    keybd_event(VK_CONTROL, 0, KEYEVENTF_KEYUP, 0);

                    Clipboard.SetDataObject(textBox2.Text); // copy

                    // paster
                    SendMessage(hwnd, WM_LBUTTONDOWN, MK_LBUTTON, pos);
                    Thread.Sleep(10);
                    SendMessage(hwnd, WM_LBUTTONUP, 0, pos);

                    keybd_event(VK_CONTROL, 0, 0, 0);
                    SendMessage(hwnd, WM_KEYDOWN, 'V', 0);
                    SendMessage(hwnd, WM_KEYUP, 'V', 0);
                    keybd_event(VK_CONTROL, 0, KEYEVENTF_KEYUP, 0);

                    SendMessage(hwnd, WM_KEYDOWN, VK_RETURN, 0); // send
                }
            }
            return true;
        }

        // 登录阿里妈妈
        private void button5_Click(object sender, EventArgs e)
        {
//             HtmlElement nameText = webBrowser1.Document.GetElementById("TPL_username_1");
//             HtmlElement passwordText = webBrowser1.Document.GetElementById("TPL_password_1");
//             nameText.InnerText = UserNameTextBox.Text;
//             passwordText.InnerText = PasswordTextBox.Text;
// 
//             HtmlElement submitButton = webBrowser1.Document.GetElementById("J_SubmitStatic");
//             submitButton.InvokeMember("click");

            RefreshTimer.Start();
        }

        // 定时器
        private void RefreshTimerTick(object sender, EventArgs e)
        {
            webBrowser1.Navigate("http://media.alimama.com/account/overview.htm");
            Thread.Sleep(5000);
            webBrowser1.Navigate("http://www.alimama.com/index.htm");
            Console.WriteLine("Timer refresh");
        }

        // 查询商品
        private void button6_Click(object sender, EventArgs e)
        {
            string QueryURL = "http://pub.alimama.com/items/search.json?q=";
            QueryURL += System.Web.HttpUtility.UrlEncode(textBox1.Text);
            HttpItem GetJuItem = new HttpItem()
            {
                URL = QueryURL,
                ContentType = "application/x-www-form-urlencoded",
                Accept = "*/*",
                UserAgent = "Mozilla/4.0 (compatible; MSIE 9.0; Windows NT 6.1)"
            };
            HttpHelper GetJuHelper = new HttpHelper();
            HttpResult GetJuResult = GetJuHelper.GetHtml(GetJuItem);
            string result = GetJuResult.Html;

            try
            {
                JObject jp1 = (JObject)JsonConvert.DeserializeObject(result);
                string title = jp1["data"]["pageList"][0]["title"].ToString();
                string price = jp1["data"]["pageList"][0]["zkPrice"].ToString();
                string coupon = jp1["data"]["pageList"][0]["couponAmount"].ToString();
                string pictureURL = jp1["data"]["pageList"][0]["pictUrl"].ToString();

                pictureURL = "http:" + pictureURL;
                pictureBox2.ImageLocation = pictureURL; // picture

                string TotalText = "【商品】 " + title + "\r\n";
                if (coupon == "0")
                {
                    // 没有优惠券
                    TotalText += "【价格】 " + price + "元！\r\n";
                    string order_url, coupon_token;
                    getOrderURL(textBox1.Text, out order_url, out coupon_token);
                    TotalText += "【口令】 " + coupon_token + "\r\n";
                    TotalText += "【下单】 " + order_url + "\r\n";
                }
                else
                {
                    // 有优惠券
                    TotalText += "【原价】 " + price + "元！\r\n";
                    TotalText += "【现价】 " + (float.Parse(price) - float.Parse(coupon)).ToString() + "秒杀！\r\n";
                    string order_url, coupon_url, coupon_token;
                    getCouponURL(textBox1.Text, out order_url, out coupon_url, out coupon_token);
                    TotalText += "【领券】 " + coupon_url + "\r\n";
                    TotalText += "【口令】 " + coupon_token + "\r\n";
                    TotalText += "【下单】 " + order_url + "\r\n";
                }
                textBox2.Text = TotalText;
            }
            catch (Exception)
            {

            }
        }
    }

    class Site
    {
        public string id;
        public string name;
        public List<Adzone> zones;
    }

    class Adzone
    {
        public string id;
        public string name;
    }
}
