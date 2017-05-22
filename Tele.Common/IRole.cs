using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Tele.Common
{
    public interface IRole
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        string RoleID { get; set; }

        /// <summary>
        /// 显示的名字
        /// </summary>
        string RoleName { get; set; }

        /// <summary>
        /// 页面URL列表
        /// </summary>
        List<string> Urls { get; set; }

        /// <summary>
        /// 是否拥有页面的访问权限
        /// </summary>
        bool EnableViewPage(string pageUrl);
    }

    public class SimpleRole : IRole
    {
        #region Properties

        /// <summary>
        /// 角色ID
        /// </summary>
        public string RoleID { get; set; }

        /// <summary>
        /// 显示的名字
        /// </summary>
        public string RoleName { get; set; }

        /// <summary>
        /// 页面URL列表
        /// </summary>
        public List<string> Urls { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// 是否拥有页面的访问权限
        /// </summary>
        public bool EnableViewPage(string pageUrl)
        {
            bool hasPermission = this.Urls.Exists(url =>
            {
                string schemeAndServer = HttpContext.Current.Request.Url.GetComponents(UriComponents.SchemeAndServer, UriFormat.SafeUnescaped);
                if (url.StartsWith("~"))
                    url = url.Replace("~", schemeAndServer);
                return url.StartsWith(pageUrl, StringComparison.CurrentCultureIgnoreCase);
            });
            return hasPermission;
        }

        #endregion
    }

}
