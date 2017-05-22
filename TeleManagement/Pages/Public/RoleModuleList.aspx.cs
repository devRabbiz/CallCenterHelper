
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Tele.DataLibrary;
using SoftPhone.Entity;
using System.Linq.Expressions;

namespace Tele.Management.Pages
{
    public partial class RoleModuleList : PageBase<SPhone_RoleModule>
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                BindRepeater();
            }
        }

        protected override void CollectInfo(Dictionary<string, object> context)
        {
            context["CreateBy"] = this.txtCreateBy.Text.Trim();
            context["CreateTime"] = this.txtCreateTime.Text.Trim();
            context["UpdateBy"] = this.txtUpdateBy.Text.Trim();
            context["UpdateTime"] = this.txtUpdateTime.Text.Trim();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindRepeater();
        }
        protected void pager1_OnPageChanged(object sender, EventArgs e)
        {
            BindRepeater();
        }

        #region BindRepeater

        protected void BindRepeater()
        {
            List<SPhone_RoleModule> lst = null;
            using (var db = DCHelper.SPhoneContext())
            {
                Predicate<SPhone_RoleModule> match = this.InitSearchData();
                Func<SPhone_RoleModule, bool> contidion = new Func<SPhone_RoleModule, bool>(item => match(item));
                lst = db.SPhone_RoleModule.Where(contidion).ToList();
            }
            this.pager1.RecordCount = lst.Count;
            this.rptList.DataSource = lst
                .Skip((this.pager1.CurrentPageIndex - 1) * this.pager1.PageSize).Take(this.pager1.PageSize);
            this.rptList.DataBind();
        }


        #endregion

    }
}