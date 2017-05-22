
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
    public partial class Sphone_CallList : PageBase<Sphone_Call>
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
            context["CustomerID"] = this.txtCustomerID.Text.Trim();
            context["CustomerID"] = this.txtCustomerID.Text.Trim();
            context["CallBeginTime"] = this.txtChatBeginTime.Text.Trim();
            context["CallEndTime"] = this.txtChatEndTime.Text.Trim();
            context["CurrentQueueName"] = this.txtQueueName.Text.Trim();
            context["MachineNo"] = this.txtMachineNo.Text.Trim();
            context["EmployeeID"] = this.txtEmployeeID.Text.Trim();
            context["PlaceIP"] = this.txtPlaceIP.Text.Trim();
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
            List<Sphone_Call> lst = null;
            using (var db = DCHelper.SPhoneContext())
            {
                //Predicate<Sphone_Call> match = this.InitSearchData();
                //Func<Sphone_Call, bool> contidion = new Func<Sphone_Call, bool>(item => match(item));
                //lst = db.Sphone_Call.Where(contidion).ToList();
            }
            this.pager1.RecordCount = lst.Count;
            this.rptList.DataSource = lst
                .Skip((this.pager1.CurrentPageIndex - 1) * this.pager1.PageSize).Take(this.pager1.PageSize);
            this.rptList.DataBind();
        }


        #endregion

    }
}