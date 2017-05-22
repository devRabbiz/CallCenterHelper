
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
    public partial class RoleList : PageBase<SPhone_Role>
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            BindRepeater();
        }

        protected override void CollectInfo(Dictionary<string, object> context)
        {
            context["RoleName"] = this.txtRoleName.Text.Trim();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindRepeater();
        }
        protected void pager1_OnPageChanged(object sender, EventArgs e)
        {
            BindRepeater();
        }
        protected void rptList_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            Guid id = Guid.Empty;
            Guid.TryParse((string)e.CommandArgument, out id);
            if (e.CommandName == "Status")
            {
                using (var db = DCHelper.SPhoneContext())
                {
                    SPhone_Role dn = db.SPhone_Role.Where(item => item.RoleID == id).FirstOrDefault();
                    if (dn != null)
                    {
                        List<SPhone_RoleModule> rms = dn.SPhone_RoleModule.Where(item => item.RoleID == id).ToList();
                        rms.ForEach(rm => dn.SPhone_RoleModule.Remove(rm));

                        db.SPhone_Role.Remove(dn);
                        db.SaveChanges();

                        BindRepeater();
                    }
                }
            }
        }
        #region BindRepeater

        protected void BindRepeater()
        {
            var searcher = GetSearcher();
            using (var db = DCHelper.SPhoneContext())
            {
                var query = db.SPhone_Role.Where(item => 1 == 1);
                if (!string.IsNullOrEmpty(searcher.RoleName))
                    query = query.Where(item => item.RoleName.IndexOf(searcher.RoleName) != -1);

                this.pager1.RecordCount = query.Count();
                this.rptList.DataSource = query.OrderByDescending(item => item.CreateTime)
                    .Skip((this.pager1.CurrentPageIndex - 1) * this.pager1.PageSize).Take(this.pager1.PageSize).ToList();
                this.rptList.DataBind();
            }
        }


        #endregion

    }
}