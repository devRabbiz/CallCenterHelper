
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Tele.DataLibrary;
using SoftPhone.Entity;
using System.Linq.Expressions;
using System.Data;
using SoftPhone.Business;
using Newtonsoft.Json;
using Tele.Common;
using System.Net;
using System.Text.RegularExpressions;

namespace Tele.Management.Pages
{
    public partial class ChatList : PageBase<SPhone_Chat>
    {

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                this.txtChatBeginTime.Text = DateTime.Now.ToString("yyyy-MM-dd");
                this.txtChatEndTime.Text = this.txtChatBeginTime.Text;

                //BindRepeater();
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindRepeater();
        }

        protected void pager1_OnPageChanged(object sender, EventArgs e)
        {
            BindRepeater();
        }


        protected void btnExport_Click(object sender, EventArgs e)
        {
            List<SPhone_Chat> data = null;
            List<string> ids = this.hdChatIDs.Value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            using (var db = DCHelper.SPhoneContext())
            {
                data = db.SPhone_Chat.Where(item => ids.Contains(item.ChatID)).ToList();
            }
            ExportData(data);
        }

        protected void btnExportAll_Click(object sender, EventArgs e)
        {
            var searcher = GetSearcher();
            if (searcher.ChatEndTime != null)
                searcher.ChatEndTime = ((DateTime)searcher.ChatEndTime).Date.AddDays(1).AddSeconds(-1);
            IEnumerable<SPhone_Chat> query = null;
            List<SPhone_Chat> data = null;
            using (var db = DCHelper.SPhoneContext())
            {
                query = db.SPhone_Chat.Where(item =>
                    item.ChatBeginTime >= searcher.ChatBeginTime && item.ChatBeginTime <= searcher.ChatEndTime);
                if (!string.IsNullOrEmpty(searcher.ChatID))
                    query = query.Where(item => item.ChatID.IndexOf(searcher.ChatID) != -1);
                if (!string.IsNullOrEmpty(searcher.CustomerID))
                    query = query.Where(item => item.CustomerID.IndexOf(searcher.CustomerID) != -1);
                if (!string.IsNullOrEmpty(searcher.EmployeeID))
                    query = query.Where(item => item.EmployeeID.IndexOf(searcher.EmployeeID) != -1);
                if (!string.IsNullOrEmpty(searcher.PlaceIP))
                    query = query.Where(item => item.PlaceIP.IndexOf(searcher.PlaceIP) != -1);
                if (!string.IsNullOrEmpty(searcher.CurrentQueueName))
                    query = query.Where(item => item.CurrentQueueName.IndexOf(searcher.CurrentQueueName) != -1);
                if (!string.IsNullOrEmpty(searcher.MachineNo))
                    query = query.Where(item => item.MachineNo.IndexOf(searcher.MachineNo) != -1);
                data = query.ToList();
            }
            ExportData(data);
        }
        #endregion

        #region Properties


        #endregion

        protected override void CollectInfo(Dictionary<string, object> context)
        {
            context["EmployeeID"] = this.txtEmployeeID.Text.Trim();
            context["CustomerID"] = this.txtCustomerID.Text.Trim();
            context["ChatID"] = this.txtChatID.Text.Trim();
            context["CurrentQueueName"] = this.txtQueueName.Text.Trim();
            context["MachineNo"] = this.txtMachineNo.Text.Trim();
            context["PlaceIP"] = this.txtPlaceIP.Text.Trim();

            context["ChatBeginTime"] = this.txtChatBeginTime.Text.Trim();
            context["ChatEndTime"] = this.txtChatEndTime.Text.Trim();
        }

        #region BindRepeater

        protected void BindRepeater()
        {
            var searcher = GetSearcher();

            if (searcher.ChatEndTime == null)
            {
                searcher.ChatEndTime = new DateTime(2000, 1, 1);
            }

            if (searcher.ChatBeginTime == null)
            {
                searcher.ChatBeginTime = new DateTime(2000, 1, 1);
            }

            if (searcher.ChatEndTime != null)
            {
                searcher.ChatEndTime = ((DateTime)searcher.ChatEndTime).Date.AddDays(1).AddSeconds(-1);
            }

            using (var db = DCHelper.SPhoneContext())
            {
                string entitySQL = string.Format("SELECT ChatID,EmployeeID,CustomerID,CustomerName,ChatBeginTime,ChatEndTime,'' AS ContentText"
                    + ",ConnectionID,CurrentQueueName,FromQueueName,NextQueueName,MachineNo,PlaceIP,MailAddress,IsConference"
                    + ",IsTransfer,IsRTO,ServicecardNo,WSISID,Enterid,CreateBy,CreateTime,UpdateBy,UpdateTime FROM SPhone_Chat where ChatBeginTime>='{0}' and ChatBeginTime<='{1}';", searcher.ChatBeginTime.Value.ToString("yyyy-MM-dd"), searcher.ChatEndTime.Value.AddDays(1).ToString("yyyy-MM-dd"));
                var query = db.SPhone_Chat.SqlQuery(entitySQL).Where(item =>
                      item.ChatBeginTime >= searcher.ChatBeginTime && item.ChatBeginTime <= searcher.ChatEndTime);
                if (!string.IsNullOrEmpty(searcher.ChatID))
                {
                    query = query.Where(item => item.ChatID.IndexOf(searcher.ChatID) != -1);
                }
                if (!string.IsNullOrEmpty(searcher.CustomerID))
                {
                    query = query.Where(item => item.CustomerID.IndexOf(searcher.CustomerID) != -1);
                }
                if (!string.IsNullOrEmpty(searcher.EmployeeID))
                {
                    query = query.Where(item => item.EmployeeID.IndexOf(searcher.EmployeeID) != -1);
                }
                if (!string.IsNullOrEmpty(searcher.PlaceIP))
                {
                    query = query.Where(item => item.PlaceIP.IndexOf(searcher.PlaceIP) != -1);
                }
                if (!string.IsNullOrEmpty(searcher.CurrentQueueName))
                {
                    query = query.Where(item => item.CurrentQueueName.IndexOf(searcher.CurrentQueueName) != -1);
                }
                if (!string.IsNullOrEmpty(searcher.MachineNo))
                {
                    query = query.Where(item => item.MachineNo.IndexOf(searcher.MachineNo) != -1);
                }

                this.pager1.RecordCount = query.Count();
                this.rptList.DataSource = query.OrderByDescending(item => item.ChatBeginTime)
                    .Skip((this.pager1.CurrentPageIndex - 1) * this.pager1.PageSize).Take(this.pager1.PageSize);
                this.rptList.DataBind();
            }
        }


        #endregion

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

        private void ExportData(List<SPhone_Chat> data)
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
                    dr["Message"] = Tele.Common.Utils.CutStr(Tele.Common.Utils.NoHtml(msg.Message), 1000); ;
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
            string templateFileName = AsposeHelper.GetTemplateFileName(documentID);
            string newFileName = string.Format(@"{0}\Documents\TempFolder\{1}.xlsx", this.Request.PhysicalApplicationPath, dt.TableName);
            BatchDownloadResponse response = AsposeHelper.DataTableToExcel(templateFileName, newFileName, dt);
            AsposeHelper.DownloadFile(newFileName);
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