using SoftPhone.Business;
using SoftPhone.Entity;
using SoftPhone.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using SoftPhone.Entity.Model.cfg;

namespace SoftPhoneDbService.Controllers
{
    public class DbController : BaseController
    {
        //
        // GET: /Db/

        public ActionResult Index()
        {
            return View();
        }

        #region 通过IP获取DN和Place
        /// <summary>
        /// 通过IP获取DN和Place
        /// </summary>
        /// <returns></returns>
        public ActionResult GetPlaceDN()
        {
            var r = new AjaxReturn();
            r.d = SoftPhone.Business.IPDNBLL.GetPlaceDN(Request.UserHostAddress);
            if (r.d == null)
            {
                r.SetError(Request.UserHostAddress + ":没有找到对应的DN和Place");
            }
            return Jsonp(r, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 创建Call记录
        public ActionResult CallCreate(string CallID, string EmployeeID, string ConnectionID, string ANI, string DNIS, int InOut, string CurrentQueueName, string FromQueueName)
        {
            var PlaceIP = Request.UserHostAddress;
            var r = new AjaxReturn();
            try
            {
                Sphone_CallBLL.Create(CallID, EmployeeID, ConnectionID, ANI, DNIS, InOut, CurrentQueueName, FromQueueName, PlaceIP);
            }
            catch (Exception ex)
            {
                r.SetError(ex.Message);
            }
            return Jsonp(r, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 案面(案头工作转换为其他状态时)
        public ActionResult CallSetDeskTime(string CallID, string EmployeeID)
        {
            var r = new AjaxReturn();
            try
            {
                Sphone_CallBLL.SetDeskTime(CallID, EmployeeID);
            }
            catch (Exception ex)
            {
                r.SetError(ex.Message);
            }
            return Jsonp(r, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 挂机
        public ActionResult CallEnd(string CallID, string EmployeeID, string CustomerID, string NextQueueName, int IsConference, int IsTransfer, int IsTransferEPOS)
        {
            var r = new AjaxReturn();
            try
            {
                if (!string.IsNullOrEmpty(CallID))
                {
                    Sphone_CallBLL.CallEnd(CallID, EmployeeID, CustomerID, NextQueueName, IsConference, IsTransfer, IsTransferEPOS);
                }
            }
            catch (Exception ex)
            {
                r.SetError(ex.Message);
            }
            return Jsonp(r, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 创建/更新Chat记录

        /// <summary>
        /// 创建Chat记录
        /// </summary>
        /// <param name="enterID"></param>
        /// <param name="inneractionID"></param>
        /// <param name="fromQueue"></param>
        /// <param name="currentQueue"></param>
        /// <param name="customerID"></param>
        /// <param name="customerName"></param>
        /// <param name="machineNo"></param>
        /// <param name="mailAddress"></param>
        /// <param name="agentID"></param>
        /// <param name="isTransfer"></param>
        /// <param name="isRTO"></param>
        /// <param name="isMeeting"></param>
        /// <returns></returns>
        public ActionResult ChatCreate(string chatID, string enterID, string inneractionID, string fromQueue, string currentQueue
            , string customerID, string customerName, string machineNo, string mailAddress, string cardNo
            , string agentID, long? beginDate, int? wSISID, int isTransfer, int isMeeting)
        {
            var result = new AjaxReturn();
            SPhone_Chat entity = new SPhone_Chat();
            entity.CreateBy = agentID;
            long ticks = Convert.ToInt64(beginDate);
            if (ticks == 0) ticks = DateTime.Now.Ticks;
            entity.CreateTime = DateTime.MinValue.AddMilliseconds(ticks / 10000);
            entity.ChatBeginTime = entity.CreateTime;
            entity.ChatEndTime = entity.CreateTime;

            entity.ChatID = chatID;
            entity.ConnectionID = inneractionID;

            entity.CustomerID = customerID;
            entity.CustomerName = customerName;
            entity.Enterid = enterID;
            entity.MachineNo = machineNo;
            entity.MailAddress = mailAddress;
            entity.ServicecardNo = cardNo;
            entity.WSISID = wSISID;

            entity.EmployeeID = agentID;
            entity.PlaceIP = this.Request.UserHostAddress;
            entity.FromQueueName = fromQueue;
            entity.CurrentQueueName = currentQueue;
            entity.ContentText = string.Empty;

            entity.IsConference = isMeeting;
            entity.IsRTO = 0;
            entity.IsTransfer = isTransfer;
            try
            {
                SPhone_ChatBLL.AddNewChat(entity);
            }
            catch (Exception ex)
            {
                result.SetError(ex.Message);
            }
            return Jsonp(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 更新Chat记录
        /// </summary>
        /// <param name="chatID"></param>
        /// <param name="nextQueue"></param>
        /// <param name="isMeeting"></param>
        /// <param name="jsonMessageData"></param>
        /// <returns></returns>
        public ActionResult ChatUpdate(string chatID, string nextQueue, int isMeeting, int isRTO, string jsonMessageData, string chatEndDate, string customerIP, string customerLocation)
        {
            var result = new AjaxReturn();
            try
            {
                if (string.IsNullOrEmpty(chatID))
                    throw new Exception("传入的参数 chatID 不可为空值！");
                if (!string.IsNullOrEmpty(jsonMessageData))
                    jsonMessageData = jsonMessageData.Trim('"');

                DateTime endTime = DateTime.Now;

                if (!string.IsNullOrEmpty(chatEndDate))
                {
                    try
                    {
                        endTime = DateTime.Parse(chatEndDate.Trim('"'));
                    }
                    catch { }
                }

                //try
                //{
                //    string chatContent = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(jsonMessageData));
                //    if (!string.IsNullOrEmpty(chatContent))
                //    {
                //        // 获取最后一条，作为chat的结束时间
                //    }
                //}
                //catch { }

                SPhone_Chat entity = SPhone_ChatBLL.GetChat(chatID);
                entity.NextQueueName = nextQueue;
                entity.IsConference = isMeeting;
                entity.IsRTO = isRTO;
                entity.ContentText = jsonMessageData;
                entity.UpdateBy = entity.EmployeeID;
                entity.ChatEndTime = endTime;
                entity.UpdateTime = endTime;
                entity.CustomerIP = customerIP;
                entity.IPLocation = customerLocation;
                if (entity == null)
                    throw new Exception(String.Format("找不到需要更新的记录！ ChatID：{0}", chatID));
                SPhone_ChatBLL.Update<SPhone_Chat>(entity);
            }
            catch (Exception ex)
            {
                result.SetError(ex.Message);
            }
            return Jsonp(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ChatEnd(string chatID)
        {
            var result = new AjaxReturn();
            try
            {
                if (string.IsNullOrEmpty(chatID))
                    throw new Exception("传入的参数 chatID 不可为空值！");
                SPhone_Chat entity = SPhone_ChatBLL.GetChat(chatID);
                if (entity == null)
                    throw new Exception(String.Format("找不到需要更新的记录！ ChatID：{0}", chatID));
                entity.UpdateBy = entity.EmployeeID;
                entity.ChatEndTime = DateTime.Now;
                entity.UpdateTime = DateTime.Now;
                SPhone_ChatBLL.Update<SPhone_Chat>(entity);
            }
            catch (Exception ex)
            {
                result.SetError(ex.Message);
            }
            return Jsonp(result, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 查询历史记录

        /// <summary>
        /// 查询历史记录
        /// </summary>
        /// <param name="chatID">聊天ID</param>
        /// <returns></returns>
        public JsonResult GetChatHistoryByID(string chatID)
        {
            List<string> formatMessages = new List<string>();
            try
            {
                SPhone_Chat chat = SPhone_ChatBLL.GetChat(chatID);
                formatMessages = GetChatText(chat, string.Empty);
            }
            catch { }
            return Jsonp(formatMessages, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取历史记录
        /// 2013-06-14 添加根据主机编号检索
        /// </summary>
        /// <param name="employeeID"></param>
        /// <param name="machineNO"></param>
        /// <param name="chatID"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public JsonResult GetChatHistory(string employeeID, string machineNO, string customerID, string beginTime, string endTime)
        {
            List<SPhone_Chat> chats = new List<SPhone_Chat>();
            try
            {
                DateTime beginDate = DateTime.Parse(beginTime);
                DateTime endDate = DateTime.Parse(endTime).AddDays(1).AddSeconds(-1);
                chats = SPhone_ChatBLL.GetChatList(employeeID, beginDate, endDate, machineNO, customerID);
            }
            catch { }
            return Jsonp(chats, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 查询常用语

        public JsonResult GetWelcomeWords(string enterID)
        {
            List<string> words = new List<string>();
            try
            {
                int eid = 9999;
                int.TryParse(enterID, out eid);
                string queueName = SoftPhone.Business.EnterID2SkillBLL.GetSkillName(eid);
                words = SPhone_ChatBLL.GetChatTextList(queueName, "欢迎话术");
                if (words.Count == 0)
                {
                    string defaultWelcomeText = System.Configuration.ConfigurationManager.AppSettings["welcomeText"];
                    if (string.IsNullOrEmpty(defaultWelcomeText)) defaultWelcomeText = "用户，您好！";
                    words.Add(defaultWelcomeText);
                }
            }
            catch { }
            // 替换时间
            string dataKey = Tele.Common.Utils.GetDateKey();
            List<string> realWords = new List<string>();
            words.ForEach(w =>
            {
                string item = w.Replace("$$DataKey$$", dataKey);
                realWords.Add(item);
            });
            return Jsonp(realWords, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 查询常用语
        /// </summary>
        /// <param name="category">常用语类别</param>
        /// <returns></returns>
        public JsonResult GetCommonWords(string queueName)
        {
            List<string> words = SPhone_ChatBLL.GetChatTextList(queueName, "常用话术");
            return Jsonp(words, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取常用链接列表
        /// </summary>
        public JsonResult GetChatLinks(string queueName)
        {
            List<string> texts = new List<string>();
            try
            {
                texts = SPhone_ChatBLL.GetChatTextList(queueName, "常用链接");
            }
            catch { }
            return Jsonp(texts, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取话术列表
        /// typeName 先进行UrlEncode转码
        /// </summary>
        public JsonResult GetChatTexts(string queueName, string typeName)
        {
            List<string> texts = new List<string>();
            try
            {
                typeName = HttpUtility.UrlDecode(typeName);
                texts = SPhone_ChatBLL.GetChatTextList(queueName, typeName);
            }
            catch { }
            return Jsonp(texts, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 登录系统
        public ActionResult Login(string EmployeeID)
        {
            var r = SPhone_LoginLogBLL.Login(EmployeeID);
            return Jsonp(1, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 注销
        public ActionResult Logout(string EmployeeID)
        {
            var r = SPhone_LoginLogBLL.Logout(EmployeeID);
            return Jsonp(r, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 登录、首页
        //判断员工编号是否正确
        public ActionResult Authenticate(string employeeId)
        {
            var r = new LoginResult();
            r.EventAuthenticated = ProcBLL.Proc_PersonExists(employeeId) == 1;
            if (!r.EventAuthenticated)
            {
                r.ErrorMessage = "没有找到员工编号";
            }
            return Jsonp(r, JsonRequestBehavior.AllowGet);
        }

        //获取员工坐席信息
        public ActionResult GetAgentInfo(string employee_id)
        {
            var r = new InitInfoVM();
            var agent = ProcBLL.Proc_GetPersonAgentInfo(employee_id);
            if (agent == null)
            {
                r.ErrCode = -1;
                r.ErrMessage = "没有找到对应的员工。";
            }
            else
            {
                r.Person.AgentInfo.IsInitAgentInfo = true;

                r.Person.DN = "";
                r.Person.Place = "";
                r.Person.DBID = agent.person_dbid.Value;
                r.Person.EmployeeID = agent.employee_id;
                r.Person.FirstName = agent.first_name;
                r.Person.UserName = agent.user_name;
                r.Person.LoginCode = agent.login_code;

                r.Person.CHAT = agent.chat.Value;
                r.Person.VOICE = 1;

                r.Person.AgentInfo.AgentLogins.Add(new AgentLogin()
                {
                    DBID = agent.login_dbid.Value,
                    LoginCode = agent.login_code
                });

                var skillLevels = ProcBLL.Proc_GetPersonSkills(r.Person.DBID);
                foreach (var item in skillLevels)
                {
                    r.Person.AgentInfo.SkillLevels.Add(new SkillLevel() { DBID = item.skill_dbid.Value, Level = item.level.Value });
                }

                r.DN = "";
                r.Place = "";

                r.AgentID = agent.login_code;
                r.FirstName = agent.first_name;
                r.EmployeeID = agent.employee_id;
                r.EnableVoice = r.Person.VOICE > 0;
                r.EnableChat = r.Person.CHAT > 0;

            }
            return Jsonp(r, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 记录坐席状态更改(后续跟进)
        public ActionResult ProcessAgentStatus(string logID, string employeeID, Nullable<int> typeID, Nullable<int> insertOrUpdate)
        {
            var r = ProcBLL.Proc_AgentStatusChangeLog(logID, employeeID, typeID, insertOrUpdate);
            return Jsonp(r, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 通过主机编号获取正确的EnterID
        public ActionResult GetChatRightEnterID(int enterID, string machineNo)
        {
            AjaxReturn r = new AjaxReturn();
            try
            {
                if (!string.IsNullOrEmpty(machineNo))
                {
                    r.d = ProcBLL.Proc_GetChatRightEnterID(enterID, machineNo);
                }
                else
                {
                    r.SetError("machineNo是必须的");
                }
            }
            catch (Exception ex)
            {
                r.SetError(ex.Message);
            }
            return Json(r, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Private Methods

        private List<string> GetChatText(SPhone_Chat chat, string keyword)
        {
            List<string> msgList = new List<string>();
            try
            {
                string chatContent = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(chat.ContentText));
                msgList.Add(string.Format("========= {0} ========<br />", chat.ChatID));
                if (!string.IsNullOrEmpty(chatContent))
                {
                    List<Chat.Common.ChatMessage> chatMessages = JsonConvert.DeserializeObject<List<Chat.Common.ChatMessage>>(chatContent);
                    if (!string.IsNullOrEmpty(keyword))
                        chatMessages = chatMessages.FindAll(item => item.Message.IndexOf(keyword) != -1);
                    chatMessages.ForEach(msg => msgList.Add(msg.FormatMessage));

                    msgList.Add("<br />");
                }
            }
            catch { }
            return msgList;
        }


        #endregion

        /// <summary>
        /// 根据客户名称查看是否在黑名单里
        /// </summary>
        /// <param name="CustName"></param>
        /// <returns></returns>
        public ActionResult IsChatInBlack(string CustName)
        {
            List<int> result = new List<int>();
            try
            {
                result.Add(SPhone_ChatBLL.IsChatInBlack(CustName));
            }
            catch
            {

            }
            return Jsonp(result, JsonRequestBehavior.AllowGet);
        }
    }
}
