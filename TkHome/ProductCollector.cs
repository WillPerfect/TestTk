using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Web;
using DotNet4.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

// 产品收集类
namespace TkHome
{
    class WndInfo
    {
        public string Name { get; set; }
        public int Wnd { get; set; }

        public bool IsQQWnd { get; set; }
        public WndInfo(string name, int wnd, bool isQQWnd)
        {
            Name = name;
            Wnd = wnd;
            IsQQWnd = isQQWnd;
        }
    }

    class CollectURL
    {
        public string URL { get; set; }
        public string Title { get; set; }
        public string Time { get; set; }

        public string OriginURL { get; set; }
        public string Sale30 { get; set; }
        public string TkRate { get; set; }
        public string TkCommFee { get; set; }
        public string ZkPrice { get; set; }
    }

    class ProductCollector
    {
        private List<WndInfo> _qqQunList = new List<WndInfo>();
        private List<int> _monitorQQWndList = new List<int>(); // 当前正在监控的QQ窗口列表
        private object _collectURLLock = new object();
        private Thread _monitorThread; // 监控QQ群线程
        public bool _stopThread = false;
        private List<CollectURL> _collectURLList = new List<CollectURL>();
        private List<string> _parsedURLList = new List<string>(); // 已经解析过的URL列表，不需要重复解析
        private List<CollectURL> _failedURLList = new List<CollectURL>(); // 超级搜索失败的URL列表，后续重试
        private List<string> _tkURLList = new List<string>(); // 待解析的淘客URL列表
        private object _tkURLLock = new object();
        private Thread _parseTkURLThread; // 解析URL线程

        private int _startTime = 9;
        private int _endTime = 21;
        private int _interval = 5;

        public List<WndInfo> GetAllQQQunWnd()
        {
            _qqQunList.Clear();
            EnumWindowsCallBack myCallBack = new EnumWindowsCallBack(EnumWindowsAndGetContent);
            Win32.EnumWindows(myCallBack, 0);
            return _qqQunList;
        }

        // 开始监控
        public void StartMonitor(List<int> qqWndList, int startTime, int endTime, int interval)
        {
            _monitorQQWndList = qqWndList;
            _startTime = startTime;
            _endTime = endTime;
            _interval = interval;
            _stopThread = false;
            _monitorThread = new Thread(MonitorThreadProc);
            _monitorThread.Start(this);
            _parseTkURLThread = new Thread(ParseURLThread);
            _parseTkURLThread.Start(this);
        }

        // 停止监控
        public void StopMonitor()
        {
            _stopThread = true;
            if (_parseTkURLThread != null && _parseTkURLThread.IsAlive)
            {
                _parseTkURLThread.Join();
            }
            if (_monitorThread != null && _monitorThread.IsAlive)
            {
                _monitorThread.Join();                
            }
        }

