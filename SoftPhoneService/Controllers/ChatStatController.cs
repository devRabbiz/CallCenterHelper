using SoftPhone.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tele.Common.Helper;
using SoftPhone.Entity;
using SoftPhone.Entity.Model.cfg;
using SoftPhone.Business;
using Newtonsoft.Json;
using Chat.Common;

namespace SoftPhoneService.Controllers
{
    public class ChatStatController : BaseController
    {
        //
        // GET: /ChatState/

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 根据EnterID获取队列名
        /// </summary>
        /// <param name="enterID"></param>
        /// <returns></returns>
        public ActionResult GetQueueName(string enterID)
        {
            int eid = 9999;
            int.TryParse(enterID, out eid);
            string skillName = SoftPhone.Business.EnterID2SkillBLL.GetSkillName(eid);
            return Jsonp(skillName, JsonRequestBehavior.AllowGet);
        }

        #region 查询排队量/队列

        /// <summary>
        /// 通过队列名查找队列排队量
        /// </summary>
        /// <param name="skillName">Q231_XXXXX</param>
        /// <returns></returns>
        public ActionResult GetQueneCountBySkillName(string skillName)
        {
            var d = 0;
            try
            {
                d = SupportClass.StatServerHelper.GetQueneCountBySkillName(skillName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Jsonp(d, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 通过队列名查询排队量
        /// </summary>
        /// <param name="enterID"></param>
        /// <returns></returns>
        public ActionResult GetQueneCountByEnterID(string enterID)
        {
            var ajaxReturn = new SoftPhone.Entity.Common.AjaxReturn();
            ajaxReturn.d = 0;
            try
            {
                int eid = int.Parse(enterID);
                var skillName = SoftPhone.Business.EnterID2SkillBLL.GetSkillName(eid);
                ajaxReturn.d = SupportClass.StatServerHelper.GetQueneCountBySkillName(skillName);
                ajaxReturn.Message = SoftPhone.Business.EnterID2SkillBLL.GetQueueMsg(eid);
            }
            catch (Exception ex)
            {
                //throw ex;
                // edit by jeff
                ajaxReturn.Code = -1;
                ajaxReturn.Message = ex.Message;
            }
            return Jsonp(ajaxReturn, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 查询客服坐席

        //根据队列获取坐席(chat),当前chat数
        public ActionResult GetAgents(string skillName, string agentName, string agentID, string placeID)
        {
            var agents = SupportClass.StatServerHelper.GetTransferChatAgent(skillName);
            var query = agents.Where(x => !string.IsNullOrEmpty(x.PlaceId)).Where(x => string.IsNullOrEmpty(agentName) || x.AgentName.ToLower().Contains(agentName.ToLower()))
                .Where(x => string.IsNullOrEmpty(agentID) || x.AgentId.ToLower().Contains(agentID.ToLower()))
                .Where(x => string.IsNullOrEmpty(placeID) || x.PlaceId.ToLower().Contains(placeID.ToLower()));
            return Jsonp(query.ToList(), JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region  CSDP_应用API

        //批量获取工程忙闲状态，以及排队人数
        public ActionResult CSDP_ChatAgent_GetStatus(string EmployeeIDs)
        {
            var arr = EmployeeIDs.Split(',').ToList();
            var query = SupportClass.StatServerHelper.CSDP_ChatAgent_GetStatus(arr);
            var result = query.Select(x => new { x.AgentId, x.Place_In_Quene, x.Status }).ToList();
            return Jsonp(result, JsonRequestBehavior.AllowGet);
        }

        //批量获取当前忙闲状态为空闲的工程师
        public ActionResult CSDP_ChatAgent_GetAllReady()
        {
            var query = SupportClass.StatServerHelper.CSDP_ChatAgent_GetAllReady();
            var result = query.Select(x => new { x.AgentId }).ToList();
            return Jsonp(result, JsonRequestBehavior.AllowGet);
        }

        //获取坐席的队列
        public ActionResult CSDP_ChatAgent_GetEnterID(string EmployeeID)
        {
            var SName = "Q" + EmployeeID + "_CSDP";
            var query = SoftPhone.Business.GenesysBLL.GetEnterID(SName);
            var r = 0;
            if (query != null)
            {
                r = query.EID.Value;
            }
            return Jsonp(r, JsonRequestBehavior.AllowGet);
        }

        //public ActionResult CSDP_ChatAgent_GetChatInfo(string LenovoID)
        //{
        //    var query = SoftPhone.Business.GenesysBLL.GetChatInfo(LenovoID);
        //    return Jsonp(query, JsonRequestBehavior.AllowGet);
        //}

        #endregion
    }
}
