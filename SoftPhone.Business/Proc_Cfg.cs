using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tele.DataLibrary;

namespace SoftPhone.Business
{
    public class Proc_Cfg
    {
        //获取队列
        public static List<string> GetCfgSkillList()
        {
            return GenesysBLL.Proc_GetCfgSkill().Select(x => x.name).ToList();

        }

        //获取管理人员
        public static List<string> GetCfgAdminList()
        {
            return GenesysBLL.Proc_GetCfgAdmin().Select(x => x.employee_id).ToList();

        }
    }
}
