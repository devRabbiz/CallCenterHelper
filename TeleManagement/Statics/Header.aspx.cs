using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Tele.Common;

namespace Tele.Management.Statics
{
    public partial class Header : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            // 用户信息
            SimpleUser user = AuthSession.GetCurrentUserInfo();
            this.ltlUser.Text = user.DisplayName;
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            AuthSession.Logout();
        }

    }
}