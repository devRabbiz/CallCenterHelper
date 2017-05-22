using SoftPhone.Entity;
using SoftPhone.Entity.Common;
using SoftPhone.Entity.Model.cfg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tele.Common.Helper;

namespace SoftPhoneService.Controllers
{
    public class PhoneStatController : BaseController
    {
        //
        // GET: /PhoneState/

        public ActionResult Index()
        {
            return View();
        }

        #region 坐席登录订阅 OpenAgentStatistic
        /// <summary>
        /// 坐席登录订阅
        /// </summary>
        /// <param name="Person">SoftPhone.Entity.Model.cfg.Person</param>
        /// <returns></returns>
        public ActionResult OpenAgentStatistic(Person person)
        {
            var p = Request.QueryString["person"];
            person = Tele.Common.Helper.JsonHelper.Deserialize<Person>(p);
            var r = new AjaxReturn();
            try
            {
                var query = SupportClass.StatServerHelper.ALLStatisticItems.Values.Where(x =>
                    x.Opened == false &&
                    x.TypeID == 2 &&
                    x.RequireOpen == false &&
                    x.DBID == person.DBID
                    );
                foreach (var item in query)
                {
                    item.LastDate = DateTime.Now;
                    item.RequireOpen = true;
                }

                var cachePersonShort = SupportClass.StatServerHelper.ALLPersons.FirstOrDefault(x => x.DBID == person.DBID);
                if (cachePersonShort != null)//zhangsl:2013.05.03 update 人可能会换座位,每次都要重新赋值
                {
                    cachePersonShort.AgentInfo.IsInitAgentInfo = true;
                    cachePersonShort.DN = person.DN;
                    cachePersonShort.Place = person.Place;
                    cachePersonShort.VOICE = person.VOICE;
                    cachePersonShort.CHAT = person.CHAT;
                    cachePersonShort.AgentInfo = (AgentInfo)person.AgentInfo.Clone();
                }
            }
            catch (Exception err)
            {
                r.SetError("发生错误:" + err.Message);
            }

            return Jsonp(r, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 3秒获取一次新数据 GetAgentStatisticResult
        /// <summary>
        /// 3秒获取一次新数据
        /// </summary>
        /// <param name="Person"></param>
        /// <returns></returns>
        public ActionResult GetAgentStatisticResult(int personDBID)
        {
            var p = Request.QueryString["person"];
            var person = SupportClass.StatServerHelper.ALLPersons.FirstOrDefault(x => x.DBID == personDBID);
            var r = new AjaxReturn();
            var d = new AgentStatisticResult();
            try
            {
                d = SupportClass.StatServerHelper.GetAgentStatisticResult(person);
            }
            catch
            {

            }
            r.d = d;
            return Jsonp(r, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 根据队列DBID集合获取集合对应的排队量
        /// </summary>
        /// <param name="skillDBIDList"></param>
        /// <returns></returns>
        public ActionResult GetQueneCount(string skillDBIDList)
        {
            var listIDS = new List<int>();
            var ids = skillDBIDList.Split(',');
            foreach (var item in ids)
            {
                listIDS.Add(int.Parse(item));
            }

            var skills = (from a in SupportClass.StatServerHelper.ALLSkills
                          join b in listIDS on a.DBID equals b
                          select new { DBID = a.DBID, Name = a.Name, IsMM = a.SwitchName == "Switch_MM" }).ToList();

            var source = SupportClass.StatServerHelper.ALLStatisticItems.Values.Where(x => x.TypeID == 1).ToList();
            var query = from a in source
                        join b in skills on a.DBID equals b.DBID
                        orderby b.Name ascending
                        select new { b.Name, Count = a.LastValue, b.IsMM };
            return Jsonp(query.ToList(), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 根据队列获取坐席 GetAgents
        /// <summary>
        /// 根据队列获取坐席,有DN才显示
        /// </summary>
        /// <param name="skillName"></param>
        /// <param name="agentName"></param>
        /// <param name="DN"></param>
        /// <returns></returns>
        public ActionResult GetAgents(string skillName, string agentName, string DN)
        {
            var agents = SupportClass.StatServerHelper.GetTransferAgent(skillName);
            var query = agents.Where(x => !string.IsNullOrEmpty(x.DN)).Where(x => string.IsNullOrEmpty(agentName) || x.AgentName.ToLower().Contains(agentName))
                .Where(x => string.IsNullOrEmpty(DN) || x.DN.Contains(DN));
            return Jsonp(query.ToList(), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 注销时取消登录的DN
        public ActionResult AgentLogout(int personDBID)
        {
            var r = false;
            var person = SupportClass.StatServerHelper.ALLPersons.FirstOrDefault(x => x.DBID == personDBID);
            if (person != null)
            {
                person.DN = "";
                person.Place = "";
                r = true;
            }
            return Jsonp(r, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AgentLogout_Voice(int personDBID)
        {
            var r = false;
            var person = SupportClass.StatServerHelper.ALLPersons.FirstOrDefault(x => x.DBID == personDBID);
            if (person != null)
            {
                person.DN = "";
                r = true;
            }
            return Jsonp(r, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}
