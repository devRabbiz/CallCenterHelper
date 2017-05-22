using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Chat.Common;
using Chat.AgentInterface;
using Newtonsoft.Json;
using System.Net;
using System.Configuration;
using System.Diagnostics;

namespace AgentClient.Controllers
{
    public class ChatController : Controller
    {
        #region 页面视图

        /// <summary>
        /// 聊天首页
        /// </summary>
        public ActionResult Index(string interactionID)
        {
            return View();
        }

        #endregion


        #region 连接聊天室

        /// <summary>
        /// 链接服务器
        /// </summary>
        /// <returns></returns>
        public JsonResult ConnectionServer(string tid, string interactionID, string queueName
            , string agentID, string placeID, string nickName, string host, string port, int isMeeting)
        {
            AjaxReturn result = new AjaxReturn();
            if (!string.IsNullOrEmpty(interactionID))
            {
                LenovoAgent agent = new LenovoAgent() { TicketID = tid, InteractionID = interactionID, AgentId = agentID, PlaceId = placeID, DisplayName = nickName };
                try
                {
                    ChatLog.GetInstance().FormatMessage("连接ChatServer：sessionid-{0},queue-{1},agentID-{2},chatServer-{3}:{4}"
                        , interactionID, queueName, agentID, host, port);

                    AgentChatContext context = new AgentChatContext(tid, interactionID, agent);
                    context.ChatServerHost = host;
                    context.ChatServerPort = port;
                    context.InitConnecton();
                    if (context.IsAvailableConnection)
                    {
                        string title = GetEmployeeTitle(queueName);
                        context.ChatJoin(interactionID, title);

                        if (isMeeting == 1)
                            context.SendMessage(interactionID, "<font color='red'>您已进入了多方支持。</font>");

                        result.Code = 1;

                        DateTime beginTime =context.Room.CreateDate;
                        result.d = new
                        {
                            ChatBeginTime = beginTime.Ticks,
                            ChatID = string.Format("{0}{1:yyyyMMddHHmmsshh}C", agentID, beginTime),
                            StrChatBeginTime = beginTime.ToString("yyyy-MM-dd HH:mm:ss")
                        };

                        //JsonResult jr = new JsonpResult() { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                        //string s=jr.ToString();                        
                        //ChatLog.GetInstance().FormatMessage("返回数据：【{0}】", s);
                    }
                }
                catch (Exception ex)
                {
                    ChatLog.GetInstance().LogException(ex);
                }
            }
            return new JsonpResult() { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

           /// <summary>
        /// 获取聊天室信息
        /// </summary>
        /// <returns></returns>
        public JsonResult GetChatinfo(string tid, string interactionID, string queueName
            , string agentID, string placeID, string nickName, string host, string port, int isMeeting)
        {
            AjaxReturn result = new AjaxReturn();
            ChatRoom Myroom = ChatRoomCache.GetInstance().GetRoom(tid, interactionID);
            if (Myroom !=null)
            {
                result.Code = 1;
                result.d = new
                {
                    ChatBeginTime = Myroom.CreateDate.Ticks,
                    ChatID = string.Format("{0}{1:yyyyMMddHHmmss}C", agentID, Myroom.CreateDate),
                    StrChatBeginTime = Myroom.CreateDate.ToString("yyyy-MM-dd HH:mm:ss")
                };
            }         
            return new JsonpResult() { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        #endregion

        #region 聊天

        /// <summary>
        /// 获取信息
        /// </summary>
        /// <returns></returns>
        public JsonResult GetInfo(string agentID)
        {
            AgentIndexMessage indexData = new AgentIndexMessage();
            if (!string.IsNullOrEmpty(agentID))
            {
                try
                {
                    List<LenovoAgent> agents = AgentCache.GetInstance().GetAgents(agentID);
                    indexData.Init(agents);
                }
                catch (Exception ex)
                {
                    ChatLog.GetInstance().LogException(ex);
                }
            }
            return new JsonpResult() { Data = indexData, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        /// <summary>
        /// 发送信息
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public JsonResult SendMessage(string tid, string message, string interactionID, string agentID)
        {
            if (!string.IsNullOrEmpty(interactionID))
            {
                try
                {
                    var context = new AgentChatContext(tid, interactionID, agentID);
                    context.SendMessage(interactionID, message);
                }
                catch (Exception ex)
                {
                    ChatLog.GetInstance().LogException(ex);
                }
            }
            return new JsonpResult() { Data = "", JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        #endregion

        #region 离开聊天室

        /// <summary>
        /// 离开聊天
        /// </summary>
        /// <returns></returns>
        public JsonResult LeftChat(string tid, string interactionID, string agentID, string queueName, int keepAlive)
        {
            string chatContent = string.Empty;
            if (!string.IsNullOrEmpty(interactionID))
            {
                try
                {
                    AgentChatContext context = new AgentChatContext(tid, interactionID, agentID);
                    bool isKeepRoomAlive = (keepAlive == 1);
                    context.LeftChat(isKeepRoomAlive);
                    try
                    {
                        System.Threading.Thread.Sleep(100);
                        chatContent = JsonConvert.SerializeObject(context.Room.Messages);
                    }
                    catch (Exception ex)
                    {
                        ChatLog.GetInstance().FormatMessage("异常信息：序列化聊天消息时异常。{0}", ex.Message);
                    }
                    chatContent = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(chatContent));

                    if (!isKeepRoomAlive)
                    {
                        // 移除缓存项
                        AgentCache.GetInstance().RemoveAgent(tid, agentID, interactionID);

                        // Room从缓存移除
                        if (context.Room != null)
                        {
                            ChatRoomCache.GetInstance().RemoveItem(context.Room);
                            ChatLog.GetInstance().FormatMessage("Room移除缓存。TicketID:{0},SessionID:{1},AgentID:{2}", tid, interactionID, agentID);
                        }
                    }
                    // 记录日志
                    if (!string.IsNullOrEmpty(queueName))
                        ChatLog.GetInstance().LogEvent(ChatEvent.TransferQueue, queueName);
                    else if (isKeepRoomAlive)
                        ChatLog.GetInstance().LogEvent(ChatEvent.TransferPerson, agentID);
                }
                catch (Exception ex)
                {
                    ChatLog.GetInstance().LogException(ex);
                }
            }
            return new JsonpResult() { Data = chatContent, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        #endregion


        #region Private Methods

        // 根据当前队列名称获取工程师称谓
        private string GetEmployeeTitle(string queueName)
        {
            List<string> result = new List<string>();
            try
            {
                string appServerDb = ConfigurationManager.AppSettings["appServerDb"];
                Uri uri = new Uri(string.Format("{0}Db/GetChatTexts?queueName={1}&typeName={2}&ss={3}&jsoncallback?"
                    , appServerDb, queueName, HttpUtility.UrlEncode("咨询员称谓"), DateTime.Now.Millisecond));
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    System.IO.StreamReader sr = new System.IO.StreamReader(response.GetResponseStream());
                    string jsonResult = sr.ReadToEnd();
                    result = JsonConvert.DeserializeObject(jsonResult, typeof(List<string>)) as List<string>;
                }
            }
            catch (Exception ex)
            {
                ChatLog.GetInstance().LogException(ex);
            }
            return result.Count > 0 ? result[0] : "工程师";
        }


        #endregion


        public ActionResult GetNow()
        {
            return new JsonpResult() { Data = DateTime.Now.ToString(), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
    }
}
