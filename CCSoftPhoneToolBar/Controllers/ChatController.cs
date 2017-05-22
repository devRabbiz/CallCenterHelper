using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using SoftPhoneToolBar.Models;
using Tele.Common;

namespace SoftPhoneToolBar.Controllers
{
    public class ChatController : Controller
    {

        #region 用户界面


        /// <summary>
        /// 用户请求进入聊天主界面
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 聊天窗口
        /// </summary>
        /// <param name="ticketID"></param>
        /// <param name="interactionID"></param>
        /// <param name="customerData"></param>
        /// <returns></returns>
        public ActionResult ChatTab(string ticketID, string interactionID, string chatData)
        {
            if (string.IsNullOrEmpty(ticketID))
                ticketID = "300";

            if (string.IsNullOrEmpty(interactionID))
                interactionID = "00005a8K09C800J4";

            if (string.IsNullOrEmpty(chatData))
                chatData = string.Empty;
            CurrentDataReference data = JsonConvert.DeserializeObject(chatData
                                       , typeof(CurrentDataReference)) as CurrentDataReference;

            // 呈现
            Models.ChatTabView model = new Models.ChatTabView();
            model.TicketID = ticketID;
            model.CurrentSessionID = interactionID;
            model.ChatData = chatData;

            //Response.Write(chatData);

            if (data != null)
            {
                model.CustomerID = data.CustomerID;
                model.CustomerName = HttpUtility.UrlDecode(data.CustomerName);
                model.MachineNO = data.MachineNO;
                model.CurrentQueueName = data.CurrentQueue;
                model.ChatServerHost = data.ChatServerHost;
                model.ChatServerPort = data.ChatServerPort;
                model.CustomerIP = data.CustomerIP;
                /*
                SoftPhoneToolBar.SupportClass.AddressForQueryIPFromBaidu oBaiDu = SoftPhoneToolBar.SupportClass.BaiDuAPI.GetAddressFromIP("211.154.135.123");

                string customerIP = "未知";

                if (oBaiDu != null)
                {
                    if (oBaiDu.Content != null)
                    {
                        if (oBaiDu.Content.Address_Detail != null)
                        {
                            customerIP = oBaiDu.Content.Address_Detail.City;
                        }
                    }
                }
                model.CustomerIP = customerIP;
               
                 */
            }

            return View(model);
        }


        /// <summary>
        /// 转出
        /// </summary>
        public ActionResult Transfer(string queueID, string interactionID)
        {
            return View();
        }

        /// <summary>
        /// 会议邀请
        /// </summary>
        public ActionResult Meeting()
        {
            return View();
        }

        /// <summary>
        /// 聊天历史记录
        /// </summary>
        /// <returns></returns>
        public ActionResult ChatHistory(string employeeID)
        {
            HistoryModel model = new HistoryModel();
            model.EmployeeID = employeeID;
            return View(model);
        }

        /// <summary>
        /// 远程协助
        /// </summary>
        /// <param name="ticketID"></param>
        /// <param name="interactionID"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public ActionResult ChatRTO(string ticketID, string interactionID, string accessCode)
        {
            ChatRTOView model = new ChatRTOView();
            model.TicketID = ticketID;
            model.SessionID = interactionID;
            model.AccessCode = accessCode;
            return View(model);
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
                        model = ChatHelper.UploadFile(file, UNCConfig.GetCfg());
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
                throw ex;
            }
        }

        /// <summary>
        /// 文件上传，增强.支持跨域
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult FileUpload2()
        {
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
                        model = ChatHelper.UploadFile(file, UNCConfig.GetCfg());
                    }
                }
                else
                {
                    model.ReturnFlag = false;
                    model.Message = "找不到可以上传的文件！";
                }
                string jsonData = JsonConvert.SerializeObject(model);
                ViewData["r"] = jsonData;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return PartialView();
        }
        #endregion
    }
}
