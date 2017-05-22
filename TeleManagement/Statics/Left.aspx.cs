using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Tele.Common;
using SoftPhone.Entity;
using Tele.DataLibrary;
using System.Text;

namespace Tele.Management.Statics
{
    public partial class Left : System.Web.UI.Page
    {
        #region Properties

        private List<SPhone_Module> modules = null;
        protected List<SPhone_Module> Modules
        {
            get
            {
                if (modules == null)
                {
                    using (var db = DCHelper.SPhoneContext())
                    {
                        modules = db.SPhone_Module.ToList();
                    }
                }
                return modules;
            }
        }

        #endregion


        protected void Page_Load(object sender, EventArgs e)
        {
            // 用户信息
            SimpleUser user = AuthSession.GetCurrentUserInfo();
            List<IRole> roles = user.Roles;
            this.divMenu.InnerHtml = BuildLinkItem(roles);
        }



        private string BuildLinkItem(List<IRole> roles)
        {
            StringBuilder sb = new StringBuilder();
            List<string> urls = new List<string>();
            roles.ForEach(item => { urls.AddRange(item.Urls); });
            string moduleID = "mod1";
            List<string> links = MenuSettings.GetInstance().GetMatchLinks(moduleID, urls, ShowType.GroupName);
            sb.AppendLine("<ul>");
            var current = 0;
            links.ForEach(l =>
            {

                if (l.StartsWith("<b"))
                {
                    sb.Append("<li class='nav" + (++current) + "'>");
                }
                else
                {
                    sb.Append("<li class='act" + (current) + "'>");
                }
                sb.Append(l);
                sb.AppendLine("</li>");
            });
            sb.AppendLine("</ul>");
            return sb.ToString();
        }



    }
}