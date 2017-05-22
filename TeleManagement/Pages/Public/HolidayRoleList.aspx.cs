
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
    public partial class HolidayRoleList : PageBase<SPhone_HolidayRole>
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
            context["HolidayRoleName"] = this.txtHolidayRoleName.Text.Trim();
            context["HolidayBegin"] = this.txtHolidayBegin.Text.Trim();
            context["HolidayEnd"] = this.txtHolidayEnd.Text.Trim();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindRepeater();
        }

        #region BindRepeater

        protected void BindRepeater()
        {
            List<SPhone_HolidayRole> lst = null;
            using (var db = DCHelper.SPhoneContext())
            {
                //Predicate<SPhone_HolidayRole> match = this.InitSearchData();
                //Func<SPhone_HolidayRole, bool> contidion = new Func<SPhone_HolidayRole, bool>(item => match(item));
                //lst = db.SPhone_HolidayRole.Where(contidion).ToList();
            }
            this.rptList.DataSource = lst;
            this.rptList.DataBind();
        }


        #endregion

    }
}