using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Drawing;
using Accessibility;

namespace TkHome
{
    public delegate bool EnumWindowsCallBack(int hwnd, int lParam); // 枚举窗口的回调函数
    class Win32
    {
        [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool InternetSetCookie(string lpszUrlName, string lbszCookieName, string lpszCookieData);

        [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool InternetGetCookieEx(string pchURL, string pchCookieName, StringBuilder pchCookieData, ref System.UInt32 pcchCookieData, int dwFlags, IntPtr lpReserved);

        // 引用3个API
        [DllImport("user32")]
        public static extern int EnumWindows(EnumWindowsCallBack x, int y);

        [DllImport("user32.dll")]
        public static extern bool IsIconic(int hwnd);

        public const int SW_MINIMIZE = 6;
        public const int SW_RESTORE = 9;

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(int hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        public static extern int GetClassName(int hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        public static extern int GetWindowText(int hWnd, StringBuilder lpString, int nMaxCount);

        public const uint WM_PASTE = 0x0302;
        public const uint WM_KEYDOWN = 0x0100;
        public const int VK_RETURN = 0x0D;

        public const uint WM_LBUTTONDOWN = 0x0201;
        public const uint WM_LBUTTONUP = 0x0202;
        public const uint WM_KEYUP = 0x0101;
        public const int MK_LBUTTON = 1;
        public const byte VK_CONTROL = 0x11;
        public const uint KEYEVENTF_KEYUP = 2;

        [DllImport("user32.dll")]
        public static extern int SendMessage(int hWnd, uint msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo);

        [DllImport("user32.dll")]
        public static extern void SwitchToThisWindow(int hWnd, int fAltTab);

        [DllImport("user32.dll")]//取设备场景 
        public static extern IntPtr GetDC(IntPtr hwnd);//返回设备场景句柄 
        [DllImport("gdi32.dll")]//取指定点颜色 
        public static extern int GetPixel(IntPtr hdc, Point p);

        public const int CHILDID_SELF = 0;
        public const int CHILDID_1 = 1;
        public const int OBJID_CLIENT = -4;
        [DllImport("Oleacc.dll")]
        public static extern int AccessibleObjectFromWindow(
        IntPtr hwnd,
        int dwObjectID,
        ref Guid refID,
        ref IAccessible ppvObject);

        [DllImport("Oleacc.dll")]
        public static extern int WindowFromAccessibleObject(
            IAccessible pacc,
            out IntPtr phwnd);

        [DllImport("Oleacc.dll")]
        public static extern int AccessibleChildren(
        Accessibility.IAccessible paccContainer,
        int iChildStart,
        int cChildren,
        [Out] object[] rgvarChildren,
        out int pcObtained);
    }
}
