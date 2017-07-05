using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace TkHome
{
    public partial class TranslateLinkForm : Form
    {
        public string ShowText { get; set; }
        public string ImagePath { get; set; }
        public TranslateLinkForm()
        {
            InitializeComponent();
        }

        private void OnFormLoad(object sender, EventArgs e)
        {
            contentRichTextBox.Text = ShowText;

            Image img = Image.FromFile(ImagePath);
            Clipboard.Clear();
            Clipboard.SetImage(img);
            contentRichTextBox.Paste();
        }

        // 复制到QQ
        private void copyToQQButton_Click(object sender, EventArgs e)
        {
            StringBuilder sb1 = new StringBuilder();
            sb1.Append("<QQRichEditFormat><Info version=\"1001\"></Info>");
            sb1.Append("<EditElement type=\"1\" filepath=\"");
            sb1.Append(ImagePath);
            sb1.Append("\" shortcut=\"\"></EditElement>");

            string[] lines = contentRichTextBox.Lines;
            for (int i = 0; i < lines.Length; i++)
            {
                sb1.Append("<EditElement type=\"0\"><![CDATA[");
                sb1.Append("\n");
                sb1.Append(lines[i]);
                sb1.Append("]]></EditElement>");
            }
            sb1.Append("</QQRichEditFormat>");

            MemoryStream ms = new MemoryStream(System.Text.Encoding.Default.GetBytes(sb1.ToString()));
            Clipboard.SetData("QQ_RichEdit_Format", ms);
        }

        // 复制到微信
        private void copyToWeChatButton_Click(object sender, EventArgs e)
        {
            StringBuilder sb1 = new StringBuilder();
            sb1.Append("<html><body>");
            sb1.AppendFormat("<img src='{0}' />", ImagePath);

            string[] lines = contentRichTextBox.Lines;
            for (int i = 0; i < lines.Length; i++)
            {
                sb1.Append("<br>");
                sb1.Append(lines[i]);
            }
            sb1.Append("</body></html>");
            Clipboard.SetData(DataFormats.Html, sb1.ToString());
        }
    }
}
