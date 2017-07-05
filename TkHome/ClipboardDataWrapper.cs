using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TkHome
{
    // 剪贴板数据封装类
    class ClipboardDataWrapper
    {
        public static string WrapFroQQ(string strImagePath, string strShowContent)
        {
            StringBuilder sb1 = new StringBuilder();
            sb1.Append("<QQRichEditFormat><Info version=\"1001\"></Info>");
            sb1.Append("<EditElement type=\"1\" filepath=\"");
            sb1.Append(strImagePath);
            sb1.Append("\" shortcut=\"\"></EditElement>");

            sb1.Append("<EditElement type=\"0\"><![CDATA[");
            sb1.Append(strShowContent);
            sb1.Append("]]></EditElement>");
            sb1.Append("</QQRichEditFormat>");

            return sb1.ToString();
        }

        public static string WrapForWechat(string strImagePath, string strShowContent)
        {
            StringBuilder sb1 = new StringBuilder();
            sb1.Append("<html><body>");
            sb1.AppendFormat("<img src='{0}' />", strImagePath);

            string newShowContent = strShowContent.Replace("\n", "<br>");
            sb1.Append(newShowContent);
            sb1.Append("</body></html>");

            return sb1.ToString();
        }
    }
}
