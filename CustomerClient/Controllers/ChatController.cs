using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Web;
using System.Web.Mvc;
using Chat.Common;
using Chat.CustomerInterface;
using CustomerClient.Models;
using Newtonsoft.Json;

namespace CustomerClient.Controllers
{
    public class ChatController : Controller
    {
        #region 用户界面

        private bool IsUserAgent()
        {
            var ua = Request.UserAgent ?? string.Empty;
            if (ua.Length < 13)
            {
                return false;
            }
            return true;
        }

        private bool IsXMLRequest()
        {
            return Request.Headers["X-Requested-With"] == "XMLHttpRequest";
        }

        private static string CHAT_SESSION_KEY = "CHAT_SESSION";

        /// <summary>
        /// 用户请求进入聊天主界面
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        //[HttpPost]
        [ValidateInput(false)]
        public ActionResult Chatting(string UserID, string TargetSkill, string UserName
            , string EnterID, string strServiceCardNo, string MachineNo, string RegisterNumber
            , string Queue, string LAStatID, string customerIP, string customerLocation, string emailClient, string WSISID, string PreText)
        {
            string value = Session[CHAT_SESSION_KEY] as string;
            if (!string.IsNullOrEmpty(value))
            {
                ChatLog.GetInstance().FormatMessage("系统检测到用户刷新了页面或开启了多个Chat窗口。");
                System.Threading.Thread.Sleep(500);
            }
            Session[CHAT_SESSION_KEY] = DateTime.Now.ToShortDateString();

            // 呈现
            Models.IndexView model = new Models.IndexView();
            try
            {
                if (this.Request.UrlReferrer != null)
                {
                    ChatLog.GetInstance().FormatMessage("请求来源：{0}", this.Request.UrlReferrer.OriginalString);
                }
                ChatLog.GetInstance().FormatMessage("客户端IP地址：{0}", this.Request.UserHostAddress);
                if (string.IsNullOrEmpty(EnterID))
                {
                    ChatLog.GetInstance().FormatMessage("无效的请求，参数EnterID为空。自动转接到Lenovo专家在线队列。");
                    EnterID = "1003";
                }

                var rightEnterID = int.Parse(EnterID);

                try
                {
                    rightEnterID = int.Parse(EnterID);
                    var r = GetChatRightEnterID(rightEnterID, MachineNo);
                    if (string.IsNullOrEmpty(LAStatID))
                    {
                        if (r.Type == 2)
                        {
                            LAStatID = r.Return;
                        }
                    }
                    if (!string.IsNullOrEmpty(MachineNo))
                    {
                        if (r.Type == 1)
                        {
                            rightEnterID = int.Parse(r.Return);
                        }
                    }
                }
                catch { }

                // 获取URL中的参数
                Dictionary<string, object> parames = new Dictionary<string, object>();
                parames["UserID"] = UserID;
                parames["TargetSkill"] = TargetSkill;
                parames["UserName"] = HttpUtility.UrlDecode(UserName);
                parames["EnterID"] = rightEnterID.ToString();
          //      if (strServiceCardNo == "")
               // {
               //     parames["strServiceCardNo"] = UserName;
               // }
               // else
               // {
                    parames["strServiceCardNo"] = strServiceCardNo;
               // }
                parames["MachineNo"] = MachineNo;
                parames["RegisterNumber"] = RegisterNumber;
                parames["Queue"] = Queue;
                parames["LAStatID"] = LAStatID;
                parames["emailClient"] = emailClient;
                parames["WSISID"] = WSISID;

                parames["CustomerIP"] = this.Request.UserHostAddress;
                parames["CustomerLocation"] = customerLocation;

                model.CurrentQueueName = TargetSkill;
                if (!string.IsNullOrEmpty(PreText))
                    model.RabitPreText = HttpUtility.UrlEncode(HttpUtility.HtmlDecode(PreText));
                model.ClientUserData = JsonConvert.SerializeObject(parames);

            }
            catch { }
            return View(model);
        }

