using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using SoftPhone.Entity;
using Tele.DataLibrary;

namespace Tele.Management.Pages.Chat
{
    public partial class ChatHistory : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string ids = Request.QueryString["ids"];

            List<string> chatIDs = new List<string>();
            if (!string.IsNullOrEmpty(ids))
                chatIDs = ids.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            if (chatIDs.Count > 0)
                GetChatList(chatIDs);
        }


        public void GetChatList(List<string> ids)
        {
            List<SPhone_Chat> lst = new List<SPhone_Chat>();
            using (var db = DCHelper.SPhoneContext())
            {
                lst = db.SPhone_Chat.Where(item => ids.Contains(item.ChatID)).ToList();
            }
            if (lst != null && lst.Count > 0)
            {
                this.ltlContent.Text = string.Empty;
                lst.ForEach(item => BuildChatText(item));
            }
        }


        private void BuildChatText(SPhone_Chat chat)
        {
            string emptyMessage = "聊天内容为空。";
            StringBuilder sb = new StringBuilder();
            try
            {
                string chatContent = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(chat.ContentText));
                sb.AppendLine("<div class='chatContent'>");
                sb.AppendLine(string.Format("<h3> {0} </h3>", chat.ChatID));
                if (string.IsNullOrEmpty(chatContent)) sb.AppendLine(emptyMessage);
                else
                {
                    List<ChatMessage> chatMessages = JsonConvert.DeserializeObject<List<ChatMessage>>(chatContent);
                    if (chatMessages.Count == 0)
                        sb.AppendLine(emptyMessage);
                    else
                        chatMessages.ForEach(msg =>
                        {
                            string content = HttpUtility.UrlDecode(msg.FormatMessage);
                            // 替换标签URL
                            content = content.Replace("/Content/Images/emote/", "/Images/emote/");
                            sb.AppendLine(content);
                        });
                }
                sb.AppendLine("</div>");
            }
            catch { }
            if (sb.Length > 0)
                this.ltlContent.Text += sb.ToString();
        }

        #region ChatMessage

        private class ChatMessage
        {
            public string ReceiverID { get; set; }
            public string UserID { get; set; }
            public string NickName { get; set; }
            public string Message { get; set; }
            public DateTime CreateData { get; set; }

            /// <summary>
            /// 信息类别
            /// </summary>
            public MessageType MessageType { get; set; }

            /// <summary>
            /// 用于呈现
            /// </summary>
            public string FormatMessage
            {
                get
                {
                    return string.Format("<div class='{2}'><b>{0} {3:HH:mm:ss}</b></div><div class='MsgContent'> {1} </div>"
                        , this.NickName, this.Message
                        , Enum.GetName(typeof(MessageType), this.MessageType)
                        , this.CreateData);
                }
            }


        }
        #endregion

        #region MessageType

        /// <summary>
        /// 消息类别
        /// </summary>
        private enum MessageType
        {
            /// <summary>
            /// 系统提示
            /// </summary>
            Alert = 1,

            /// <summary>
            /// 坐席消息
            /// </summary>
            AgentMessage = 2,

            /// <summary>
            /// 访客消息
            /// </summary>
            ClientMessage = 4,

            /// <summary>
            /// 系统通知
            /// </summary>
            Notice = 8,
        }

        #endregion
    }
}