using SoftPhone.Entity;
using SoftPhone.Entity.Genesys.cfg;
using SoftPhone.Entity.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftPhone.Business
{
    public class ProcBLL
    {
        #region 员工编号是否存在
        public static int Proc_PersonExists(string employee_id)
        {
            //using (var db = Tele.DataLibrary.DCHelper.SPhoneContext())
            //{
            //    var r = db.Proc_PersonExists(employee_id).FirstOrDefault();
            //    if (r != null)
            //    {
            //        return r.Value;
            //    }
            //    return 0;
            //}

            var r = GenesysBLL.Proc_PersonExists(employee_id).FirstOrDefault();
            if (r != null)
            {
                return r.Value;
            }
            return 0;
        }
        #endregion

        #region 获取坐席信息
        public static Proc_GetPersonAgentInfo_Result Proc_GetPersonAgentInfo(string employee_id)
        {
            return GenesysBLL.Proc_GetPersonAgentInfo(employee_id).FirstOrDefault();
        }
        #endregion

        public static List<SoftPhone.Entity.Genesys.cfg.Proc_GetPersonSkills_Result> Proc_GetPersonSkills(int person_dbid)
        {
            var r = GenesysBLL.Proc_GetPersonSkills(person_dbid);
            return r.ToList();

        }

        public static int Proc_AgentStatusChangeLog(string logID, string employeeID, Nullable<int> typeID, Nullable<int> insertOrUpdate)
        {
            using (var db = Tele.DataLibrary.DCHelper.SPhoneContext())
            {
                return db.Proc_AgentStatusChangeLog(logID, employeeID, typeID, insertOrUpdate);
            }
        }

        public static List<SoftPhone.Entity.Genesys.cfg.Proc_GetCfgAdmin_Result> Proc_GetCfgAdmin()
        {
            return GenesysBLL.Proc_GetCfgAdmin().ToList();

        }

        #region 获取enterid
        public static Proc_GetChatRightEnterID_Result Proc_GetChatRightEnterID(int enterID, string machineNo)
        {
            var result = new Proc_GetChatRightEnterID_Result() { Return = enterID.ToString(), Type = 1 };
            using (var db = Tele.DataLibrary.DCHelper.SPhoneContext())
            {
                var qs = db.Proc_GetChatRightEnterID(enterID, machineNo).ToList();
                if (qs.Count > 0)
                {
                    var q = qs.FirstOrDefault();
                    if (q != null)
                    {
                        result = q;
                    }
                }
            }
            return result;
        }
        #endregion
    }
}
