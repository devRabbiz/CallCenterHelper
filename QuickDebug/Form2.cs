using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace QuickDebug
{
    public partial class Form2 : Form
    {
        private VoiceOCX.Media.MediaCall mc;
        private delegate void GenesysEventHandlerDelegate(string message);
        public Form2()
        {
            InitializeComponent();

            mc = new VoiceOCX.Media.MediaCall();
            mc.OnLog += mc_OnLog;
            mc.InitializePSDKApplicationBlocks();
            //mc.Connect();
            //mc.AgentLogin();

            this.FormClosing += Form2_FormClosing;
        }

        void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            mc.FinalizePSDKApplicationBlocks();
        }

        void mc_OnLog(string message)
        {
            if (this.InvokeRequired)
            {
                GenesysEventHandlerDelegate handler = new GenesysEventHandlerDelegate(this.mc_OnLog);
                this.Invoke(handler, new object[] { message });
            }
            else
            {
                this.richTextBox1.AppendText(message);
                this.richTextBox1.ScrollToCaret();
                this.richTextBox1.AppendText("\r\n");
            }
        }

        private void Connect_Click(object sender, EventArgs e)
        {
            mc.Connect();
        }

        private void AgentLogin_Click(object sender, EventArgs e)
        {
            mc.AgentLogin();
        }

        private void AgentLogout_Click(object sender, EventArgs e)
        {
            mc.AgentLogout();
        }

        private void Disconnect_Click(object sender, EventArgs e)
        {
            mc.Disconnect();
        }

        private void Ready_Click(object sender, EventArgs e)
        {
            mc.CancelNotReadyForMedia();
        }

        private void NotReady_Click(object sender, EventArgs e)
        {
            mc.NotReadyForMedia();
        }

        private void 接受_Click(object sender, EventArgs e)
        {
            var d = mc.dicInteraction.FirstOrDefault();
            mc.Accept(d.Value.TicketId, d.Value.InteractionId);
        }
    }
}