        [ValidateInput(false)]
        public ActionResult Chatting_baidu(string UserID, string TargetSkill, string UserName
            , string EnterID, string strServiceCardNo, string MachineNo, string RegisterNumber
            , string Queue, string LAStatID, string emailClient, string WSISID, string PreText)
        {
            string value = Session[CHAT_SESSION_KEY] as string;
            if (!string.IsNullOrEmpty(value))
            {
                ChatLog.GetInstance().FormatMessage("系统检测到用户刷新了页面或开启了多个Chat窗口。");
                System.Threading.Thread.Sleep(500);
            }
            Session[CHAT_SESSION_KEY] = DateTime.Now.ToShortDateString();

            // 呈现
            Models.IndexView model = new Models.IndexView();
            try
            {
                if (this.Request.UrlReferrer != null)
                {
                    ChatLog.GetInstance().FormatMessage("请求来源：{0}", this.Request.UrlReferrer.OriginalString);
                }
                ChatLog.GetInstance().FormatMessage("客户端IP地址：{0}", this.Request.UserHostAddress);
                if (string.IsNullOrEmpty(EnterID))
                {
                    ChatLog.GetInstance().FormatMessage("无效的请求，参数EnterID为空。自动转接到Lenovo专家在线队列。");
                    EnterID = "1003";
                }

                var rightEnterID = int.Parse(EnterID);

                try
                {
                    rightEnterID = int.Parse(EnterID);
                    var r = GetChatRightEnterID(rightEnterID, MachineNo);
                    if (string.IsNullOrEmpty(LAStatID))
                    {
                        if (r.Type == 2)
                        {
                            LAStatID = r.Return;
                        }
                    }
                    if (!string.IsNullOrEmpty(MachineNo))
                    {
                        if (r.Type == 1)
                        {
                            rightEnterID = int.Parse(r.Return);
                        }
                    }
                }
                catch { }

                // 获取URL中的参数
                Dictionary<string, object> parames = new Dictionary<string, object>();
                parames["UserID"] = UserID;
                parames["TargetSkill"] = TargetSkill;
                parames["UserName"] = HttpUtility.UrlDecode(UserName);
                parames["EnterID"] = rightEnterID.ToString();
                parames["strServiceCardNo"] = strServiceCardNo;
                parames["MachineNo"] = MachineNo;
                parames["RegisterNumber"] = RegisterNumber;
                parames["Queue"] = Queue;
                parames["LAStatID"] = LAStatID;
                parames["emailClient"] = emailClient;
                parames["WSISID"] = WSISID;

                model.CurrentQueueName = TargetSkill;
                if (!string.IsNullOrEmpty(PreText))
                    model.RabitPreText = HttpUtility.UrlEncode(HttpUtility.HtmlDecode(PreText));
                model.ClientUserData = JsonConvert.SerializeObject(parames);
            }
            catch { }
            return View(model);
        }


        [ValidateInput(false)]
        public ActionResult Chatting_wap(string UserID, string TargetSkill, string UserName
            , string EnterID, string strServiceCardNo, string MachineNo, string RegisterNumber
            , string Queue, string LAStatID, string customerIP, string customerLocation, string emailClient, string WSISID, string PreText)
        {
            string value = Session[CHAT_SESSION_KEY] as string;
            if (!string.IsNullOrEmpty(value))
            {
                ChatLog.GetInstance().FormatMessage("系统检测到用户刷新了页面或开启了多个Chat窗口。");
                System.Threading.Thread.Sleep(500);
            }
            Session[CHAT_SESSION_KEY] = DateTime.Now.ToShortDateString();

            // 呈现
            Models.IndexView model = new Models.IndexView();
            try
            {
                if (this.Request.UrlReferrer != null)
                {
                    ChatLog.GetInstance().FormatMessage("请求来源：{0}", this.Request.UrlReferrer.OriginalString);
                }
                ChatLog.GetInstance().FormatMessage("客户端IP地址：{0}", this.Request.UserHostAddress);
                if (string.IsNullOrEmpty(EnterID))
                {
                    ChatLog.GetInstance().FormatMessage("无效的请求，参数EnterID为空。自动转接到Lenovo专家在线队列。");
                    EnterID = "1003";
                }

                var rightEnterID = int.Parse(EnterID);

                try
                {
                    rightEnterID = int.Parse(EnterID);
                    var r = GetChatRightEnterID(rightEnterID, MachineNo);
                    if (string.IsNullOrEmpty(LAStatID))
                    {
                        if (r.Type == 2)
                        {
                            LAStatID = r.Return;
                        }
                    }
                    if (!string.IsNullOrEmpty(MachineNo))
                    {
                        if (r.Type == 1)
                        {
                            rightEnterID = int.Parse(r.Return);
                        }
                    }
                }
                catch { }

                // 获取URL中的参数
                Dictionary<string, object> parames = new Dictionary<string, object>();
                parames["UserID"] = UserID;
                parames["TargetSkill"] = TargetSkill;
                parames["UserName"] = HttpUtility.UrlDecode(UserName);
                parames["EnterID"] = rightEnterID.ToString();
                parames["strServiceCardNo"] = strServiceCardNo;
                parames["MachineNo"] = MachineNo;
                parames["RegisterNumber"] = RegisterNumber;
                parames["Queue"] = Queue;
                parames["LAStatID"] = LAStatID;
                parames["emailClient"] = emailClient;
                parames["WSISID"] = WSISID;

                parames["CustomerIP"] = this.Request.UserHostAddress;
                parames["CustomerLocation"] = customerLocation;

                model.CurrentQueueName = TargetSkill;
                if (!string.IsNullOrEmpty(PreText))
                    model.RabitPreText = HttpUtility.UrlEncode(HttpUtility.HtmlDecode(PreText));
                model.ClientUserData = JsonConvert.SerializeObject(parames);
            }
            catch 
            {

            }
            return View(model);
        }

