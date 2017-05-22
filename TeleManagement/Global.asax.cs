using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace Tele.Management
{
    public class Global : System.Web.HttpApplication
    {
        public static string PhysicalApplicationPath = string.Empty;

        protected void Application_Start(object sender, EventArgs e)
        {
            PhysicalApplicationPath = AppDomain.CurrentDomain.BaseDirectory;
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}