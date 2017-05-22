namespace QuickDebug
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
            this.webBrowser1 = new System.Windows.Forms.WebBrowser();
            this.softPhone1 = new VoiceOCX.SoftPhone();
            this.clipboardControl1 = new ClipboardOCX.ClipboardControl();
            this.SuspendLayout();
            // 
            // webBrowser1
            // 
            this.webBrowser1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowser1.Location = new System.Drawing.Point(0, 0);
            this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser1.Name = "webBrowser1";
            this.webBrowser1.Size = new System.Drawing.Size(627, 264);
            this.webBrowser1.TabIndex = 1;
            this.webBrowser1.Url = new System.Uri("http://localhost:4898/TestUpload.html", System.UriKind.Absolute);
            // 
            // softPhone1
            // 
            this.softPhone1.Location = new System.Drawing.Point(38, 26);
            this.softPhone1.Name = "softPhone1";
            this.softPhone1.Size = new System.Drawing.Size(68, 60);
            this.softPhone1.TabIndex = 2;
            // 
            // clipboardControl1
            // 
            this.clipboardControl1.Location = new System.Drawing.Point(153, 37);
            this.clipboardControl1.Name = "clipboardControl1";
            this.clipboardControl1.Size = new System.Drawing.Size(150, 150);
            this.clipboardControl1.TabIndex = 3;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(627, 264);
            this.Controls.Add(this.webBrowser1);
            this.Controls.Add(this.softPhone1);
            this.Controls.Add(this.clipboardControl1);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.WebBrowser webBrowser1;
        private VoiceOCX.SoftPhone softPhone1;
        private ClipboardOCX.ClipboardControl clipboardControl1;
    }
}

