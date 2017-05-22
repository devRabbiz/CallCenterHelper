
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
    public partial class DutyManagerList : PageBase<SPhone_DutyManager>
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            BindRepeater();
        }

        protected override void CollectInfo(Dictionary<string, object> context)
        {
            context["ManagerName"] = this.txtManagerName.Text.Trim();
            context["PhoneNo"] = this.txtPhoneNo.Text.Trim();
            context["DutyDate"] = this.txtBeginDate.Text.Trim();
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
                    SPhone_DutyManager dn = db.SPhone_DutyManager.Where(item => item.DutyManagerID == id).FirstOrDefault();
                    if (dn != null)
                    {
                        db.SPhone_DutyManager.Remove(dn);
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
                var query = db.SPhone_DutyManager.Where(item => 1 == 1);
                if (!string.IsNullOrEmpty(searcher.ManagerName))
                    query = query.Where(item => item.ManagerName.IndexOf(searcher.ManagerName) != -1);
                if (!string.IsNullOrEmpty(searcher.PhoneNo))
                    query = query.Where(item => item.PhoneNo.IndexOf(searcher.PhoneNo) != -1);
                if (searcher.DutyDate != default(DateTime))
                    query = query.Where(item => item.DutyDate.Value.Date == searcher.DutyDate.Value.Date);
                this.pager1.RecordCount = query.Count();
                this.rptList.DataSource = query.OrderByDescending(item => item.CreateTime)
                    .Skip((this.pager1.CurrentPageIndex - 1) * this.pager1.PageSize).Take(this.pager1.PageSize).ToList();
                this.rptList.DataBind();
            }
        }


        #endregion

    }
}