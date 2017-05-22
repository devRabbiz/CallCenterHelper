using SoftPhone.Entity;
using SoftPhone.Entity.Common;
using SoftPhone.Entity.Model.cfg;
using SoftPhoneService.SupportClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;

namespace SoftPhoneService.Controllers
{
    public class CFGController : BaseController
    {
        //验证用户名密码
        public ActionResult Authenticate(string employeeId, string userPassword)
        {
            var r = new LoginResult();
            var userName = SupportClass.CfgServerHelper.GetPersonUserName(employeeId);
            if (string.IsNullOrEmpty(userName))
            {
                r.EventAuthenticated = false;
                r.ErrorMessage = "没有找到员工编号";
            }
            else
            {
                r = SupportClass.CfgServerHelper.Authenticate(userName, userPassword);
            }
            return Jsonp(r, JsonRequestBehavior.AllowGet);
        }

        //根据员工编号获取坐席信息和坐席能力
        public ActionResult GetPerson(string employee_id)
        {
            var r = new InitInfoVM();
            var person = SupportClass.CfgServerHelper.GetPerson(employee_id);
            if (person == null)
            {
                r.ErrCode = -1;
                r.ErrMessage = "没有找到对应的员工。";
            }
            else
            {
                if (person.CHAT == 0 && person.VOICE == 0)
                {
                    r.ErrCode = -1;
                    r.ErrMessage = "没有语音能力和Chat能力，请联系管理员。";
                }
                r.EnableChat = person.CHAT > 0;
                r.EnableVoice = person.VOICE > 0;
                r.FirstName = person.FirstName;
                if (person.AgentInfo.AgentLogins.Count > 0)
                {
                    r.AgentID = person.AgentInfo.AgentLogins[0].LoginCode;
                }
                else
                {
                    r.ErrCode = -1;
                    r.ErrMessage = "没有找到坐席编号，请联系管理员。";
                }
                if (r.ErrCode == 0)
                {
                    r.Person = person;
                    r.EmployeeID = r.Person.EmployeeID;
                }
            }
            return Jsonp(r, JsonRequestBehavior.AllowGet);
        }

        //缓存休息队列
        private List<string> GetRestQList()
        {
            var key = "sphone/skills/rest";
            if (HttpContext.Cache[key] == null)
            {
                lock (lockSet)
                {
                    if (HttpContext.Cache[key] == null)
                    {
                        var v = SoftPhone.Business.Sphone_QProcessingRest.GetRestQ();
                        HttpContext.Cache.Insert(key, v, null, DateTime.Now.AddMinutes(20), Cache.NoSlidingExpiration);
                    }
                }
            }
            return (List<string>)HttpContext.Cache[key];
        }

        //缓存设置的工作时间队列
        private List<Proc_GetProcessingQList_Result> GetProcessingQList()
        {
            var key = "sphone/skills/processing";
            if (HttpContext.Cache[key] == null)
            {
                lock (lockSet)
                {
                    if (HttpContext.Cache[key] == null)
                    {
                        var v = SoftPhone.Business.Sphone_QProcessingRest.GetProcessingQList();
                        HttpContext.Cache.Insert(key, v, null, DateTime.Now.AddMinutes(20), Cache.NoSlidingExpiration);
                    }
                }
            }
            return (List<Proc_GetProcessingQList_Result>)HttpContext.Cache[key];
        }

        private static object lockSet = new object();

        //获取Voice队列
        public ActionResult GetSkills(string skill)
        {
            var key = "cfg/skills";
            if (HttpContext.Cache[key] == null)
            {
                lock (lockSet)
                {
                    if (HttpContext.Cache[key] == null)
                    {
                        var v = SupportClass.CfgServerHelper.GetSkills();
                        HttpContext.Cache.Insert(key, v, null, DateTime.Now.AddMinutes(30), Cache.NoSlidingExpiration);
                    }
                }
            }
            var skills = (List<Skill>)HttpContext.Cache[key];
            var query = skills.Where(x => x.SwitchName != "Switch_MM");
            if (!string.IsNullOrWhiteSpace(skill))
            {
                query = query.Where(x => x.Name.ToLower().Contains(skill.ToLower()));
            }
            var querySkills = query.ToList();
            var rests = GetRestQList();
            var processings = GetProcessingQList();
            var now = DateTime.Now;
            var nowTimeSpan = new TimeSpan(now.Hour, now.Minute, now.Second);

            var showSkills = new List<Skill>();
            foreach (var skillItem in querySkills)
            {
                if (!rests.Exists(x => x.ToLower() == skillItem.Name.ToLower()))
                {
                    var p = processings.FirstOrDefault(x => x.Q_Name.ToLower() == skillItem.Name.ToLower());
                    if (p != null)
                    {
                        try
                        {
                            var w_begin = p.Q_ProcessingBegin.Split(':');
                            var w_end = p.Q_ProcessingEnd.Split(':');
                            var begin = new TimeSpan(int.Parse(w_begin[0]), int.Parse(w_begin[1]), 0);
                            var end = new TimeSpan(int.Parse(w_end[0]), int.Parse(w_end[1]), 0);
                            if (nowTimeSpan > begin && nowTimeSpan < end)
                            {
                                showSkills.Add(skillItem);
                            }
                        }
                        catch
                        {
                 
                        }
                    }
                    else
                    {
                        showSkills.Add(skillItem);
                    }
                }
            }

            var result = showSkills.OrderBy(x => x.Name).Select(x => new { x.Name }).ToList();
            return Jsonp(result, JsonRequestBehavior.AllowGet);
        }

