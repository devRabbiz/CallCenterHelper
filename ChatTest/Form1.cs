using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Chat.Common;
using Chat.CustomerInterface;
using Genesyslab.Platform.Commons.Collections;
using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.WebMedia.Protocols;
using Genesyslab.Platform.WebMedia.Protocols.BasicChat;
using System.Web;
using System.Web.UI.HtmlControls;
using Newtonsoft.Json;

namespace ChatTest
{
    public partial class Form1 : Form
    {
        CustomerChatContext context = null;
        private const string SESSION_ID = "TEST_USER";
        private bool enableSendMsg = true;

        private List<string> allMessages = new List<string>();
        private StringBuilder sb = new StringBuilder();

        private int SendCount = 0, ReceivedCount = 0;
        private Random rd = new Random(DateTime.Now.Second);
        private int ii = 0;

        //private string lastRequest = "";
        public Form1()
        {
            InitializeComponent();

            this.btnSend.Visible = false;
            this.btnStop.Visible = false;
        }

        private void btnCon_Click(object sender, EventArgs e)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict["EnterID"] = "9999";
            dict["UserID"] = "12345";
            dict["UserName"] = "联想测试用户";
            dict["MachineNo"] = "123456";
            dict["strServiceCardNo"] = "123456";

            // 根据当前会话ID，找到所在聊天室
            context = new CustomerChatContext(SESSION_ID, JsonConvert.SerializeObject(dict));
            context.InitConnecton();
            if (context.IsAvailableConnection)
            {
                //context.RequestChat();
                this.btnSend.Visible = true;
                this.btnStop.Visible = true;
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            timer1.Start();
            timer2.Start();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            enableSendMsg = false;
            //timer1.Stop();
            timer2.Stop();
        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;

            ClientChatMsgList msgList = new ClientChatMsgList();
            msgList.Init(context);
            if (msgList.MsgList.Count > 0)
            {
                msgList.MsgList.ForEach(item =>
                {
                    this.allMessages.Add(item);
                    sb.Insert(0, item);
                    this.ReceivedCount++;
                });
                this.lblDelay.Text = msgList.MaxDelay.ToString();
            }
            if (msgList.Status == -1)
            {
                enableSendMsg = false;
                timer1.Enabled = false;
                timer1.Stop();
            }

            this.lblS.Text = this.SendCount.ToString();
            this.lblR.Text = this.ReceivedCount.ToString();
            this.webBrowser1.DocumentText = sb.ToString();
            this.Refresh();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (context != null)
            {
                if (enableSendMsg)
                {
                    //int index = rd.Next(this.listBox1.Items.Count - 1);
                    //string msg = this.listBox1.Items[index].ToString();
                    //msg = string.Format("<font color='red'>{0}</font>", msg);
                    context.SendMessage(ii.ToString());
                    ii++;
                    this.SendCount++;
                }
            }
        }



    }
}
