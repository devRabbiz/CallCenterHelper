using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;
using SoftPhone.Entity.Model.cfg;
using Tele.Common;
using Tele.Common.Helper;
using SoftPhone.Entity.Common;
using System.Web.Security;
using SoftPhone.Entity;
using Tele.DataLibrary;

namespace Tele.Management
{
    public class AuthSession
    {
        const string AuthSessionKey = "TELE_SPHONE_AUTH_SESSIONKEY";

        /// <summary>
        /// 检查是否需要登录
        /// </summary>
        /// <returns></returns>
        public static bool NeedLogin()
        {
            string loginMode = ConfigurationManager.AppSettings["IsUrgentLogin"];
            return !Utils.ToBooleanValue(loginMode);
        }

        /// <summary>
        /// 验证用户
        /// </summary>
        /// <param name="logonID">ITCode</param>
        /// <param name="password">密码</param>
        public static bool IsAuthenticated(string logonID, string password)
        {
            LoginResult result = null;
            List<IRole> roles = GetUserPermissions(logonID);
            if (roles != null && roles.Count > 0)
            {
                string pathAndQuery = string.Format("CFG/Authenticate?employeeId={0}&userPassword={1}"
                    , logonID, password);
                result = GetRemoteData<LoginResult>(pathAndQuery);
                if (result != null && result.EventAuthenticated)
                {
                    SimpleUser user = new SimpleUser();
                    user.LogonID = logonID;
                    user.DisplayName = logonID;
                    // 获取角色
                    user.Roles = roles;
                    HttpContext.Current.Session[AuthSessionKey] = user;
                }
            }
            bool isLogin = (result != null) ? result.EventAuthenticated : false;
            return isLogin;
        }

        public static SimpleUser GetCurrentUserInfo()
        {
            SimpleUser user = HttpContext.Current.Session[AuthSessionKey] as SimpleUser;
            // 跳转到登陆页
            if (user == null)
            {
                bool needLogin = NeedLogin();
                // 模拟管理员
                if (!needLogin) user = InitSuperAdmin();
                else
                    HttpContext.Current.Response.Redirect(FormsAuthentication.LoginUrl);
            }
            return user;
        }

        /// <summary>
        /// 退出登录
        /// </summary>
        public static void Logout()
        {
            HttpContext.Current.Session[AuthSessionKey] = null;
            HttpContext.Current.Response.Redirect(FormsAuthentication.LoginUrl);
        }


        #region Private Methods

        private static T GetRemoteData<T>(string pathAndQuery)
            where T : class
        {
            T result = null;
            string appServer = ConfigurationManager.AppSettings["appServer"];
            Uri uri = new Uri(string.Format("{0}{1}&ss={2}&jsoncallback?"
                , appServer, pathAndQuery, DateTime.Now.Millisecond));
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                System.IO.StreamReader sr = new System.IO.StreamReader(response.GetResponseStream());
                string jsonResult = sr.ReadToEnd();
                result = JsonHelper.Deserialize<T>(jsonResult);
            }
            return result;
        }

        private static SimpleUser InitSuperAdmin()
        {
            List<IRole> roles = new List<IRole>();
            SimpleRole role = new SimpleRole();
            role.RoleID = "SUPER_ADMIN";
            role.RoleName = "超级管理员";
            role.Urls = GetModulesInRole(new Guid("69A61B69-B57F-480A-BCB2-5B71E5BF954A"));
            roles.Add(role);

            SimpleUser user = new SimpleUser();
            user.LogonID = "SUPER_ADMIN";
            user.DisplayName = "超级管理员";
            user.Roles = roles;
            return user;
        }

        // 获取用户的角色集合
        private static List<IRole> GetUserPermissions(string employeeID)
        {
            List<IRole> roles = new List<IRole>();
            List<SPhone_Role> lst = null;
            using (var db = DCHelper.SPhoneContext())
            {
                List<Guid> rids = db.SPhone_UserPermission.Where(rm => rm.EmployeeID == employeeID).Select(rm => rm.RoleID).ToList();
                lst = db.SPhone_Role.ToList();
                rids.ForEach(id =>
                {
                    SPhone_Role role = lst.Find(m => m.RoleID == id);
                    if (role != null)
                    {
                        SimpleRole model = new SimpleRole();
                        model.RoleID = role.RoleID.ToString();
                        model.RoleName = role.RoleName;
                        model.Urls = GetModulesInRole(role.RoleID);
                        roles.Add(model);
                    }
                });
            }
            return roles;
        }

        // 根据角色ID获取链接
        private static List<string> GetModulesInRole(Guid roleID)
        {
            List<string> urls = new List<string>();
            List<SPhone_Module> lst = null;
            using (var db = DCHelper.SPhoneContext())
            {
                List<Guid> mids = db.SPhone_RoleModule.Where(rm => rm.RoleID == roleID).Select(rm => rm.ModuleID).ToList();
                lst = db.SPhone_Module.ToList();
                mids.ForEach(id =>
                {
                    SPhone_Module module = lst.Find(m => m.ModuleID == id);
                    if (module != null)
                        urls.Add(module.ModuleUrl);
                });
            }
            return urls;
        }

        #endregion

    }
}