        //获取Voice队列（正常模式)不过滤
        public ActionResult GetSkillsNormal()
        {
            var key = "cfg/skills/normal";
            if (HttpContext.Cache[key] == null)
            {
                lock (lockSet)
                {
                    if (HttpContext.Cache[key] == null)
                    {
                        var v = SupportClass.CfgServerHelper.GetSkills();
                        HttpContext.Cache.Insert(key, v, null, DateTime.Now.AddMinutes(30), Cache.NoSlidingExpiration);
                    }
                }
            }
            var skills = (List<Skill>)HttpContext.Cache[key];
            var query = skills.OrderBy(x => x.Name);//.Where(x => x.SwitchName != "Switch_MM");
            return Jsonp(query.Select(x => new { x.Name, x.DBID }).ToList(), JsonRequestBehavior.AllowGet);
        }

        //获取Chat队列（正常模式）不过滤
        public ActionResult GetMMSkillsNormal(string skill)
        {
            var key = "cfg/skills/normal";
            if (HttpContext.Cache[key] == null)
            {
                lock (lockSet)
                {
                    if (HttpContext.Cache[key] == null)
                    {
                        var v = SupportClass.CfgServerHelper.GetSkills();
                        HttpContext.Cache.Insert(key, v, null, DateTime.Now.AddMinutes(30), Cache.NoSlidingExpiration);
                    }
                }
            }
            var skills = (List<Skill>)HttpContext.Cache[key];
            var query = skills.OrderBy(x => x.Name).Where(x => x.SwitchName == "Switch_MM");
            if (!string.IsNullOrWhiteSpace(skill))
            {
                query = query.Where(x => x.Name.ToLower().StartsWith(skill.ToLower()));
            }
            return Jsonp(query.Select(x => new { x.Name }).ToList(), JsonRequestBehavior.AllowGet);
        }

        //获取Chat队列
        public ActionResult GetMMSkills(string skill)
        {
            var key = "cfg/skills";
            if (HttpContext.Cache[key] == null)
            {
                lock (lockSet)
                {
                    if (HttpContext.Cache[key] == null)
                    {
                        var v = SupportClass.CfgServerHelper.GetSkills();
                        HttpContext.Cache.Insert(key, v, null, DateTime.Now.AddMinutes(30), Cache.NoSlidingExpiration);
                    }
                }
            }
            var skills = (List<Skill>)HttpContext.Cache[key];
            var query = skills.Where(x => x.SwitchName == "Switch_MM");
            if (!string.IsNullOrWhiteSpace(skill))
            {
                query = query.Where(x => x.Name.ToLower().StartsWith(skill.ToLower()));
            }
            var querySkills = query.ToList();
            var rests = GetRestQList();
            var processings = GetProcessingQList();
            var now = DateTime.Now;
            var nowTimeSpan = new TimeSpan(now.Hour, now.Minute, now.Second);

            var showSkills = new List<Skill>();
            foreach (var skillItem in querySkills)
            {
                if (!rests.Exists(x => x.ToLower() == skillItem.Name.ToLower()))
                {
                    var p = processings.FirstOrDefault(x => x.Q_Name.ToLower() == skillItem.Name.ToLower());
                    if (p != null)
                    {
                        try
                        {
                            var w_begin = p.Q_ProcessingBegin.Split(':');
                            var w_end = p.Q_ProcessingEnd.Split(':');
                            var begin = new TimeSpan(int.Parse(w_begin[0]), int.Parse(w_begin[1]), 0);
                            var end = new TimeSpan(int.Parse(w_end[0]), int.Parse(w_end[1]), 0);
                            if (nowTimeSpan > begin && nowTimeSpan < end)
                            {
                                showSkills.Add(skillItem);
                            }
                        }
                        catch 
                        {

                        }
                    }
                    else
                    {
                        showSkills.Add(skillItem);
                    }
                }
            }

            var result = showSkills.OrderBy(x => x.Name).Select(x => new { x.Name }).ToList();
            return Jsonp(result, JsonRequestBehavior.AllowGet);

            /*
            var key = "cfg/skills";
            if (HttpContext.Cache[key] == null)
            {
                lock (lockSet)
                {
                    if (HttpContext.Cache[key] == null)
                    {
                        var v = SupportClass.CfgServerHelper.GetSkills();
                        HttpContext.Cache.Insert(key, v, null, DateTime.Now.AddMinutes(30), Cache.NoSlidingExpiration);
                    }
                }
            }
            var skills = (List<Skill>)HttpContext.Cache[key];
            var query = skills.OrderBy(x => x.Name).Where(x => x.SwitchName == "Switch_MM");
            if (!string.IsNullOrWhiteSpace(skill))
            {
                query = query.Where(x => x.Name.ToLower().StartsWith(skill.ToLower()));
            }
            return Jsonp(query.Select(x => new { x.Name }).ToList(), JsonRequestBehavior.AllowGet);
             */
        }
    }
}
