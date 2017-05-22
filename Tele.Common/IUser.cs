using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tele.Common
{
    public interface IUser
    {

        /// <summary>
        /// 用户的登录名
        /// </summary>
        string LogonID { get; set; }

        /// <summary>
        /// 显示的名字
        /// </summary>
        string DisplayName { get; set; }

        /// <summary>
        /// 是否拥有角色
        /// </summary>
        bool IsInRole(params string[] roleName);

        /// <summary>
        /// 是否有当前页面的访问权限
        /// </summary>
        bool HasPagePermission(string pageUrl);

    }


    public class SimpleUser : IUser
    {
        #region Properties

        /// <summary>
        /// 用户的登录名
        /// </summary>
        public string LogonID { get; set; }

        /// <summary>
        /// 显示的名字
        /// </summary>
        public string DisplayName { get; set; }


        /// <summary>
        /// 所在的AD域
        /// </summary>
        public string Domain { get; set; }

        /// <summary>
        /// 用户角色列表
        /// </summary>
        public List<IRole> Roles { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// 是否拥有角色
        /// 如果传入多个角色，则只要有一个角色满足，即返回True
        /// </summary>
        public bool IsInRole(params string[] roleName)
        {
            List<string> names = new List<string>();
            names.AddRange(roleName);
            bool hasRole = (names.Count == 0);
            foreach (string role in names)
            {
                hasRole = this.Roles.Exists(r => r.RoleName.Equals(role, StringComparison.CurrentCultureIgnoreCase));
                if (hasRole) break;
            };
            return hasRole;
        }

        /// <summary>
        /// 是否有当前页面的访问权限
        /// </summary>
        public bool HasPagePermission(string pageUrl)
        {
            bool enableViewPage = false;
            if (this.Roles != null)
                enableViewPage = this.Roles.Exists(item => item.EnableViewPage(pageUrl));
            return enableViewPage;
        }
        #endregion
    }

}
