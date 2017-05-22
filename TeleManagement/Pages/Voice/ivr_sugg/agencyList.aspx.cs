using SoftPhone.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Tele.DataLibrary;

namespace Tele.Management.Pages.Voice.ivr_sugg
{
    public partial class agencyList : PageBase<ivr_sugg_agency>
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var qType = dc.SPhone_NameValue.Where(x => x.TypeCode == "AgencyProductType" && ((x.IsDel.HasValue && x.IsDel != 1) || x.IsDel == null)).OrderBy(x => x.Sort).ToList();
                txtPRODUCT_TYPE.DataValueField = "Value";
                txtPRODUCT_TYPE.DataTextField = "Name";
                txtPRODUCT_TYPE.DataSource = qType;
                txtPRODUCT_TYPE.DataBind();
            }
            BindRepeater();
        }

        protected override void CollectInfo(Dictionary<string, object> context)
        {
            context["AGENCY_ID"] = txtAGENCY_ID.Text.Trim();
            context["AGENCY_NAME"] = txtAGENCY_NAME.Text.Trim();
            context["PRODUCT_TYPE"] = txtPRODUCT_TYPE.Text.Trim();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindRepeater();
        }

        protected void rptList_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Del")
            {
                var id = e.CommandArgument.ToString().Split(',')[0];
                var type = e.CommandArgument.ToString().Split(',')[1];
                using (var db = DCHelper.SPhoneContext())
                {
                    var info = db.ivr_sugg_agency.FirstOrDefault(x => x.AGENCY_ID == id && x.PRODUCT_TYPE == type);
                    if (info != null)
                    {
                        info.VALID = "0";
                        db.SaveChanges();
                    }
                }
                BindRepeater();
            }
        }

        protected void pager1_PageChanged(object sender, EventArgs e)
        {
            BindRepeater();
        }

        protected void BindRepeater()
        {
            var searcher = GetSearcher();
            using (var db = DCHelper.SPhoneContext())
            {
                var query = db.ivr_sugg_agency.Where(x => x.VALID == "1");

                if (!string.IsNullOrEmpty(searcher.AGENCY_NAME))
                {
                    query = query.Where(x => x.AGENCY_NAME.Contains(searcher.AGENCY_NAME));
                }
                if (!string.IsNullOrEmpty(searcher.PRODUCT_TYPE))
                {
                    query = query.Where(x => x.PRODUCT_TYPE == searcher.PRODUCT_TYPE);
                }
                if (!string.IsNullOrEmpty(searcher.AGENCY_ID))
                {
                    query = query.Where(x => x.AGENCY_ID == searcher.AGENCY_ID);
                }
                query = query.OrderBy(x => x.AGENCY_ID).ThenByDescending(x => x.CATEGORY);

                rptList.DataSource = query.Skip((pager1.CurrentPageIndex - 1) * pager1.PageSize).Take(pager1.PageSize).ToList();
                rptList.DataBind();
                pager1.RecordCount = query.Count();
            }
        }
    }
}