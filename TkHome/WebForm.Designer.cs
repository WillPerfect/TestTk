namespace TkHome
{
    partial class WebForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.aliWebBrowser = new System.Windows.Forms.WebBrowser();
            this.detectLoginTimer = new System.Windows.Forms.Timer(this.components);
            this.loginTimer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // aliWebBrowser
            // 
            this.aliWebBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.aliWebBrowser.Location = new System.Drawing.Point(0, 0);
            this.aliWebBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.aliWebBrowser.Name = "aliWebBrowser";
            this.aliWebBrowser.ScriptErrorsSuppressed = true;
            this.aliWebBrowser.Size = new System.Drawing.Size(387, 319);
            this.aliWebBrowser.TabIndex = 0;
            this.aliWebBrowser.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.OnDocumentComplete);
            // 
            // detectLoginTimer
            // 
            this.detectLoginTimer.Interval = 60000;
            this.detectLoginTimer.Tick += new System.EventHandler(this.OnDetectLoginTimer);
            // 
            // loginTimer
            // 
            this.loginTimer.Interval = 5000;
            this.loginTimer.Tick += new System.EventHandler(this.OnLoginTimer);
            // 
            // WebForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(387, 319);
            this.Controls.Add(this.aliWebBrowser);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "WebForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "请先登录阿里妈妈";
            this.Load += new System.EventHandler(this.OnFormLoad);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.WebBrowser aliWebBrowser;
        private System.Windows.Forms.Timer detectLoginTimer;
        private System.Windows.Forms.Timer loginTimer;
    }
}