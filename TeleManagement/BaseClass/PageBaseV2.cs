using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tele.Common;
using Tele.DataLibrary;

namespace Tele.Management
{
    public class PageBaseV2 : System.Web.UI.Page
    {        // 当前用户
        protected IUser UserInfo = null;
        // 菜单【菜单名，地址】
        protected Dictionary<string, string> MenuDict = null;
        /// <summary>
        /// 是否超级管理员
        /// </summary>
        public bool IsSuperAdmin
        {
            get
            {
                bool result = false;
                if (this.UserInfo != null)
                    result = this.UserInfo.IsInRole("SUPER_ADMIN", "超级管理员");
                return result;
            }
        }
        // 检查页面访问权限
        protected bool HasPermission
        {
            get
            {
                bool enableViewPage = false;
                enableViewPage = this.IsSuperAdmin;
                if (!enableViewPage)
                {
                    string currentPageUrl = Request.Url.GetComponents(UriComponents.SchemeAndServer | UriComponents.Path, UriFormat.SafeUnescaped);
                    if (this.UserInfo != null)
                        enableViewPage = this.UserInfo.HasPagePermission(currentPageUrl);
                }
                return enableViewPage;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            // 用户信息
            this.UserInfo = AuthSession.GetCurrentUserInfo();
            // 用户权限
            if (!this.HasPermission)
                throw new Exception("拒绝访问，您没有页面的访问权限。");
        }

        private SPhoneEntities _dc;

        protected SPhoneEntities dc
        {
            get
            {
                if (_dc == null)
                {
                    _dc = DCHelper.SPhoneContext();
                }
                return _dc;
            }
        }
    }
}