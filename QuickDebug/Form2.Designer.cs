namespace QuickDebug
{
    partial class Form2
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
            this.AgentLogout = new System.Windows.Forms.Button();
            this.Disconnect = new System.Windows.Forms.Button();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.AgentLogin = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.Connect = new System.Windows.Forms.Button();
            this.Ready = new System.Windows.Forms.Button();
            this.NotReady = new System.Windows.Forms.Button();
            this.接受 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // AgentLogout
            // 
            this.AgentLogout.Location = new System.Drawing.Point(310, 36);
            this.AgentLogout.Name = "AgentLogout";
            this.AgentLogout.Size = new System.Drawing.Size(91, 23);
            this.AgentLogout.TabIndex = 0;
            this.AgentLogout.Text = "AgentLogout";
            this.AgentLogout.UseVisualStyleBackColor = true;
            this.AgentLogout.Click += new System.EventHandler(this.AgentLogout_Click);
            // 
            // Disconnect
            // 
            this.Disconnect.Location = new System.Drawing.Point(310, 7);
            this.Disconnect.Name = "Disconnect";
            this.Disconnect.Size = new System.Drawing.Size(91, 23);
            this.Disconnect.TabIndex = 0;
            this.Disconnect.Text = "Disconnect";
            this.Disconnect.UseVisualStyleBackColor = true;
            this.Disconnect.Click += new System.EventHandler(this.Disconnect_Click);
            // 
            // richTextBox1
            // 
            this.richTextBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox1.Location = new System.Drawing.Point(12, 100);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(635, 306);
            this.richTextBox1.TabIndex = 1;
            this.richTextBox1.Text = "";
            // 
            // AgentLogin
            // 
            this.AgentLogin.Location = new System.Drawing.Point(21, 36);
            this.AgentLogin.Name = "AgentLogin";
            this.AgentLogin.Size = new System.Drawing.Size(91, 23);
            this.AgentLogin.TabIndex = 0;
            this.AgentLogin.Text = "AgentLogin";
            this.AgentLogin.UseVisualStyleBackColor = true;
            this.AgentLogin.Click += new System.EventHandler(this.AgentLogin_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(-250, 12);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(91, 23);
            this.button4.TabIndex = 0;
            this.button4.Text = "AgentLogout";
            this.button4.UseVisualStyleBackColor = true;
            // 
            // Connect
            // 
            this.Connect.Location = new System.Drawing.Point(21, 7);
            this.Connect.Name = "Connect";
            this.Connect.Size = new System.Drawing.Size(91, 23);
            this.Connect.TabIndex = 0;
            this.Connect.Text = "Connect";
            this.Connect.UseVisualStyleBackColor = true;
            this.Connect.Click += new System.EventHandler(this.Connect_Click);
            // 
            // Ready
            // 
            this.Ready.Location = new System.Drawing.Point(21, 66);
            this.Ready.Name = "Ready";
            this.Ready.Size = new System.Drawing.Size(75, 23);
            this.Ready.TabIndex = 2;
            this.Ready.Text = "Ready";
            this.Ready.UseVisualStyleBackColor = true;
            this.Ready.Click += new System.EventHandler(this.Ready_Click);
            // 
            // NotReady
            // 
            this.NotReady.Location = new System.Drawing.Point(310, 66);
            this.NotReady.Name = "NotReady";
            this.NotReady.Size = new System.Drawing.Size(75, 23);
            this.NotReady.TabIndex = 2;
            this.NotReady.Text = "NotReady";
            this.NotReady.UseVisualStyleBackColor = true;
            this.NotReady.Click += new System.EventHandler(this.NotReady_Click);
            // 
            // 接受
            // 
            this.接受.Location = new System.Drawing.Point(514, 13);
            this.接受.Name = "接受";
            this.接受.Size = new System.Drawing.Size(75, 23);
            this.接受.TabIndex = 3;
            this.接受.Text = "接受";
            this.接受.UseVisualStyleBackColor = true;
            this.接受.Click += new System.EventHandler(this.接受_Click);
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(659, 418);
            this.Controls.Add(this.接受);
            this.Controls.Add(this.NotReady);
            this.Controls.Add(this.Ready);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.Connect);
            this.Controls.Add(this.Disconnect);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.AgentLogin);
            this.Controls.Add(this.AgentLogout);
            this.Name = "Form2";
            this.Text = "Form2";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button AgentLogout;
        private System.Windows.Forms.Button Disconnect;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Button AgentLogin;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button Connect;
        private System.Windows.Forms.Button Ready;
        private System.Windows.Forms.Button NotReady;
        private System.Windows.Forms.Button 接受;
    }
}