        [ValidateInput(false)]
        public ActionResult Chatting_html5(string UserID, string TargetSkill, string UserName
            , string EnterID, string strServiceCardNo, string MachineNo, string RegisterNumber
            , string Queue, string LAStatID, string customerIP, string customerLocation, string emailClient, string WSISID, string PreText)
        {
            string value = Session[CHAT_SESSION_KEY] as string;
            if (!string.IsNullOrEmpty(value))
            {
                ChatLog.GetInstance().FormatMessage("系统检测到用户刷新了页面或开启了多个Chat窗口。");
                System.Threading.Thread.Sleep(500);
            }
            Session[CHAT_SESSION_KEY] = DateTime.Now.ToShortDateString();

            // 呈现
            Models.IndexView model = new Models.IndexView();
            try
            {
                if (this.Request.UrlReferrer != null)
                {
                    ChatLog.GetInstance().FormatMessage("请求来源：{0}", this.Request.UrlReferrer.OriginalString);
                }
                ChatLog.GetInstance().FormatMessage("客户端IP地址：{0}", this.Request.UserHostAddress);
                if (string.IsNullOrEmpty(EnterID))
                {
                    ChatLog.GetInstance().FormatMessage("无效的请求，参数EnterID为空。自动转接到Lenovo专家在线队列。");
                    EnterID = "1003";
                }

                var rightEnterID = int.Parse(EnterID);

                try
                {
                    rightEnterID = int.Parse(EnterID);
                    var r = GetChatRightEnterID(rightEnterID, MachineNo);
                    if (string.IsNullOrEmpty(LAStatID))
                    {
                        if (r.Type == 2)
                        {
                            LAStatID = r.Return;
                        }
                    }
                    if (!string.IsNullOrEmpty(MachineNo))
                    {
                        if (r.Type == 1)
                        {
                            rightEnterID = int.Parse(r.Return);
                        }
                    }
                }
                catch { }

                // 获取URL中的参数
                Dictionary<string, object> parames = new Dictionary<string, object>();
                parames["UserID"] = UserID;
                parames["TargetSkill"] = TargetSkill;
                parames["UserName"] = HttpUtility.UrlDecode(UserName);
                parames["EnterID"] = rightEnterID.ToString();
                parames["strServiceCardNo"] = strServiceCardNo;
                parames["MachineNo"] = MachineNo;
                parames["RegisterNumber"] = RegisterNumber;
                parames["Queue"] = Queue;
                parames["LAStatID"] = LAStatID;
                parames["emailClient"] = emailClient;
                parames["WSISID"] = WSISID;

                parames["CustomerIP"] = this.Request.UserHostAddress;
                parames["CustomerLocation"] = customerLocation;

                model.CurrentQueueName = TargetSkill;
                if (!string.IsNullOrEmpty(PreText))
                    model.RabitPreText = HttpUtility.UrlEncode(HttpUtility.HtmlDecode(PreText));
                model.ClientUserData = JsonConvert.SerializeObject(parames);
            }
            catch { }
            return View(model);
        }


