using OCSService.SupportClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace OCSService.Test
{
    public partial class Test : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            OCSHelperInterface.GetHelper().ReloadCampaginlist();
            gridView1.DataSource = OCSHelperInterface.GetHelper().ALLCampaign;
            gridView1.DataBind();
        }
    }
}