        // 获取监控数据
        public List<CollectURL> GetCollectedURL()
        {
            List<CollectURL> collectURList = new List<CollectURL>();
            lock (_collectURLLock)
            {
                _collectURLList.ForEach(t => collectURList.Add(t));
                _collectURLList.Clear();
            }
            return collectURList;
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
                            _qqQunList.Add(new WndInfo(strWndName, hwnd, true));
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

        // 从聊天内容中解析出链接
        private void ParseChatContent(string content)
        {
            string signal = "https://s.click.taobao.com/";
            int pos = content.IndexOf(signal);
            while (pos != -1)
            {
                if (content.Length >= signal.Length + 7)
                {
                    string url = content.Substring(pos, signal.Length + 7);
                    content = content.Substring(pos + signal.Length + 7);

                    pos = content.IndexOf(signal);

                    if (_parsedURLList.IndexOf(url) == -1) // 没有解析过
                    {
                        lock (_tkURLLock)
                        {
                            _parsedURLList.Add(url); // 添加到待解析列表中去
                        }
                    }
                }
                if (_stopThread)
                {
                    break;
                }
            }
        }

        private static void MonitorThreadProc(object o)
        {
            DateTime lastCollectTime = new DateTime();
            ProductCollector colllector = o as ProductCollector;
            while (!colllector._stopThread)
            {
                DateTime nowTime = DateTime.Now;
                TimeSpan delta = nowTime - lastCollectTime;
                if (delta.TotalSeconds > colllector._interval * 60 && nowTime.Hour >= colllector._startTime && nowTime.Hour < colllector._endTime)
                {
                    Debug.WriteLine("开始采集");
                    // 先遍历QQ窗口列表
                    foreach (int item in colllector._monitorQQWndList)
                    {
                        QqWindowHelper a = new QqWindowHelper((IntPtr)item);
                        string strContent = a.GetContent();
                        colllector.ParseChatContent(a.GetContent());
                        if (colllector._stopThread)
                        {
                            break;
                        }
                    }

                    // 再重试之前失败的
                    colllector.RetryFailedURLList();

                    lastCollectTime = nowTime;
                }
                
                if (colllector._stopThread)
                {
                    break;
                }
                Thread.Sleep(1000);
            }
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
                };
                HttpHelper GetJuHelper = new HttpHelper();
                HttpResult GetJuResult = GetJuHelper.GetHtml(GetJuItem);
                if (GetJuResult.StatusCode == HttpStatusCode.Found)
                {
                    // 重定向
                    string strNewURL = GetJuResult.Header["Location"];

                    if (_stopThread)
                    {
                        return "";
                    }
                    HttpItem GetJuItem2 = new HttpItem() // 第二次跳转
                    {
                        URL = strNewURL,
                        ContentType = "application/x-www-form-urlencoded",
                        Accept = "*/*",
                    };
                    HttpHelper GetJuHelper2 = new HttpHelper();
                    HttpResult GetJuResult2 = GetJuHelper2.GetHtml(GetJuItem2);
                    if (GetJuResult2.StatusCode == HttpStatusCode.Found)
                    {
                        string strNewURL2 = GetJuResult2.Header["Location"];

                        if (_stopThread)
                        {
                            return "";
                        }
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
                                Referer = strNewURL2
                            };
                            HttpHelper GetJuHelper3 = new HttpHelper();
                            HttpResult GetJuResult3 = GetJuHelper3.GetHtml(GetJuItem3);

                            if (_stopThread)
                            {
                                return "";
                            }
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

        // 使用超级搜索获取商品信息
        private void GetProductInfo(string url, out string strTitle, out string sale30, out string tkRate, out string tkCommFee, out string zkPrice)
        {
            string strReqURL = "http://pub.alimama.com/items/search.json?q=" + HttpUtility.UrlEncode(url);
            HttpItem GetJuItem = new HttpItem() // 第一次跳转
            {
                URL = strReqURL,
                ContentType = "application/x-www-form-urlencoded",
                Accept = "*/*"
            };
            HttpHelper GetJuHelper = new HttpHelper();
            HttpResult GetJuResult = GetJuHelper.GetHtml(GetJuItem);
            if (GetJuResult.StatusCode == HttpStatusCode.OK)
            {
                try
                {
                    JObject jp = (JObject)JsonConvert.DeserializeObject(GetJuResult.Html);
                    JArray jArray = (JArray)jp["data"]["pageList"];
                    strTitle = jArray[0]["title"].ToString();
                    sale30 = jArray[0]["biz30day"].ToString();
                    tkRate = jArray[0]["tkRate"].ToString();
                    tkCommFee = jArray[0]["tkCommFee"].ToString();
                    zkPrice = jArray[0]["zkPrice"].ToString();
                }
                catch (Exception)
                {
                    strTitle = "";
                    sale30 = "";
                    tkRate = "";
                    tkCommFee = "";
                    zkPrice = "";

                    Debug.WriteLine(GetJuResult.StatusCode);
                    Debug.WriteLine(url);
                    Debug.WriteLine(strReqURL);
                }
            }
            else
            {
                strTitle = "";
                sale30 = "";
                tkRate = "";
                tkCommFee = "";
                zkPrice = "";

                Debug.WriteLine(GetJuResult.StatusCode);
                Debug.WriteLine(url);
                Debug.WriteLine(strReqURL);
            }
        }

        // 重试失败的URL列表
        private void RetryFailedURLList()
        {
            for (int i = _failedURLList.Count - 1; i >= 0; i--)
            {
                string strTitle, sale30, tkRate, tkCommFee, zkPrice;
                GetProductInfo(_failedURLList[i].OriginURL, out strTitle, out sale30, out tkRate, out tkCommFee, out zkPrice);
                if (strTitle != "")
                {
                    _failedURLList[i].Title = strTitle;
                    _failedURLList[i].Sale30 = sale30;
                    _failedURLList[i].TkRate = tkRate;
                    _failedURLList[i].TkCommFee = tkCommFee;
                    _failedURLList[i].ZkPrice = zkPrice;
                    lock (_collectURLLock)
                    {
                        _collectURLList.Add(_failedURLList[i]);
                    }
                    Debug.WriteLine(_failedURLList[i].Title);
                    _failedURLList.Remove(_failedURLList[i]);
                }
            }
        }

        private void ParseTkURLList()
        {
            string strTkURL = "";
            lock (_tkURLLock)
            {
                if (_parsedURLList.Count > 0)
                {
                    strTkURL = _parsedURLList[0];
                    _parsedURLList.RemoveAt(0); // 每次取一个
                }
            }
            if (strTkURL != "")
            {
                string strOriginURL = ParseTkURL(strTkURL);
                if (strOriginURL != "")
                {
                    CollectURL newURL = new CollectURL();
                    newURL.URL = strTkURL;

                    string strTitle, sale30, tkRate, tkCommFee, zkPrice;
                    GetProductInfo(strOriginURL, out strTitle, out sale30, out tkRate, out tkCommFee, out zkPrice);

                    newURL.OriginURL = strOriginURL;
                    if (strTitle == "")
                    {
                        // 超级搜索失败了，添加到失败列表中
                        _failedURLList.Add(newURL);
                    }
                    else
                    {
                        // 成功
                        newURL.Title = strTitle;
                        newURL.Sale30 = sale30;
                        newURL.TkRate = tkRate;
                        newURL.TkCommFee = tkCommFee;
                        newURL.ZkPrice = zkPrice;
                        newURL.Time = DateTime.Now.ToString();
                        lock (_collectURLLock)
                        {
                            _collectURLList.Add(newURL);
                        }
                        _parsedURLList.Add(strTkURL); // 添加到已经解析过的列表中
                    }
                }
            }
        }
        private static void ParseURLThread(object o)
        {
            DateTime lastParseTime = new DateTime();
            ProductCollector collector = o as ProductCollector;
            while (!collector._stopThread)
            {
                DateTime now = DateTime.Now;
                TimeSpan delta = now - lastParseTime;
                if (delta.TotalSeconds > 70)
                {
                    // 开始解析
                    Debug.WriteLine("开始解析");

                    collector.ParseTkURLList();
                    lastParseTime = now;
                }
                Thread.Sleep(1000);
            }
        }
    }
}
