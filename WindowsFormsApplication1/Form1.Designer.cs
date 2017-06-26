namespace WindowsFormsApplication1
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.RefreshTimer = new System.Windows.Forms.Timer(this.components);
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.HookButton = new System.Windows.Forms.Button();
            this.KeepAliveBtn = new System.Windows.Forms.Button();
            this.saveCookieBtn = new System.Windows.Forms.Button();
            this.PasswordTextBox = new System.Windows.Forms.TextBox();
            this.UserNameTextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.button5 = new System.Windows.Forms.Button();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.button6 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.webBrowser1 = new System.Windows.Forms.WebBrowser();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.button3 = new System.Windows.Forms.Button();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.label5 = new System.Windows.Forms.Label();
            this.UpPageButton = new System.Windows.Forms.Button();
            this.DownPageButton = new System.Windows.Forms.Button();
            this.listView2 = new System.Windows.Forms.ListView();
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.KeepLiveTimer = new System.Windows.Forms.Timer(this.components);
            this.LoadImgTimer = new System.Windows.Forms.Timer(this.components);
            this.label6 = new System.Windows.Forms.Label();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.SearchButton = new System.Windows.Forms.Button();
            this.tabPage4.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.tabPage1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.SuspendLayout();
            // 
            // RefreshTimer
            // 
            this.RefreshTimer.Interval = 600000;
            this.RefreshTimer.Tick += new System.EventHandler(this.RefreshTimerTick);
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.HookButton);
            this.tabPage4.Controls.Add(this.KeepAliveBtn);
            this.tabPage4.Controls.Add(this.saveCookieBtn);
            this.tabPage4.Controls.Add(this.PasswordTextBox);
            this.tabPage4.Controls.Add(this.UserNameTextBox);
            this.tabPage4.Controls.Add(this.label4);
            this.tabPage4.Controls.Add(this.label3);
            this.tabPage4.Controls.Add(this.button5);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(524, 383);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "登录";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // HookButton
            // 
            this.HookButton.Enabled = false;
            this.HookButton.Location = new System.Drawing.Point(395, 177);
            this.HookButton.Name = "HookButton";
            this.HookButton.Size = new System.Drawing.Size(75, 23);
            this.HookButton.TabIndex = 7;
            this.HookButton.Text = "Hook";
            this.HookButton.UseVisualStyleBackColor = true;
            this.HookButton.Visible = false;
            this.HookButton.Click += new System.EventHandler(this.HookButton_Click);
            // 
            // KeepAliveBtn
            // 
            this.KeepAliveBtn.Enabled = false;
            this.KeepAliveBtn.Location = new System.Drawing.Point(281, 178);
            this.KeepAliveBtn.Name = "KeepAliveBtn";
            this.KeepAliveBtn.Size = new System.Drawing.Size(75, 23);
            this.KeepAliveBtn.TabIndex = 6;
            this.KeepAliveBtn.Text = "KeepAlive";
            this.KeepAliveBtn.UseVisualStyleBackColor = true;
            this.KeepAliveBtn.Visible = false;
            this.KeepAliveBtn.Click += new System.EventHandler(this.KeepAliveBtn_Click);
            // 
            // saveCookieBtn
            // 
            this.saveCookieBtn.Location = new System.Drawing.Point(174, 178);
            this.saveCookieBtn.Name = "saveCookieBtn";
            this.saveCookieBtn.Size = new System.Drawing.Size(75, 23);
            this.saveCookieBtn.TabIndex = 5;
            this.saveCookieBtn.Text = "saveCookie";
            this.saveCookieBtn.UseVisualStyleBackColor = true;
            this.saveCookieBtn.Visible = false;
            this.saveCookieBtn.Click += new System.EventHandler(this.saveCookieBtn_Click);
            // 
            // PasswordTextBox
            // 
            this.PasswordTextBox.Location = new System.Drawing.Point(124, 112);
            this.PasswordTextBox.Name = "PasswordTextBox";
            this.PasswordTextBox.PasswordChar = '*';
            this.PasswordTextBox.Size = new System.Drawing.Size(153, 21);
            this.PasswordTextBox.TabIndex = 4;
            this.PasswordTextBox.Text = "jfKSK941";
            this.PasswordTextBox.Visible = false;
            // 
            // UserNameTextBox
            // 
            this.UserNameTextBox.Location = new System.Drawing.Point(124, 73);
            this.UserNameTextBox.Name = "UserNameTextBox";
            this.UserNameTextBox.Size = new System.Drawing.Size(153, 21);
            this.UserNameTextBox.TabIndex = 3;
            this.UserNameTextBox.Text = "xingkong_2000";
            this.UserNameTextBox.Visible = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(45, 112);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 12);
            this.label4.TabIndex = 2;
            this.label4.Text = "Password:";
            this.label4.Visible = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(43, 73);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 12);
            this.label3.TabIndex = 1;
            this.label3.Text = "UserName:";
            this.label3.Visible = false;
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(92, 178);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(75, 23);
            this.button5.TabIndex = 0;
            this.button5.Text = "Login";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.textBox1);
            this.tabPage2.Controls.Add(this.label2);
            this.tabPage2.Controls.Add(this.label1);
            this.tabPage2.Controls.Add(this.comboBox1);
            this.tabPage2.Controls.Add(this.button1);
            this.tabPage2.Controls.Add(this.groupBox1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(524, 383);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "推广";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(103, 86);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(397, 21);
            this.textBox1.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(32, 90);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "URL：";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(32, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "推广单元：";
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(103, 21);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(121, 20);
            this.comboBox1.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(246, 19);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(100, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "获取推广单元";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.pictureBox2);
            this.groupBox1.Controls.Add(this.textBox2);
            this.groupBox1.Controls.Add(this.button6);
            this.groupBox1.Controls.Add(this.button2);
            this.groupBox1.Location = new System.Drawing.Point(-134, 58);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(650, 269);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "groupBox1";
            // 
            // pictureBox2
            // 
            this.pictureBox2.Location = new System.Drawing.Point(168, 94);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(185, 169);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox2.TabIndex = 4;
            this.pictureBox2.TabStop = false;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(359, 94);
            this.textBox2.Multiline = true;
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(275, 169);
            this.textBox2.TabIndex = 3;
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(237, 65);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(100, 23);
            this.button6.TabIndex = 2;
            this.button6.Text = "查询商品";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(359, 65);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 1;
            this.button2.Text = "群发";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.webBrowser1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(524, 383);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "阿里妈妈";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // webBrowser1
            // 
            this.webBrowser1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowser1.Location = new System.Drawing.Point(3, 3);
            this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser1.Name = "webBrowser1";
            this.webBrowser1.Size = new System.Drawing.Size(518, 377);
            this.webBrowser1.TabIndex = 2;
            this.webBrowser1.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.OnDocumentCompleted);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage5);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(532, 409);
            this.tabControl1.TabIndex = 1;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.listView1);
            this.tabPage3.Controls.Add(this.button3);
            this.tabPage3.Controls.Add(this.textBox3);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(524, 383);
            this.tabPage3.TabIndex = 4;
            this.tabPage3.Text = "监控";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.listView1.FullRowSelect = true;
            this.listView1.LabelEdit = true;
            this.listView1.Location = new System.Drawing.Point(9, 207);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(507, 168);
            this.listView1.TabIndex = 2;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.DoubleClick += new System.EventHandler(this.OnDoubleClickURL);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "淘客链接";
            this.columnHeader1.Width = 250;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "原始链接";
            this.columnHeader2.Width = 400;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(224, 178);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 1;
            this.button3.Text = "获取";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(8, 6);
            this.textBox3.Multiline = true;
            this.textBox3.Name = "textBox3";
            this.textBox3.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox3.Size = new System.Drawing.Size(508, 166);
            this.textBox3.TabIndex = 0;
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.SearchButton);
            this.tabPage5.Controls.Add(this.textBox4);
            this.tabPage5.Controls.Add(this.label6);
            this.tabPage5.Controls.Add(this.label5);
            this.tabPage5.Controls.Add(this.UpPageButton);
            this.tabPage5.Controls.Add(this.DownPageButton);
            this.tabPage5.Controls.Add(this.listView2);
            this.tabPage5.Location = new System.Drawing.Point(4, 22);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Size = new System.Drawing.Size(524, 383);
            this.tabPage5.TabIndex = 5;
            this.tabPage5.Text = "产品库";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(392, 350);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(11, 12);
            this.label5.TabIndex = 5;
            this.label5.Text = "1";
            // 
            // UpPageButton
            // 
            this.UpPageButton.Location = new System.Drawing.Point(292, 345);
            this.UpPageButton.Name = "UpPageButton";
            this.UpPageButton.Size = new System.Drawing.Size(75, 23);
            this.UpPageButton.TabIndex = 4;
            this.UpPageButton.Text = "上一页";
            this.UpPageButton.UseVisualStyleBackColor = true;
            this.UpPageButton.Click += new System.EventHandler(this.UpPageButton_Click);
            // 
            // DownPageButton
            // 
            this.DownPageButton.Location = new System.Drawing.Point(426, 345);
            this.DownPageButton.Name = "DownPageButton";
            this.DownPageButton.Size = new System.Drawing.Size(75, 23);
            this.DownPageButton.TabIndex = 3;
            this.DownPageButton.Text = "下一页";
            this.DownPageButton.UseVisualStyleBackColor = true;
            this.DownPageButton.Click += new System.EventHandler(this.DownPageButton_Click);
            // 
            // listView2
            // 
            this.listView2.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6,
            this.columnHeader7,
            this.columnHeader8});
            this.listView2.FullRowSelect = true;
            this.listView2.GridLines = true;
            this.listView2.LargeImageList = this.imageList1;
            this.listView2.Location = new System.Drawing.Point(8, 59);
            this.listView2.Name = "listView2";
            this.listView2.Size = new System.Drawing.Size(508, 269);
            this.listView2.SmallImageList = this.imageList1;
            this.listView2.TabIndex = 0;
            this.listView2.UseCompatibleStateImageBehavior = false;
            this.listView2.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "标题";
            this.columnHeader3.Width = 250;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "价格";
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "佣金比例";
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "佣金";
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "月销";
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "网址";
            this.columnHeader8.Width = 200;
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(64, 64);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // KeepLiveTimer
            // 
            this.KeepLiveTimer.Interval = 180000;
            this.KeepLiveTimer.Tick += new System.EventHandler(this.OnKeepLiveTimer);
            // 
            // LoadImgTimer
            // 
            this.LoadImgTimer.Interval = 2000;
            this.LoadImgTimer.Tick += new System.EventHandler(this.OnLoadImgTimer);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(40, 25);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(41, 12);
            this.label6.TabIndex = 6;
            this.label6.Text = "搜索：";
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(87, 22);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(316, 21);
            this.textBox4.TabIndex = 7;
            // 
            // SearchButton
            // 
            this.SearchButton.Location = new System.Drawing.Point(409, 21);
            this.SearchButton.Name = "SearchButton";
            this.SearchButton.Size = new System.Drawing.Size(75, 23);
            this.SearchButton.TabIndex = 8;
            this.SearchButton.Text = "搜索";
            this.SearchButton.UseVisualStyleBackColor = true;
            this.SearchButton.Click += new System.EventHandler(this.SearchButton_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(532, 409);
            this.Controls.Add(this.tabControl1);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.OnFormLoad);
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.tabPage1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.tabPage5.ResumeLayout(false);
            this.tabPage5.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer RefreshTimer;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.TextBox PasswordTextBox;
        private System.Windows.Forms.TextBox UserNameTextBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.WebBrowser webBrowser1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.Button saveCookieBtn;
        private System.Windows.Forms.Timer KeepLiveTimer;
        private System.Windows.Forms.Button KeepAliveBtn;
        private System.Windows.Forms.Button HookButton;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.Button DownPageButton;
        private System.Windows.Forms.ListView listView2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Timer LoadImgTimer;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button UpPageButton;
        private System.Windows.Forms.Button SearchButton;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.Label label6;

    }
}