        public ActionResult Chatting_Mobile(string UserID, string TargetSkill, string UserName
    , string EnterID, string strServiceCardNo, string MachineNo, string RegisterNumber
    , string Queue, string LAStatID, string customerIP, string customerLocation, string emailClient, string WSISID, string PreText)
        {
            string value = Session[CHAT_SESSION_KEY] as string;
            if (!string.IsNullOrEmpty(value))
            {
                ChatLog.GetInstance().FormatMessage("系统检测到用户刷新了页面或开启了多个Chat窗口。");
                System.Threading.Thread.Sleep(500);
            }
            Session[CHAT_SESSION_KEY] = DateTime.Now.ToShortDateString();

            // 呈现
            Models.IndexView model = new Models.IndexView();
            try
            {
                if (this.Request.UrlReferrer != null)
                {
                    ChatLog.GetInstance().FormatMessage("请求来源：{0}", this.Request.UrlReferrer.OriginalString);
                }
                ChatLog.GetInstance().FormatMessage("客户端IP地址：{0}", this.Request.UserHostAddress);
                if (string.IsNullOrEmpty(EnterID))
                {
                    ChatLog.GetInstance().FormatMessage("无效的请求，参数EnterID为空。自动转接到Lenovo专家在线队列。");
                    EnterID = "1003";
                }

                var rightEnterID = int.Parse(EnterID);

                try
                {
                    rightEnterID = int.Parse(EnterID);
                    var r = GetChatRightEnterID(rightEnterID, MachineNo);
                    if (string.IsNullOrEmpty(LAStatID))
                    {
                        if (r.Type == 2)
                        {
                            LAStatID = r.Return;
                        }
                    }
                    if (!string.IsNullOrEmpty(MachineNo))
                    {
                        if (r.Type == 1)
                        {
                            rightEnterID = int.Parse(r.Return);
                        }
                    }
                }
                catch { }

                // 获取URL中的参数
                Dictionary<string, object> parames = new Dictionary<string, object>();
                parames["UserID"] = UserID;
                parames["TargetSkill"] = TargetSkill;
                parames["UserName"] = HttpUtility.UrlDecode(UserName);
                parames["EnterID"] = rightEnterID.ToString();
                //      if (strServiceCardNo == "")
                // {
                //     parames["strServiceCardNo"] = UserName;
                // }
                // else
                // {
                parames["strServiceCardNo"] = strServiceCardNo;
                // }
                parames["MachineNo"] = MachineNo;
                parames["RegisterNumber"] = RegisterNumber;
                parames["Queue"] = Queue;
                parames["LAStatID"] = LAStatID;
                parames["emailClient"] = emailClient;
                parames["WSISID"] = WSISID;

                parames["CustomerIP"] = this.Request.UserHostAddress;
                parames["CustomerLocation"] = customerLocation;

                model.CurrentQueueName = TargetSkill;
                if (!string.IsNullOrEmpty(PreText))
                    model.RabitPreText = HttpUtility.UrlEncode(HttpUtility.HtmlDecode(PreText));
                model.ClientUserData = JsonConvert.SerializeObject(parames);

            }
            catch { }
            return View(model);
        }




        public ActionResult ChatRTO(string accessCode)
        {
            ChatRTOView model = new ChatRTOView();
            model.AccessCode = accessCode;
            return View(model);
        }

        #endregion

        #region 连接服务器

