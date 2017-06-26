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
using DotNet4.Utilities;
using System.Runtime.InteropServices;
using System.Web;
using System.Threading;
using System.Diagnostics;
using System.Net;

namespace WindowsFormsApplication1
{
    public delegate bool CallBack(int hwnd, int lParam); // 枚举窗口的回调函数

    public partial class Form1 : Form
    {
        private List<Site> SiteList = new List<Site>();
        private string loginURL1 = "https://login.taobao.com/member/login.jhtml?style=mini&newMini2=true&from=alimama&redirectURL=http%3A%2F%2Flogin.taobao.com%2Fmember%2Ftaobaoke%2Flogin.htm%3Fis_login%3d1&full_redirect=true";

        private string initCookies = "";
        private string ua = "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/45.0.2454.101 Safari/537.36";
        private int curPage = 1;
        private object insertImgLock = new object();
        private List<ProductItem> productList = new List<ProductItem>();
        private string searchText = "";

        void setCookies(string url, string cookies)
        {
            foreach (string c in cookies.Split(';'))
            {
                string[] item = c.Split('=');
                if (item.Length == 2)
                {
                    string strCookieName = HttpUtility.UrlEncode(item[0]).Replace("+", "");
                    string strCookieValue = HttpUtility.UrlEncode(item[1]);
                    Win32.InternetSetCookie(url, strCookieName, strCookieValue);
                }
            }
        }
        public Form1()
        {
            InitializeComponent();
            webBrowser1.ScriptErrorsSuppressed = true;
//             string cookies;
//             CookieSaver.loadCookies("cookies.txt", out cookies);
//             initCookies = cookies; // 记录初始cookie
//             setCookies(loginURL1, cookies);
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
                Debugger.Log(0, null, "siteid : " + id + "\t" + name.ToString());

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
                        Debugger.Log(0, null, "adzoneid : " + subid + "\t" + subname.ToString());
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

        private string GetCookies(string url)
        {
            try
            {
                uint datasize = 1024;
                StringBuilder cookieData = new StringBuilder((int)datasize);
                if (!Win32.InternetGetCookieEx(url, null, cookieData, ref datasize, 0x2000, IntPtr.Zero))
                {
                    if (datasize < 0)
                        return null;

                    cookieData = new StringBuilder((int)datasize);
                    if (!Win32.InternetGetCookieEx(url, null, cookieData, ref datasize, 0x00002000, IntPtr.Zero))
                        return null;
                }
//                 CookieSaver.saveCookies("cookies.txt", cookieData.ToString());
                return cookieData.ToString();
            }
            catch (Exception)
            {
                return initCookies;
            }
        }

        // 获取推广位
        private void getAdzoneEx()
        {
            string cookie = "";
            if (webBrowser1.Document == null)
            {
                cookie = initCookies;
            }
            else
            {
                cookie = GetCookies(webBrowser1.Document.Url.ToString());
            } 
            HttpItem GetJuItem = new HttpItem()
            {
                URL = "http://pub.alimama.com/common/adzone/newSelfAdzone2.json?tag=29&t=" + GetTimeStamp() + "000",
                ContentType = "application/x-www-form-urlencoded",
                Cookie = cookie,
                Accept = "*/*",
                UserAgent = ua
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
            Debugger.Log(0, null, "id = " + itemid);

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
                        Debugger.Log(0, null, "site id : " + site.id + ", adzone id : " + adzone.id);
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

            string cookie = "";
            if (webBrowser1.Document == null)
            {
                cookie = initCookies;
            }
            else
            {
                cookie = GetCookies(webBrowser1.Document.Url.ToString());
            } 
            HttpItem GetJuItem = new HttpItem()
            {
                URL = newURL,
                ContentType = "application/x-www-form-urlencoded",
                Cookie = cookie,
                Accept = "*/*",
                UserAgent = ua
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
                        Debugger.Log(0, null, "site id : " + site.id + ", adzone id : " + adzone.id);
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

            string cookie = "";
            if (webBrowser1.Document == null)
            {
                cookie = initCookies;
            }
            else
            {
                cookie = GetCookies(webBrowser1.Document.Url.ToString());
            } 
            HttpItem GetJuItem = new HttpItem()
            {
                URL = newURL,
                ContentType = "application/x-www-form-urlencoded",
                Cookie = cookie,
                Accept = "*/*",
                UserAgent = ua
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
            CallBack myCallBack = new CallBack(EnumWindowsAndSendMessage);
            Win32.EnumWindows(myCallBack, 0);
        }

        private bool EnumWindowsAndSendMessage(int hwnd, int param)
        {
            StringBuilder className = new StringBuilder(200);
            int len;
            len = Win32.GetClassName(hwnd, className, 200);
            if(len > 0)
            {
                if(className.ToString() == "TXGuiFoundation")
                {
                    StringBuilder wndName = new StringBuilder(200);
                    int nameLen;
                    nameLen = Win32.GetWindowText(hwnd, wndName, 200);
                    if (nameLen > 0)
                    {
                        string strWndName = wndName.ToString();
                        if (strWndName != "QQ" && strWndName != "TXMenuWindow")
                        {
                            Win32.SwitchToThisWindow(hwnd, 1); // 设为当前窗口
                            Thread.Sleep(100);

                            Clipboard.SetDataObject(pictureBox2.Image, true); // copy
                            uint WM_PASTE = 0x0302;
                            uint WM_KEYDOWN = 0x0100;
                            int VK_RETURN = 0x0D;
                            Win32.SendMessage(hwnd, WM_PASTE, 0, 0); // paste

                            Clipboard.SetDataObject(textBox2.Text); // copy
                            Win32.SendMessage(hwnd, WM_PASTE, 0, 0); // paste

                            Win32.SendMessage(hwnd, WM_KEYDOWN, VK_RETURN, 0); // send
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
                    Win32.SendMessage(hwnd, WM_LBUTTONDOWN, MK_LBUTTON, pos);
                    Thread.Sleep(10);
                    Win32.SendMessage(hwnd, WM_LBUTTONUP, 0, pos);

                    Win32.keybd_event(VK_CONTROL, 0, 0, 0);
                    Win32.SendMessage(hwnd, WM_KEYDOWN, 'V', 0);
                    Win32.SendMessage(hwnd, WM_KEYUP, 'V', 0);
                    Win32.keybd_event(VK_CONTROL, 0, KEYEVENTF_KEYUP, 0);

                    Clipboard.SetDataObject(textBox2.Text); // copy

                    // paster
                    Win32.SendMessage(hwnd, WM_LBUTTONDOWN, MK_LBUTTON, pos);
                    Thread.Sleep(10);
                    Win32.SendMessage(hwnd, WM_LBUTTONUP, 0, pos);

                    Win32.keybd_event(VK_CONTROL, 0, 0, 0);
                    Win32.SendMessage(hwnd, WM_KEYDOWN, 'V', 0);
                    Win32.SendMessage(hwnd, WM_KEYUP, 'V', 0);
                    Win32.keybd_event(VK_CONTROL, 0, KEYEVENTF_KEYUP, 0);

                    Win32.SendMessage(hwnd, WM_KEYDOWN, VK_RETURN, 0); // send
                }
            }
            return true;
        }

        public static DateTime GetTime(string timeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timeStamp + "0000000");
            TimeSpan toNow = new TimeSpan(lTime);
            return dtStart.Add(toNow);
        }
        public static string GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }  


        // 登录阿里妈妈
        private void button5_Click(object sender, EventArgs e)
        {
            OpenHook();
//             if (!IsOnline())
            {
                tabControl1.SelectedIndex = 1;
                webBrowser1.Navigate(loginURL1, "_self", null, "User-Agent: " + ua); // 跳转到登录页面           
            }
//             else
//             {
//                 Console.WriteLine("已是登录状态");
//                 string pageURL = "http://www.alimama.com/index.htm";
//                 setCookies(pageURL, initCookies);
//                 webBrowser1.Navigate(pageURL, "_self", null, "User-Agent: " + ua); // 跳转到首页          
//             }
            RefreshTimer.Start();
        }

        // 定时检测是否在线
        private void RefreshTimerTick(object sender, EventArgs e)
        {
            if (!IsOnline())
            {
                Console.WriteLine("检测到掉线 at " + DateTime.Now.ToString());
                Debugger.Log(0, null, "检测到掉线 at " + DateTime.Now.ToString());
                KeepLiveTimer.Stop();

//                 bLoginManual = false;
//                 webBrowser1.Navigate(loginURL2, "_self", null, "User-Agent: " + ua); // 跳转到登录页面           
            }
            else
            {
                Console.WriteLine("Timer refresh at " + DateTime.Now);
                Debugger.Log(0, null, "Timer refresh at " + DateTime.Now);
            }
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
                UserAgent = ua
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

        private bool IsOnline()
        {
            string cookie = "";
            if(webBrowser1.Document == null)
            {
                cookie = initCookies;
            }
            else
            {
                cookie = GetCookies(webBrowser1.Document.Url.ToString());
            }
            // 查询是否是登录状态
            HttpItem GetJuItem = new HttpItem()
            {
                 URL = "http://pub.alimama.com/common/getUnionPubContextInfo.json",
                ContentType = "application/x-www-form-urlencoded",
                Accept = "*/*",
                Cookie = cookie,
                UserAgent = ua
            };
            HttpHelper GetJuHelper = new HttpHelper();
            HttpResult GetJuResult = GetJuHelper.GetHtml(GetJuItem);
            string result = GetJuResult.Html;
            if (result.IndexOf("mmNick") == -1)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static readonly int MOUSEEVENTF_MOVE = 0x0001;
        public static readonly int MOUSEEVENTF_ABSOLUTE = 0x8000;
        public static readonly int MOUSEEVENTF_LEFTDOWN = 0x0002;//左键按下
        public static readonly int MOUSEEVENTF_LEFTUP = 0x0004;//左键抬起
        public static readonly int MOUSEEVENTF_RIGHTDOWN = 0x0008; //右键按下 
        public static readonly int MOUSEEVENTF_RIGHTUP = 0x0010; //右键抬起 
        public static readonly int MOUSEEVENTF_MIDDLEDOWN = 0x0020; //中键按下 
        public static readonly int MOUSEEVENTF_MIDDLEUP = 0x0040;// 中键抬起 

        [DllImport("User32.dll")]
        public static extern bool GetCursorPos(out Point pt);

        [DllImport("User32.dll", EntryPoint = "SetCursorPos")]
        public static extern void SetCursorPos(int x, int y);

        [DllImport("user32", EntryPoint = "mouse_event")]
        public static extern int mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        // WEB页面加载完成
        private void OnDocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            string curUrl = webBrowser1.Document.Url.ToString();
            Console.WriteLine(curUrl);
            Debugger.Log(0, null, curUrl);

            {
                if (IsOnline())
                {
                    string curURL = webBrowser1.Document.Url.ToString();

                    if (curURL == "https://www.alimama.com/index.htm")
                    {
                        Console.WriteLine("登录成功");
                        Debugger.Log(0, null, "登录成功");
                        Console.WriteLine("Keep Alive at " + DateTime.Now.ToString());
                        Debugger.Log(0, null, "Keep Alive at " + DateTime.Now.ToString());
                        KeepLiveTimer.Start();
                    }
                }
            }
        }
        private Rectangle GetAbsPos(HtmlElement em)
        {
            Rectangle rect = new Rectangle();
            Point p = new Point();
            string path = "";    //用于显示em的路径

            p.X = 0;
            p.Y = 0;

            while (em != null)
            {
                path += "<" + em.TagName;
                rect = em.OffsetRectangle;
                p.X += rect.X - em.ScrollLeft;
                p.Y += rect.Y - em.ScrollTop;
                em = em.OffsetParent;
            };
            rect.X = p.X;
            rect.Y = p.Y;
            return rect;
        }

        private void ScrollSlider()
        {
            HtmlElement nocaptcha = webBrowser1.Document.GetElementById("nocaptcha");
            if (nocaptcha.GetAttribute("classname").IndexOf("nc-tm-min-fix") != 0)
            {
                Console.WriteLine("出现滑动验证");
                HtmlElement nc_1_n1z = webBrowser1.Document.GetElementById("nc_1_n1z");
                Rectangle rect = GetAbsPos(nc_1_n1z);
                Point screenPoint = webBrowser1.PointToScreen(new Point(rect.X, rect.Y));
                SetCursorPos(screenPoint.X + 10, screenPoint.Y + 10);

                mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
                mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
                int scrollWidth = 0;
                Random ran = new Random();
                while(scrollWidth < 220)
                {
                    int rand = ran.Next(5, 10);
                    mouse_event(MOUSEEVENTF_MOVE, rand, 0, 0, 0);
                    scrollWidth += rand;
                    Thread.Sleep(50);
                }
                mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
            }
        }

        // 保存cookie
        private void saveCookieBtn_Click(object sender, EventArgs e)
        {
            string cookie = GetCookies(webBrowser1.Document.Url.ToString());
            Console.WriteLine(cookie);
        }

        private void stepOne()
        {
            string url = "https://login.taobao.com/aso/tgs?domain=alimama&sign_account=98aadb2a2067ad35a64b078fe6075b13&service=user_on_taobao&target=";
            string cookie = "";
            if (webBrowser1.Document == null)
            {
                cookie = initCookies;
            }
            else
            {
                cookie = GetCookies(url);
            }
            // 查询是否是登录状态
            HttpItem GetJuItem = new HttpItem()
            {
                URL = url,
                ContentType = "application/x-www-form-urlencoded",
                Accept = "*/*",
                Cookie = cookie,
                UserAgent = ua
            };
            HttpHelper GetJuHelper = new HttpHelper();
            HttpResult GetJuResult = GetJuHelper.GetHtml(GetJuItem);
            if (GetJuResult.StatusCode == HttpStatusCode.Found)
            {
                // 重定向
                string strNewURL = GetJuResult.Header["Location"];
                cookie = GetCookies(strNewURL);

                HttpItem GetJuItem2 = new HttpItem()
                {
                    URL = strNewURL,
                    ContentType = "application/x-www-form-urlencoded",
                    Accept = "*/*",
                    Cookie = cookie,
                    UserAgent = ua
                };
                HttpHelper GetJuHelper2 = new HttpHelper();
                HttpResult GetJuResult2 = GetJuHelper2.GetHtml(GetJuItem2);
                string strCookie = GetJuResult2.Cookie;
//                 setCookies(strNewURL, strCookie);
                string result2 = GetJuResult2.Html;
            }
        }
        private void stepTwo()
        {
            string cookie = "";
            if (webBrowser1.Document == null)
            {
                cookie = initCookies;
            }
            else
            {
                cookie = GetCookies(webBrowser1.Document.Url.ToString());
            }
            // 查询是否是登录状态
            HttpItem GetJuItem = new HttpItem()
            {
                URL = "http://www.alimama.com/index.htm",
                ContentType = "application/x-www-form-urlencoded",
                Accept = "*/*",
                Cookie = cookie,
                UserAgent = ua
            };
            HttpHelper GetJuHelper = new HttpHelper();
            HttpResult GetJuResult = GetJuHelper.GetHtml(GetJuItem);
            string result = GetJuResult.Html;
        }

        private void stepThree()
        {
            string cookie = "";
            if (webBrowser1.Document == null)
            {
                cookie = initCookies;
            }
            else
            {
                cookie = GetCookies(webBrowser1.Document.Url.ToString());
            }
            // 查询是否是登录状态
            HttpItem GetJuItem = new HttpItem()
            {
                URL = "http://www.alimama.com/getLogInfo.htm?callback=__jp0",
                ContentType = "application/x-www-form-urlencoded",
                Accept = "*/*",
                Cookie = cookie,
                UserAgent = ua
            };
            HttpHelper GetJuHelper = new HttpHelper();
            HttpResult GetJuResult = GetJuHelper.GetHtml(GetJuItem);
            string result = GetJuResult.Html;
        }
        private void OnKeepLiveTimer(object sender, EventArgs e)
        {
            KeepLive();
        }

        private void KeepAliveBtn_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Keep Alive at " + DateTime.Now.ToString());
            KeepLiveTimer.Start();
        }

        [DllImport("OpenHook.dll")]
        public static extern void OpenHook();

        [DllImport("OpenHook.dll")]
        public static extern void KeepLive();

        private void HookButton_Click(object sender, EventArgs e)
        {
            OpenHook();
        }

        // 还原淘客链接到原始链接
        private string ParseTkURL(string tkURL)
        {
            string strOrignalURL = ""; // 原始链接
            if (tkURL.IndexOf("https://s.click.taobao.com") == 0)
            {
                // 是淘客链接
                HttpItem GetJuItem = new HttpItem() // 第一次跳转
                {
                    URL = tkURL,
                    ContentType = "application/x-www-form-urlencoded",
                    Accept = "*/*",
                    UserAgent = ua
                };
                HttpHelper GetJuHelper = new HttpHelper();
                HttpResult GetJuResult = GetJuHelper.GetHtml(GetJuItem);
                if (GetJuResult.StatusCode == HttpStatusCode.Found)
                {
                    // 重定向
                    string strNewURL = GetJuResult.Header["Location"];
                    
                    HttpItem GetJuItem2 = new HttpItem() // 第二次跳转
                    {
                        URL = strNewURL,
                        ContentType = "application/x-www-form-urlencoded",
                        Accept = "*/*",
                        UserAgent = ua
                    };
                    HttpHelper GetJuHelper2 = new HttpHelper();
                    HttpResult GetJuResult2 = GetJuHelper2.GetHtml(GetJuItem2);
                    if (GetJuResult2.StatusCode == HttpStatusCode.Found)
                    {
                        string strNewURL2 = GetJuResult2.Header["Location"];

                        // 解析出tu
                        int pos = strNewURL2.IndexOf("?tu=");
                        if (pos != -1)
                        {
                            string tuURL = strNewURL2.Substring(pos + 4);
                            string decURL = HttpUtility.UrlDecode(tuURL); // 解码

                            HttpItem GetJuItem3 = new HttpItem() // 第三次跳转
                            {
                                URL = decURL,
                                ContentType = "application/x-www-form-urlencoded",
                                Accept = "*/*",
                                UserAgent = ua,
                                Referer = strNewURL2
                            };
                            HttpHelper GetJuHelper3 = new HttpHelper();
                            HttpResult GetJuResult3 = GetJuHelper3.GetHtml(GetJuItem3);
                            if (GetJuResult3.StatusCode == HttpStatusCode.Found)
                            {
                                string strNewURL3 = GetJuResult3.Header["Location"];
                                int pos2 = strNewURL3.IndexOf("&ali_trackid");

                                strOrignalURL = strNewURL3.Substring(0, pos2);
                            }
                        }
                    }

                }
            }
            return strOrignalURL;
        }

        // 从聊天内容中解析出链接
        private void ParseChatContent(string content)
        {
            string signal = "https://s.click.taobao.com/";
            int pos = content.IndexOf(signal);
            while(pos != -1)
            {
                if (content.Length >= signal.Length + 7)
                {
                    string url = content.Substring(pos, signal.Length + 7);
                    content = content.Substring(pos + signal.Length + 7);

                    pos = content.IndexOf(signal);

                    string strOriginURL = ParseTkURL(url);
                    ListViewItem lvi = listView1.Items.Add(url);
                    lvi.SubItems.Add(strOriginURL);
                }
            }
        }

        private bool EnumWindowsAndGetContent(int hwnd, int param)
        {
            StringBuilder className = new StringBuilder(200);
            int len;
            len = Win32.GetClassName(hwnd, className, 200);
            if (len > 0)
            {
                if (className.ToString() == "TXGuiFoundation")
                {
                    StringBuilder wndName = new StringBuilder(200);
                    int nameLen;
                    nameLen = Win32.GetWindowText(hwnd, wndName, 200);
                    if (nameLen > 0)
                    {
                        string strWndName = wndName.ToString();
                        if (strWndName != "QQ" && strWndName != "TXMenuWindow")
                        {
                            // QQ窗口
                            QqWindowHelper a = new QqWindowHelper((IntPtr)hwnd);
                            textBox3.Text += a.GetContent();
                            ParseChatContent(a.GetContent());
                            textBox3.Text += "\n\n\n";
                        }
                    }
                }
                else if (className.ToString() == "ChatWnd")
                {
                    // 微信窗口
                }
            }
            return true;
        }

        // 监控QQ，微信聊天信息
        private void button3_Click(object sender, EventArgs e)
        {
            textBox3.Text = "";
            listView1.Items.Clear();

            CallBack myCallBack = new CallBack(EnumWindowsAndGetContent);
            Win32.EnumWindows(myCallBack, 0);
        }

        private void OnDoubleClickURL(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                Clipboard.SetDataObject(listView1.SelectedItems[0].SubItems[1].Text); // copy
                Console.WriteLine(listView1.SelectedItems[0].SubItems[1].Text);
            }
        }

        public static string ReplaceHtmlTag(string html, int length = 0)
        {
            string strText = System.Text.RegularExpressions.Regex.Replace(html, "<[^>]+>", "");
            strText = System.Text.RegularExpressions.Regex.Replace(strText, "&[^;]+;", "");

            if (length > 0 && strText.Length > length)
                return strText.Substring(0, length);

            return strText;
        }

        public void GetResponseCallBack(IAsyncResult ia)
        {
            ProductListData data = ia.AsyncState as ProductListData;

            HttpWebResponse resp = data.req.EndGetResponse(ia) as HttpWebResponse;
            Image img1 = Image.FromStream(resp.GetResponseStream());

            ///线程锁
            lock (insertImgLock)
            {
                productList.Add(new ProductItem(data.name, img1));
            }
        }
        // 解析产品列表
        private void parseProducts(String jsonText)
        {
            JObject jp = (JObject)JsonConvert.DeserializeObject(jsonText);
            JArray items = (JArray)jp["data"]["pageList"];
            foreach (var item in items)
            {
                var title = ((JObject)item)["title"];
                var picURL = ((JObject)item)["pictUrl"];
                var Sale30 = ((JObject)item)["biz30day"];
                var tkRate = ((JObject)item)["tkRate"];
                var tkCommFee = ((JObject)item)["tkCommFee"];
                var zkPrice = ((JObject)item)["zkPrice"];
                var auctionUrl = ((JObject)item)["auctionUrl"];

                // 异步下载图片
                HttpWebRequest pReq = (HttpWebRequest)WebRequest.Create(new Uri("http:" + picURL));
                ProductListData data = new ProductListData();
                data.name = picURL.ToString();
                data.req = pReq;
                pReq.BeginGetResponse(new AsyncCallback(GetResponseCallBack), data);

                string strTitle = HttpUtility.HtmlDecode(title.ToString());
                strTitle = ReplaceHtmlTag(strTitle);
                ListViewItem lv = listView2.Items.Add(strTitle);
                lv.SubItems.Add(zkPrice.ToString());
                lv.SubItems.Add(tkRate.ToString() + "%");
                lv.SubItems.Add(tkCommFee.ToString());
                lv.SubItems.Add(Sale30.ToString());
                lv.SubItems.Add(auctionUrl.ToString());
                lv.ImageKey = picURL.ToString();
            }
        }
        // 加载产品库
        private void LoadProducts()
        {
            listView2.Items.Clear();
            imageList1.Images.Clear();

//             strURL = "http://pub.alimama.com/items/search.json?spm=a219t.7664554.1998457203.dfb730492.J3mH3D&toPage=" + curPage.ToString() + "&queryType=2&auctionTag=&perPageSize=40";
            string strURL = "http://pub.alimama.com/items/search.json";
            if (searchText != "")
            {
                strURL += "?q=" + HttpUtility.UrlEncode(searchText) + "&";
            }
            else{
                strURL += "?";
            }
            strURL += "toPage=" + curPage.ToString() + "&queryType=2&auctionTag=&perPageSize=40";
            HttpItem GetJuItem = new HttpItem()
            {
                URL = strURL,
                ContentType = "application/x-www-form-urlencoded",
                Accept = "*/*",
                UserAgent = ua
            };
            HttpHelper GetJuHelper = new HttpHelper();
            HttpResult GetJuResult = GetJuHelper.GetHtml(GetJuItem);
            string result = GetJuResult.Html;
            parseProducts(result);
        }

        private void OnFormLoad(object sender, EventArgs e)
        {
            LoadProducts();
            LoadImgTimer.Start();
        }

        private void OnLoadImgTimer(object sender, EventArgs e)
        {
            lock (insertImgLock)
            {
                if (productList.Count != 0)
                {
                    foreach (ProductItem item in productList)
                    {
                        imageList1.Images.Add(item._name, item._img);
                    }
                    productList.Clear();
                }
            }

        }

        private void UpPageButton_Click(object sender, EventArgs e)
        {
            if (curPage > 1)
            {
                curPage--;
                LoadProducts();
                label5.Text = curPage.ToString();
            }
        }

        private void DownPageButton_Click(object sender, EventArgs e)
        {
            curPage++;
            LoadProducts();
            label5.Text = curPage.ToString();
        }

        // 搜索产品库
        private void SearchButton_Click(object sender, EventArgs e)
        {
            searchText = textBox4.Text;
            curPage = 1;
            LoadProducts();
            label5.Text = curPage.ToString();
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
