using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Tele.Management
{
    public partial class LoginAuth : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var employeeId = Request.QueryString["employeeId"] ?? "";
            var userPassword = Request.QueryString["userPassword"] ?? "";
            var callback = Request.QueryString["callback"] ?? "";
            var isLogin = false;
            try
            {
                isLogin = AuthSession.IsAuthenticated(employeeId, userPassword);
            }
            catch
            {

            }
            if (isLogin)
            {
                Response.Clear();
                Response.Write(string.Format("{0}({{islogin:1}})", callback));
                Response.End();
            }
            else
            {
                Response.Clear();
                Response.Write(string.Format("{0}({{islogin:0}})",callback));
                Response.End();
            }
        }
    }
}