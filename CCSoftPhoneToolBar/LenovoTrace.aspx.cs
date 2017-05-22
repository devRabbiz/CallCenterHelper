using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SoftPhoneToolBar
{
    public partial class LenovoTrace : System.Web.UI.Page
    {
        private static log4net.ILog log = log4net.LogManager.GetLogger(typeof(LenovoTrace));

        protected void Page_Load(object sender, EventArgs e)
        {
            var act = Request.Form["act"] ?? "";

            if (act == "trace")
            {
                var msg = Request.Form["msg"] ?? "";
                log.Info(msg);
            }
        }
    }
}