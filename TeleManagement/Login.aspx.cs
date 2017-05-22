using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Tele.Management
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.divMessage.Visible = false;
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string logonID = this.txtLogonID.Text.Trim().Replace("'", "");
            string pwd = this.txtPassword.Text;

            bool isLogin = AuthSession.IsAuthenticated(logonID, pwd);
            if (isLogin)
                Response.Redirect("Default.aspx");
            else
                this.divMessage.Visible = true;
        }


    }
}