using SoftPhone.Business;
using SoftPhone.Entity;
using SoftPhone.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftPhoneService.Controllers
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
            , long? customerID, string customerName, string machineNo, string mailAddress
            , string agentID, long? beginDate, int isTransfer, int isMeeting)
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

            entity.CustomerID = Convert.ToInt64(customerID);
            entity.CustomerName = customerName;
            entity.Enterid = enterID;
            entity.MachineNo = machineNo;
            entity.MailAddress = mailAddress;
            //entity.ServicecardNo = "";
            //entity.WSISID = "";

            entity.EmployeeID = agentID;
            entity.PlaceIP = this.Request.UserHostAddress;
            entity.FromQueueName = fromQueue;
            entity.CurrentQueueName = currentQueue;
            //entity.NextQueueName = "";
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
        public ActionResult ChatUpdate(string chatID, string nextQueue, int isMeeting, int isRTO, string jsonMessageData)
        {
            var result = new AjaxReturn();
            try
            {
                if (string.IsNullOrEmpty(chatID))
                    throw new Exception("传入的参数 chatID 不可为空值！");
                SPhone_Chat entity = SPhone_ChatBLL.GetChat(chatID);
                entity.NextQueueName = nextQueue;
                entity.IsConference = isMeeting;
                entity.IsRTO = isRTO;
                entity.ContentText = jsonMessageData;
                entity.UpdateBy = entity.EmployeeID;
                entity.ChatEndTime = DateTime.Now;
                entity.UpdateTime = entity.ChatEndTime;
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
    }
}
