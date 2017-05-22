using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tele.DataLibrary;

namespace SoftPhone.Business
{
    public class EnterID2SkillBLL
    {
        public static string GetSkillName(int enterID)
        {
            var info = GenesysBLL.EnterID2Skill(enterID).FirstOrDefault();
            if (info == null)
            {
                throw new Exception("没有找到对应的队列");
            }
            return info.SName;
        }

        public static string GetQueueMsg(int enterID)
        {
            var info = GenesysBLL.EnterID2Skill(enterID).FirstOrDefault();
            if (info == null)
            {
                throw new Exception("没有找到对应的队列");
            }
            return info.QueueMsg0;
        }
    }
}
