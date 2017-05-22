using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftPhone.Entity.Model.cfg
{
    public class AgentInfo : ICloneable
    {
        public AgentInfo()
        {
            SkillLevels = new List<SkillLevel>();
            AgentLogins = new List<AgentLogin>();
        }

        /// <summary>
        /// 坐席所属队列
        /// </summary>
        public List<SkillLevel> SkillLevels { get; set; }

        /// <summary>
        /// 坐席登录帐号 AgentID
        /// </summary>
        public List<AgentLogin> AgentLogins { get; set; }

        public object Clone()
        {
            var info = (AgentInfo)this.MemberwiseClone();
            return info;
        }

        /// <summary>
        /// 是否初始化坐席信息.获取用户简短信息时不包含坐席信息
        /// </summary>
        public bool IsInitAgentInfo { get; set; }
    }
}
