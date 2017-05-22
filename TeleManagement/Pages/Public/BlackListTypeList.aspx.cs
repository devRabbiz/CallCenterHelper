
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
    public partial class BlackListTypeList : PageBase<SPhone_BlackListType>
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
            context["TypeName"] = this.txtTypeName.Text.Trim();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindRepeater();
        }

        #region BindRepeater

        protected void BindRepeater()
        {
            var searcher = GetSearcher();
            using (var db = DCHelper.SPhoneContext())
            {
                var query = db.SPhone_BlackListType.Where(item => 1 == 1);
                if (!string.IsNullOrEmpty(searcher.TypeName))
                    query = query.Where(item => item.TypeName.IndexOf(searcher.TypeName) != -1);

                this.rptList.DataSource = query.OrderByDescending(item => item.CreateTime).ToList();
                this.rptList.DataBind();
            }
        }


        #endregion

    }
}