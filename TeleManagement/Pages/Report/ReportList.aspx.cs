using SoftPhone.Business;
using SoftPhone.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Tele.Management.Pages.Report
{
    public partial class ReportList : PageBase<SPhone_ReportUrl>
    {
        protected List<SPhone_ReportUrl> list = new List<SPhone_ReportUrl>();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                list = SPhone_ReportUrlBLL.GetQuery().Where(x => x.IsShow == 1).ToList();
            }
        }
    }
}