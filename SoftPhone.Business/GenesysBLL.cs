using SoftPhone.Entity.Genesys.cfg;
using SoftPhone.Entity.Genesys.ers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tele.DataLibrary;

namespace SoftPhone.Business
{
    public class GenesysBLL
    {
        private static string schema = "dbo";
        static GenesysBLL()
        {
            var str1 = System.Configuration.ConfigurationManager.AppSettings["Genesys.schema"];
            if (!string.IsNullOrEmpty(str1))
            {
                schema = str1;
            }
        }

        #region ers
        /// <summary>
        /// 获取对应的队列名
        /// </summary>
        /// <param name="enterID">chat的enterid</param>
        /// <returns></returns>
        public static List<EnterID2Skill_Result> EnterID2Skill(Nullable<int> enterID)
        {
            using (var dc = DCHelper.GenesysErsContext())
            {
                var sql = "SELECT * FROM dbo.IRD_EnterID2Skill WHERE ( {0} IS NULL OR EID = {0} ) AND SName IS NOT NULL";
                var r = dc.Database.SqlQuery<EnterID2Skill_Result>(sql, enterID);
                return r.ToList();
            }
        }

        /// <summary>
        /// 获取队列ID
        /// </summary>
        /// <param name="SName">Q名称</param>
        /// <returns></returns>
        public static EnterID2Skill_Result GetEnterID(string SName)
        {
            using (var dc = DCHelper.GenesysErsContext())
            {
                var sql = "SELECT * FROM dbo.IRD_EnterID2Skill WHERE  SName = {0}";
                var r = dc.Database.SqlQuery<EnterID2Skill_Result>(sql, SName.Replace("'", ""));
                return r.FirstOrDefault();
            }
        }

        //public static List<SoftPhone_ChatInfo> GetChatInfo(string LenovoID)
        //{
        //    using (var dc = DCHelper.SPhoneContext())
        //    {
        //        string start = DateTime.Now.AddMonths(-6).ToShortDateString()+" 00:00:00";
        //        string end = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        //        var sql = string.Format(@"SELECT ChatID,EmployeeID,ChatBeginTime,ChatEndTime
        //                    FROM dbo.sphone_chat WHERE CustomerName='{0}' AND ChatBeginTime>='{1}' And ChatBeginTime<='{2}'", LenovoID, start, end);

        //        var result = dc.Database.SqlQuery<SoftPhone_ChatInfo>(sql).ToList();
        //        if (result != null && result.Count > 0)
        //        {
        //            using (var db = DCHelper.GenesysCfgContext())
        //            {
        //                result.ForEach(p =>
        //                {
        //                    var getNameSql = string.Format(@"select first_name from {0}.cfg_person with(nolock) where is_agent = 2 AND state = 1 AND employee_id='{1}'",
        //                        schema, p.EmployeeID);
        //                    var r2 = db.Database.SqlQuery<string>(getNameSql).FirstOrDefault();
        //                    p.EmployeeName = r2;
        //                });
        //            }
        //        }
        //        return result;
        //    }
        //}
        #endregion

        #region cfg
        /// <summary>
        /// 获取员工编号是否存在，登录用
        /// </summary>
        /// <param name="employee_id">员工编号</param>
        /// <returns></returns>
        public static List<Nullable<int>> Proc_PersonExists(string employee_id)
        {
            using (var dc = DCHelper.GenesysCfgContext())
            {
                var sql = "SELECT 1 FROM " + schema + ".cfg_person WITH ( NOLOCK ) WHERE is_agent = 2 AND state = 1 AND employee_id = {0}";
                var r = dc.Database.SqlQuery<Nullable<int>>(sql, employee_id);
                return r.ToList();
            }
        }

        /// <summary>
        /// 获取cfg所有管理人员
        /// </summary>
        /// <returns></returns>
        public static List<Proc_GetCfgAdmin_Result> Proc_GetCfgAdmin()
        {
            using (var dc = DCHelper.GenesysCfgContext())
            {
                var sql = "SELECT * FROM " + schema + ".cfg_person WHERE is_agent = 1 AND state = 1 AND dbid != 100";
                var r = dc.Database.SqlQuery<Proc_GetCfgAdmin_Result>(sql);
                return r.ToList();
            }
        }

        /// <summary>
        /// 获取cfg所有队列
        /// </summary>
        /// <returns></returns>
        public static List<Proc_GetCfgSkill_Result> Proc_GetCfgSkill()
        {
            using (var dc = DCHelper.GenesysCfgContext())
            {
                var sql = "SELECT * FROM " + schema + ".cfg_skill WHERE state = 1 AND name LIKE 'Q%' ORDER BY name ASC";
                var r = dc.Database.SqlQuery<Proc_GetCfgSkill_Result>(sql);
                return r.ToList();
            }
        }

        /// <summary>
        /// 通过员工DBID获取对应的队列
        /// </summary>
        /// <param name="person_dbid">员工的dbid</param>
        /// <returns></returns>
        public static List<Proc_GetPersonSkills_Result> Proc_GetPersonSkills(Nullable<int> person_dbid)
        {
            using (var dc = DCHelper.GenesysCfgContext())
            {
                var sql = "SELECT CAST( skill_dbid AS INT) 'skill_dbid' , level_ 'level' FROM " + schema + ".cfg_skill_level WITH(NOLOCK) WHERE person_dbid = {0} ";
                var r = dc.Database.SqlQuery<Proc_GetPersonSkills_Result>(sql, person_dbid);
                return r.ToList();
            }
        }

        /// <summary>
        /// 通过员工编号获取坐席信息
        /// </summary>
        /// <param name="employee_id">员工编号</param>
        /// <returns></returns>
        public static List<Proc_GetPersonAgentInfo_Result> Proc_GetPersonAgentInfo(string employee_id)
        {
            using (var dc = DCHelper.GenesysCfgContext())
            {
                var sql = "SELECT CAST(a.dbid AS INT) 'person_dbid' , e.login_code , CAST(e.dbid AS INT) 'login_dbid' , a.employee_id , a.first_name , a.user_name , CAST(ISNULL(c.prop_value, 0) AS INT) 'chat' FROM " + schema + ".cfg_person a WITH ( NOLOCK ) LEFT JOIN " + schema + ".cfg_script b WITH ( NOLOCK ) ON a.capacity_dbid = b.dbid AND b.type = 22 AND b.state = 1 LEFT JOIN " + schema + ".cfg_flex_prop c WITH ( NOLOCK ) ON b.dbid = c.object_dbid AND c.prop_type = 1 AND c.prop_name = 'chat' INNER JOIN " + schema + ".cfg_login_info d WITH ( NOLOCK ) ON d.person_dbid = a.dbid INNER JOIN " + schema + ".cfg_agent_login e WITH ( NOLOCK ) ON e.dbid = d.agent_login_dbid WHERE a.is_agent = 2 AND a.state = 1 AND a.employee_id = {0}";
                var r = dc.Database.SqlQuery<Proc_GetPersonAgentInfo_Result>(sql, employee_id);
                return r.ToList();
            }
        }
        #endregion
    }
}
