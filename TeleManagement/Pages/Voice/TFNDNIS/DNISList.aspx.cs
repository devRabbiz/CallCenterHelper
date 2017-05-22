using SoftPhone.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Tele.DataLibrary;

namespace Tele.Management.Pages.Voice.TFNDNIS
{
    public partial class DNISList : PageBase<SPhone_DNIS>
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            BindRepeater();
        }

        protected override void CollectInfo(Dictionary<string, object> context)
        {
            context["DNISName"] = txtDNISName.Text.Trim();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindRepeater();
        }

        protected void pager1_PageChanged(object sender, EventArgs e)
        {
            BindRepeater();
        }

        protected void rptList_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Del")
            {
                var id = long.Parse(e.CommandArgument.ToString());
                var info = dc.SPhone_DNIS.FirstOrDefault(x => x.DNISID == id);
                if (info != null)
                {
                    dc.SPhone_DNIS.Remove(info);
                    dc.SaveChanges();
                }
                BindRepeater();
            }
        }

        protected void BindRepeater()
        {
            var searcher = GetSearcher();
            using (var db = DCHelper.SPhoneContext())
            {
                var query = db.SPhone_DNIS.Where(x => 1 == 1);

                if (!string.IsNullOrEmpty(searcher.DNISName))
                {
                    query = query.Where(x => x.DNISName.Contains(searcher.DNISName));
                }

                query = query.OrderByDescending(x => x.DNISID);

                rptList.DataSource = query.Skip((pager1.CurrentPageIndex - 1) * pager1.PageSize).Take(pager1.PageSize).ToList();
                rptList.DataBind();
                pager1.RecordCount = query.Count();
            }
        }
    }
}