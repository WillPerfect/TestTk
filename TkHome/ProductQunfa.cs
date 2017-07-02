using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// 商品群发类
namespace TkHome
{
    class ProductQunfa
    {
        private List<WndInfo> _qqWechatList = new List<WndInfo>(); // QQ微信窗口列表

        public List<WndInfo> GetAllQQWechatWnd()
        {
            _qqWechatList.Clear();
            EnumWindowsCallBack myCallBack = new EnumWindowsCallBack(EnumWindowsAndGetContent);
            Win32.EnumWindows(myCallBack, 0);
            return _qqWechatList;
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
                            _qqWechatList.Add(new WndInfo(strWndName, hwnd));
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
    }
}
