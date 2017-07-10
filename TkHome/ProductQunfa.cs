using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;

// 商品群发类
namespace TkHome
{
    class QunfaParam
    {
        public ProductQunfa Qunfa { get; set; }
        public DbOperator Database { get; set; }
        public Alimama Mama { get; set; }
        public string AdzoneName { get; set; }
        public int StartTime { get; set; }
        public int EndTime { get; set; }
        public int Interval { get; set; }

        public QunfaParam(ProductQunfa qunfa, DbOperator database, Alimama alimama, string adzoneName, int startTime, int endTime, int interval)
        {
            Qunfa = qunfa;
            Database = database;
            Mama = alimama;
            AdzoneName = adzoneName;
            StartTime = startTime;
            EndTime = endTime;
            Interval = interval;
        }
    }

    class TranslateUrlResult
    {
        public string ProductTitle { get; set; }
        public string QQShowContent { get; set; }
        public string WechatShowContent { get; set; } // 文案

        public TranslateUrlResult(string title, string qqShowContent, string wechatShowContent)
        {
            ProductTitle = title;
            QQShowContent = qqShowContent;
            WechatShowContent = wechatShowContent;
        }
    }

    class ProductQunfa
    {
        private List<WndInfo> _qqWechatList = new List<WndInfo>(); // QQ微信窗口列表
        private Thread _qunfaThread;
        private bool StopThread { get; set; }
        private int QunfaStartRow { get; set; } // 群发开始行
        private int QunfaCount { get; set; } // 一次群发的数量
        private List<TranslateUrlResult> _translateList = new List<TranslateUrlResult>();
        private object _translateListLock = new object();

        public ProductQunfa()
        {
            QunfaStartRow = 0; // 从第一行开始
            QunfaCount = 1; // 一次群发1个商品
        }
        public List<WndInfo> GetAllQQWechatWnd()
        {
            _qqWechatList.Clear();
            EnumWindowsCallBack myCallBack = new EnumWindowsCallBack(EnumWindowsAndGetContent);
            Win32.EnumWindows(myCallBack, 0);
            return _qqWechatList;
        }

        public void StartTranslateUrl(DbOperator database, Alimama alimama, string adzoneName, int startTime, int endTime, int interval)
        {
            StopThread = false;
            _qunfaThread = new Thread(QunfaThreadProc);
            QunfaParam param = new QunfaParam(this, database, alimama, adzoneName, startTime, endTime, interval);
            _qunfaThread.Start(param);
        }

        public void StopTranslateUrl()
        {
            StopThread = true;
            if (_qunfaThread != null && _qunfaThread.IsAlive)
                _qunfaThread.Join();
        }

        public List<TranslateUrlResult> GetTranslateResult()
        {
            List<TranslateUrlResult> retList = new List<TranslateUrlResult>();
            lock (_translateListLock)
            {
                _translateList.ForEach(t => retList.Add(t));
                _translateList.Clear();
            }
            return retList;
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
                            _qqWechatList.Add(new WndInfo(strWndName, hwnd, true));
                        }
                    }
                }
                else if (className.ToString() == "ChatWnd")
                {
                    // 微信窗口
                    StringBuilder wndName = new StringBuilder(200);
                    int nameLen;
                    nameLen = Win32.GetWindowText(hwnd, wndName, 200);
                    if (nameLen > 0)
                    {
                        string strWndName = wndName.ToString();
                        _qqWechatList.Add(new WndInfo(strWndName, hwnd, false));
                    }
                }
            }
            return true;
        }

        private void QunfaThreadProc(object param)
        {
            QunfaParam qunfaParam = param as QunfaParam;
            DateTime lastQunfaTime = new DateTime();
            while (!qunfaParam.Qunfa.StopThread)
            {
                DateTime nowTime = DateTime.Now;
                TimeSpan delta = nowTime - lastQunfaTime;
                if (delta.TotalSeconds > qunfaParam.Interval * 60 && nowTime.Hour >= qunfaParam.StartTime && nowTime.Hour < qunfaParam.EndTime)
                {
                    Debugger.Log(0, null, "开始群发 " + DateTime.Now.ToString());
                    while (!qunfaParam.Qunfa.StopThread) // 解析失败后重试
                    {
                        bool bTranslatedSuccess = false;
                        // 从数据库中加载商品
                        List<ProductInfo> productList = qunfaParam.Database.loadProductList(qunfaParam.Qunfa.QunfaStartRow, qunfaParam.Qunfa.QunfaCount, true);
                        foreach (ProductInfo product in productList)
                        {
                            string url = product._auctionUrl;
                            string decryptURL = EncryptDES.Decrypt(url); // 解密

                            if (qunfaParam.Qunfa.StopThread)
                            {
                                break;
                            }
                            string imgPath, qqShowContent, wechatShowContent; // 图片路径，文案
                            bool bSuccess = qunfaParam.Mama.TranslateURL(decryptURL, qunfaParam.AdzoneName, out imgPath, out qqShowContent, out wechatShowContent); // 转链
                            if (qunfaParam.Qunfa.StopThread)
                            {
                                break;
                            }
                            if (!bSuccess) // 转链失败
                            {
                                Debugger.Log(0, null, decryptURL + " translate failed");
                                continue;
                            }

                            string strQQShowContent = ClipboardDataWrapper.WrapFroQQ(imgPath, qqShowContent);
                            string strWechatShowContent = ClipboardDataWrapper.WrapForWechat(imgPath, wechatShowContent);
                            TranslateUrlResult result = new TranslateUrlResult(product._title, strQQShowContent, strWechatShowContent);
                            lock (_translateListLock)
                            {
                                _translateList.Add(result);
                                bTranslatedSuccess = true;
                            }
                        }

                        //                             if (productList.Count == 0)
                        //                             {
                        //                                 qunfaParam.Qunfa.QunfaStartRow = 0; // 从头开始群发
                        //                             }
                        //                             else
                        //                             {
                        //                                 qunfaParam.Qunfa.QunfaStartRow += qunfaParam.Qunfa.QunfaCount; // 取后面的数据
                        //                             }
                        // 
                        if (bTranslatedSuccess) // 如果有结果，则不再取，否则继续取
                        {
                            break;
                        }
                    }

                    lastQunfaTime = nowTime;
                }
            }
        }
    }
}
