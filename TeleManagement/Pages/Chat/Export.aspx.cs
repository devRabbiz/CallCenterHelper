using Newtonsoft.Json;
using SoftPhone.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Tele.Common;
using System.IO;
using Tele.DataLibrary;
using System.Threading;

namespace Tele.Management.Pages.Chat
{
    public partial class Export : System.Web.UI.Page
    {
        protected DateTime now = DateTime.Now;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["date"] != null)
            {
                now = DateTime.Parse(Request.QueryString["date"]);
            }
            var path = Server.MapPath("~/Documents/TempFolder/Chat详细记录-" + now.ToString("yyMMdd") + ".xlsx");
            if (!File.Exists(path))
            {
                //ThreadPool.QueueUserWorkItem(c =>
                // {
                using (var db = DCHelper.SPhoneContext())
                {
                    try
                    {
                        var query = db.SPhone_Chat.Where(x => System.Data.Objects.SqlClient.SqlFunctions.DateDiff("d", x.CreateTime, now) == 0 && x.UpdateTime != x.CreateTime).OrderBy(x => x.CreateTime).ToList();
                        ExportData(query, path.ToString());
                        Response.Write("succ:" + now);
                    }
                    catch (Exception ex)
                    {
                        Response.Write("error:" + now + "|" + ex.ToString());
                    }
                }
                //}, path);
            }
            else
            {
                Response.Write("Exists:" + now);
            }
        }

        #region ExportData

        public class ChatMessage
        {
            public string ReceiverID { get; set; }
            public string Queue { get; set; }
            public string UserID { get; set; }
            public string NickName { get; set; }
            public string Message { get; set; }
            public DateTime CreateData { get; set; }
            public string Date
            {
                get
                {
                    return string.Format("{0:yyyy-MM-dd}", this.CreateData);
                }
            }
            public string Time
            {
                get
                {
                    return string.Format("{0:HH:mm:ss}", this.CreateData);
                }
            }

            public string ChatID { get; set; }
            public string CustomerID { get; set; }
            public string CustomerName { get; set; }
        }

        private void ExportData(List<SPhone_Chat> data, string targetFile)
        {
            string documentID = "Xlt101";
            List<ChatMessage> allMsgs = new List<ChatMessage>();
            // 保存文件
            DataTable dt = AsposeHelper.GetTemplateDataTable(documentID);
            data.ForEach(chat =>
            {
                allMsgs.AddRange(GetChatDetails(chat));
            });
            // lst 转 table
            allMsgs.ForEach(msg =>
            {
                DataRow dr = dt.NewRow();
                dr["ChatID"] = msg.ChatID;
                dr["Queue"] = msg.Queue;
                dr["CustomerID"] = msg.CustomerID;
                dr["CustomerName"] = msg.CustomerName;
                dr["Date"] = msg.Date;
                dr["Time"] = msg.Time;
                dr["NickName"] = msg.NickName;
                if (msg.Message != null && msg.Message.IndexOf("<img src=\"data:image/") != -1)
                {
                    dr["Message"] = Tele.Common.Utils.CutStr(Tele.Common.Utils.NoHtml(msg.Message), 1000);
                }
                else if (msg.Message.Length > 1000)
                {
                    dr["Message"] = Tele.Common.Utils.CutStr(Tele.Common.Utils.NoHtml(msg.Message), 1000);
                }
                else
                {
                    dr["Message"] = msg.Message;
                }
                // 
                dt.Rows.Add(dr);
            });

            if (Request.QueryString["debug"] != null)
            {
                var qs = allMsgs.OrderByDescending(x => x.Message.Length).Take(10);
                foreach (var item in qs)
                {
                    Response.Write(item.ChatID + "\r\n");
                }
                return;
            }

            string templateFileName = AsposeHelper.GetTemplateFileName(documentID);
            //string newFileName = string.Format(@"{0}\Documents\TempFolder\{1}.xlsx", this.Request.PhysicalApplicationPath, dt.TableName + DateTime.Now.ToString("-yyMMdd"));
            BatchDownloadResponse response = AsposeHelper.DataTableToExcel(templateFileName, targetFile, dt);
            //AsposeHelper.DownloadFile(newFileName);
        }

        private List<ChatMessage> GetChatDetails(SPhone_Chat chat)
        {
            List<ChatMessage> chatMessages = new List<ChatMessage>();
            try
            {
                string chatContent = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(chat.ContentText));
                if (!string.IsNullOrEmpty(chatContent))
                {
                    chatMessages = JsonConvert.DeserializeObject<List<ChatMessage>>(chatContent);
                    Regex reg = new Regex(@"<\/?(p|font|strong|em)[^>]*>", RegexOptions.IgnoreCase);
                    chatMessages.ForEach(msg =>
                    {
                        msg.ChatID = chat.ChatID;
                        msg.Queue = chat.CurrentQueueName;
                        msg.CustomerID = chat.CustomerID.ToString();
                        msg.CustomerName = chat.CustomerName;

                        msg.Message = HttpUtility.UrlDecode(msg.Message);
                        if (!string.IsNullOrEmpty(msg.Message))
                            msg.Message = reg.Replace(msg.Message, "");
                    });
                }
            }
            catch { }
            return chatMessages;
        }
        #endregion
    }
}