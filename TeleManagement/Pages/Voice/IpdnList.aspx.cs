using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Tele.DataLibrary;
using SoftPhone.Entity;
using System.Linq.Expressions;

namespace Tele.Management.Pages.Public
{
    public partial class IpdnList : PageBase<SPhone_IPDN>
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            BindRepeater();
        }

        protected override void CollectInfo(Dictionary<string, object> context)
        {
            context["Place"] = this.txtPlace.Text.Trim();
            context["PlaceIP"] = this.txtPlaceIP.Text.Trim();
            context["DNNo"] = this.txtDNNo.Text.Trim();
            context["IsValid"] = Convert.ToInt16(this.ddlStatus.SelectedValue);
            context["BindType"] = this.ddlBindType.SelectedValue;
            context["PhoneType"] = this.ddlPhoneType.SelectedValue;
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
                    SPhone_IPDN dn = db.SPhone_IPDN.Where(item => item.ID == id).FirstOrDefault();
                    if (dn != null)
                    {
                        dn.IsValid = 0;
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
               var query = db.SPhone_IPDN.Where(item => item.IsValid == searcher.IsValid);
                if (!string.IsNullOrEmpty(searcher.Place))
                    query = query.Where(item => item.Place.IndexOf(searcher.Place) != -1);
                if (!string.IsNullOrEmpty(searcher.PlaceIP))
                    query = query.Where(item => item.PlaceIP.IndexOf(searcher.PlaceIP) != -1);
                if (!string.IsNullOrEmpty(searcher.DNNo))
                    query = query.Where(item => item.DNNo.IndexOf(searcher.DNNo) != -1);
                if (!string.IsNullOrEmpty(searcher.BindType))
                    query = query.Where(item => item.BindType == searcher.BindType);
                if (!string.IsNullOrEmpty(searcher.PhoneType))
                    query = query.Where(item => item.PhoneType == searcher.PhoneType);

                this.pager1.RecordCount = query.Count();
                this.rptList.DataSource = query.OrderBy(item => item.DNNo)
                    .Skip((this.pager1.CurrentPageIndex - 1) * this.pager1.PageSize).Take(this.pager1.PageSize).ToList();
                this.rptList.DataBind();
            }
        }


        #endregion


    }
}