        public JsonResult GetWelcomeText(string enterID)
        {
            List<string> result = new List<string>();
            if (!IsUserAgent())
            {
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            try
            {
                string appServerDb = ConfigurationManager.AppSettings["appServerDb"];
                Uri uri = new Uri(string.Format("{0}Db/GetWelcomeWords?enterID={1}&ss={2}&jsoncallback?"
                    , appServerDb, enterID, DateTime.Now.Millisecond));

                result = ChatHelper.RemoteRequest<List<string>>(uri);
                List<string> realResult = new List<string>();
                if (result != null && result.Count > 0)
                {
                    result.FindAll(item => !string.IsNullOrEmpty(item))
                        .ForEach(item => { realResult.Add(item.Replace("\r\n", "<br />")); });
                    result = realResult;
                }
                else 
                {
                    ChatLog.GetInstance().FormatMessage("Error02:获取欢迎话术未渠道值，方法:ChatController:GetWelcomeText，enterID:{0}", enterID);
                }
            }
            catch (Exception ex)
            {
                ChatLog.GetInstance().LogException(ex);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 黑名单
        /// </summary>
        /// <param name="CustName"></param>
        /// <returns></returns>
        public JsonResult IsChatInBlack(string CustName)
        {
            var result = 0;
            try
            {
                string appServerDb = ConfigurationManager.AppSettings["appServerDb"];
                Uri uri = new Uri(string.Format("{0}Db/IsChatInBlack?CustName={1}&ss={2}&jsoncallback?"
                    , appServerDb, CustName, DateTime.Now.Millisecond));
                var a = ChatHelper.RemoteRequest<List<int>>(uri);
                if (a.Count > 0)
                {
                    result = a[0];
                }
            }
            catch (Exception ex)
            {
                ChatLog.GetInstance().LogException(ex);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult WaitForService(string sessionID, string enterID)
        {
            if (!IsUserAgent() || !IsXMLRequest())
            {
                return Json(new AjaxReturn() { Code = -1, Message = "" }, JsonRequestBehavior.AllowGet);
            }
            string appServer = ConfigurationManager.AppSettings["appServer"];
            Uri uri = new Uri(string.Format("{0}ChatStat/GetQueneCountByEnterID?enterID={1}&ss={2}&jsoncallback?"
                , appServer, enterID, DateTime.Now.Millisecond));

            AjaxReturn result = ChatHelper.RemoteRequest<AjaxReturn>(uri);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 链接ChatServer服务器
        /// </summary>
        /// <returns></returns>
        public JsonResult ConnectionServer(string sessionID, string queryString)
        {
            //上海问题.日志记录 2015.03.11
            ChatLog.GetInstance().Info("SH_BUG|ConnectionServer|{{【sessionID】:" + sessionID +
                ",【Method】:" + Request.HttpMethod +
                ",【IP】:" + Request.UserHostAddress +
                ",【QueryString】:" + Server.UrlDecode(Request.QueryString.ToString()) +
                ",【Form】:" + Server.UrlDecode(Request.Form.ToString()) +
                ",【Header】:" + Server.UrlDecode(Request.Headers.ToString()) +
                "}}");

            if (!IsUserAgent() || !IsXMLRequest())
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
            string status = "正在尝试连接……";
            try
            {
                // 根据当前会话ID，找到所在聊天室
                CustomerChatContext context = new CustomerChatContext(sessionID, queryString);
                context.InitConnecton();
                if (context.IsAvailableConnection) status = "OK.";
            }
            catch (Exception ex)
            {
                ChatLog.GetInstance().LogException(ex);
            }
            return Json(status, JsonRequestBehavior.AllowGet);
        }

        private GetChatRightEnterID_Result GetChatRightEnterID(int enterID, string machineNo)
        {
            if (!string.IsNullOrEmpty(machineNo))
            {
                string appServer = ConfigurationManager.AppSettings["appServerDb"];
                Uri uri = new Uri(string.Format("{0}Db/GetChatRightEnterID?enterID={1}&machineNo={2}"
                    , appServer, enterID, machineNo));
                AjaxReturn result = ChatHelper.RemoteRequest<AjaxReturn>(uri);
                if (result.Code == 0)
                {
                    return new GetChatRightEnterID_Result()
                    {
                        Return = result.d.Return.ToString(),
                        Type = result.d.Type
                    };
                }
            }
            return new GetChatRightEnterID_Result() { Return = enterID.ToString(), Type = 1 };
        }

        #endregion

        #region 聊天

        /// <summary>
        /// 获取信息
        /// </summary>
        /// <returns></returns>
        public JsonResult GetInfo(string sessionID)
        {
            ClientChatMsgList msgList = new ClientChatMsgList();
            if (!IsUserAgent() || !IsXMLRequest())
            {
                return Json(msgList, JsonRequestBehavior.AllowGet);
            }
            try
            {
                CustomerChatContext context = new CustomerChatContext(sessionID);
                msgList.Init(context);
            }
            catch (Exception ex)
            {
                ChatLog.GetInstance().LogException(ex);
            }
            return Json(msgList, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 获取信息
        /// </summary>
        /// <returns></returns>
        public JsonResult GetInfo_wap(string sessionID)
        {
            ClientChatMsgList msgList = new ClientChatMsgList();
            if (!IsUserAgent() || !IsXMLRequest())
            {
                return Json(msgList, JsonRequestBehavior.AllowGet);
            }
            try
            {
                CustomerChatContext context = new CustomerChatContext(sessionID);
                msgList.Init(context, true);
            }
            catch (Exception ex)
            {
                ChatLog.GetInstance().LogException(ex);
            }
            return Json(msgList, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 发送信息
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public JsonResult SendMessage(string sessionID, string message)
        {
            //上海问题.日志记录 2015.03.11
            ChatLog.GetInstance().Info("SH_BUG|SendMessage|{{【sessionID】:" + sessionID +
                ",【Method】:" + Request.HttpMethod +
                ",【IP】:" + Request.UserHostAddress +
                ",【QueryString】:" + Server.UrlDecode(Request.QueryString.ToString()) +
                ",【Form】:" + Server.UrlDecode(Request.Form.ToString()) +
                ",【Header】:" + Server.UrlDecode(Request.Headers.ToString()) +
                "}}");

            if (!IsUserAgent() || !IsXMLRequest())
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
            var context = new CustomerChatContext(sessionID);
            try
            {
                message = SafetyHtml(message);
                context.SendMessage(message);
            }
            catch (Exception ex)
            {
                ChatLog.GetInstance().LogException(ex);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 将html中含有危险的标记转换为安全的标记
        /// </summary>
        private string SafetyHtml(string parameter)
        {
            string strNewValue = parameter;
            string[] blackListkeywork = { "and", "or", "delete", "truncate", "select", "exec", "create", "alter", "union", "insert", "update", "drop", "script", "iframe", "onload=", "alert(", "javascript:", "Document.", "onerror", "onload", "onBlur", "onClick", "onDblClick", "onFocus", "onKeyDown", "onKeyPress", "onKeyUp", "onMouseDown", "onMouseMove", "onMouseOut", "onMouseOver", "onMouseUp" };
            string[] blackListContain = { "'", "<", ">",")","(","&","+" };

            for (int i = 0; i < blackListContain.Length; i++)
            {
                if ((strNewValue.IndexOf(blackListContain[i], StringComparison.OrdinalIgnoreCase) >= 0))
                {
                    strNewValue = strNewValue.Replace(blackListContain[i], "");
                }
            }
            for (int i = 0; i < blackListkeywork.Length; i++)
            {
                if ((strNewValue.IndexOf(blackListkeywork[i], StringComparison.OrdinalIgnoreCase) >= 0))//含有字符串
                {
                    strNewValue = strNewValue.Replace(blackListkeywork[i], "");
                }
            }
            return strNewValue;
        }

        /// <summary>
        /// 发送输入状态
        /// Paused/Typing
        /// </summary>
        /// <param name="status"></param>
        public JsonResult UpdateTypingStatus(string sessionID, int isStarted)
        {
            if (!IsUserAgent() || !IsXMLRequest())
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
            try
            {
                var context = new CustomerChatContext(sessionID);
                context.SendTypingNotice(isStarted);
            }
            catch (Exception ex)
            {
                ChatLog.GetInstance().LogException(ex);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 离开聊天室

        /// <summary>
        /// 离开聊天
        /// </summary>
        /// <returns></returns>
        public JsonResult LeftChat(string sessionID)
        {
            if (!IsUserAgent() || !IsXMLRequest())
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
            try
            {
                var context = new CustomerChatContext(sessionID);
                context.LeftChat();
            }
            catch (Exception ex)
            {
                ChatLog.GetInstance().LogException(ex);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 文件上传

        /// <summary>
        /// 文件上传
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public void FileUpload()
        {
            if (!IsUserAgent())
            {
                return;
            }
            try
            {
                UploadModel model = new UploadModel();
                var result = new { Flag = false, Message = "上传文件失败。" };
                if (this.Request.Files.Count > 0)
                {
                    var file = this.Request.Files[0];

                    if (file.ContentLength > 1024 * 1024 * 2)
                    {
                        model.ReturnFlag = false;
                        model.Message = "文件不能大于2M！";
                    }
                    else
                    {
                        //model = ChatHelper.UploadFile(file, Tele.Common.UNCConfig.GetCfg());
                        model = ChatHelper.UploadFile(file);
                    }
                }
                else
                {
                    model.ReturnFlag = false;
                    model.Message = "找不到可以上传的文件！";
                }
                string jsonData = JsonConvert.SerializeObject(model);
                Response.Write(jsonData);
            }
            catch (Exception ex)
            {
                ChatLog.GetInstance().LogException(ex);
            }
        }

        #endregion

    }


}
