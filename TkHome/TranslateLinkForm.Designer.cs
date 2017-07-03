namespace TkHome
{
    partial class TranslateLinkForm
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
            this.contentRichTextBox = new System.Windows.Forms.RichTextBox();
            this.copyToQQButton = new System.Windows.Forms.Button();
            this.copyToWeChatButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // contentRichTextBox
            // 
            this.contentRichTextBox.Dock = System.Windows.Forms.DockStyle.Left;
            this.contentRichTextBox.Location = new System.Drawing.Point(0, 0);
            this.contentRichTextBox.Name = "contentRichTextBox";
            this.contentRichTextBox.Size = new System.Drawing.Size(375, 479);
            this.contentRichTextBox.TabIndex = 0;
            this.contentRichTextBox.Text = "";
            // 
            // copyToQQButton
            // 
            this.copyToQQButton.Location = new System.Drawing.Point(444, 204);
            this.copyToQQButton.Name = "copyToQQButton";
            this.copyToQQButton.Size = new System.Drawing.Size(75, 23);
            this.copyToQQButton.TabIndex = 1;
            this.copyToQQButton.Text = "复制到QQ";
            this.copyToQQButton.UseVisualStyleBackColor = true;
            this.copyToQQButton.Click += new System.EventHandler(this.copyToQQButton_Click);
            // 
            // copyToWeChatButton
            // 
            this.copyToWeChatButton.Location = new System.Drawing.Point(444, 270);
            this.copyToWeChatButton.Name = "copyToWeChatButton";
            this.copyToWeChatButton.Size = new System.Drawing.Size(75, 23);
            this.copyToWeChatButton.TabIndex = 2;
            this.copyToWeChatButton.Text = "复制到微信";
            this.copyToWeChatButton.UseVisualStyleBackColor = true;
            this.copyToWeChatButton.Click += new System.EventHandler(this.copyToWeChatButton_Click);
            // 
            // TranslateLinkForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(565, 479);
            this.Controls.Add(this.copyToWeChatButton);
            this.Controls.Add(this.copyToQQButton);
            this.Controls.Add(this.contentRichTextBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "TranslateLinkForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "转链结果";
            this.Load += new System.EventHandler(this.OnFormLoad);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox contentRichTextBox;
        private System.Windows.Forms.Button copyToQQButton;
        private System.Windows.Forms.Button copyToWeChatButton;
    }
}