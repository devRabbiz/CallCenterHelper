
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
    public partial class ChatTextTypeList : PageBase<SPhone_ChatTextType>
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
                        context["ChatTextTypeName"] = this.txtChatTextTypeName.Text.Trim();
                        context["SN"] = this.txtSN.Text.Trim();
                        context["CreateBy"] = this.txtCreateBy.Text.Trim();
                        context["CreateTime"] = this.txtCreateTime.Text.Trim();
                        context["UpdateBy"] = this.txtUpdateBy.Text.Trim();
                        context["UpdateTime"] = this.txtUpdateTime.Text.Trim();
                    }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindRepeater();
        }

        #region BindRepeater

        protected void BindRepeater()
        {
            List<SPhone_ChatTextType> lst = null;
            using (var db = DCHelper.SPhoneContext())
            {
                Predicate<SPhone_ChatTextType> match = this.InitSearchData();
                Func<SPhone_ChatTextType, bool> contidion = new Func<SPhone_ChatTextType, bool>(item => match(item));
                lst = db.SPhone_ChatTextType.Where(contidion).ToList();
            }
            this.rptList.DataSource = lst;
            this.rptList.DataBind();
        }


        #endregion

    }
}