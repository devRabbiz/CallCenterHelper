using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftPhone.Entity.Model.cfg
{
    /// <summary>
    /// 员工
    /// </summary>
    public class Person
    {
        public Person()
        {
            AgentInfo = new AgentInfo();
        }

        public int DBID { get; set; }

        /// <summary>
        /// 员工编号
        /// </summary>
        public string EmployeeID { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// 用户名.大部分是P开头。chat转接用。
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// AgentLogin.LoginCode
        /// </summary>
        public string LoginCode { get; set; }

        /// <summary>
        /// 坐席信息
        /// </summary>
        public AgentInfo AgentInfo { get; set; }

        /// <summary>
        /// 坐席能力
        /// </summary>
        public int CHAT { get; set; }
        /// <summary>
        /// 坐席能力
        /// </summary>
        public int VOICE { get; set; }


        /// <summary>
        /// DN:登陆后根据IP获取DN和PLACE
        /// </summary>
        public string DN { get; set; }
        /// <summary>
        /// Place:登陆后根据IP获取DN和PLACE
        /// </summary>
        public string Place { get; set; }
    }
}
