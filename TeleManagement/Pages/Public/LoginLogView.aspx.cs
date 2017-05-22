

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Tele.DataLibrary;
using SoftPhone.Entity;

namespace Tele.Management.Pages
{
    public partial class LoginLogView : PageBase<SPhone_LoginLog>
    {
        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            this.SubmitData();
        }

        #endregion

        #region Properties

        protected override string PrimaryKey
        {
            get
            {
                return "ID";
            }
        }

        #endregion

        #region Override Methods

        protected override SPhone_LoginLog LoadInfo(string id)
        {
            using (var db = DCHelper.SPhoneContext())
            {
                Guid entityID = new Guid(id);
                return db.SPhone_LoginLog.FirstOrDefault(item => item.ID == entityID);
            }
        }

        protected override void InitView(SPhone_LoginLog model)
        {
            if (model != null)
            {
                this.txtEmployeeID.Text = model.EmployeeID;
                //this.txtLoginDate.Text = model.LoginDate;
                //this.txtFirstloginTime.Text = model.FirstloginTime;
                //this.txtFirstlogoutTime.Text = model.FirstlogoutTime;
                //this.txtLastlogout.Text = model.Lastlogout;
            }
        }

        protected override void CollectInfo(Dictionary<string, object> context)
        {
            base.CollectInfo(context);
            this.NewPrimaryValue = Guid.NewGuid().ToString();
            context["EmployeeID"] = this.txtEmployeeID.Text.Trim();
            context["LoginDate"] = this.txtLoginDate.Text.Trim();
            context["FirstloginTime"] = this.txtFirstloginTime.Text.Trim();
            context["FirstlogoutTime"] = this.txtFirstlogoutTime.Text.Trim();
            context["Lastlogout"] = this.txtLastlogout.Text.Trim();
            if (string.IsNullOrEmpty(this.PrimaryValue))
            {
                context["CreateBy"] = this.UserInfo.LogonID;
                context["CreateTime"] = DateTime.Now;
            }
            else
            {
                context["UpdateBy"] = this.UserInfo.LogonID;
                context["UpdateTime"] = DateTime.Now;
            }
        }

        protected override void CustomValidation(SPhone_LoginLog data, List<string> errorMessages)
        {
            if (string.IsNullOrEmpty(data.EmployeeID))
                errorMessages.Add("EmployeeID 不能为空！");
            //if (string.IsNullOrEmpty(data.LoginDate))
            //    errorMessages.Add("LoginDate 不能为空！");
            //if (string.IsNullOrEmpty(data.FirstloginTime))
            //    errorMessages.Add("FirstloginTime 不能为空！");
            //if (string.IsNullOrEmpty(data.FirstlogoutTime))
            //    errorMessages.Add("FirstlogoutTime 不能为空！");
            //if (string.IsNullOrEmpty(data.Lastlogout))
            //    errorMessages.Add("Lastlogout 不能为空！");
        }

        protected override void PersistData(SPhone_LoginLog data, bool isCreate)
        {
            using (var db = DCHelper.SPhoneContext())
            {
                if (isCreate)
                    db.SPhone_LoginLog.Add(data);
                else
                {
                    var entity = db.SPhone_LoginLog.FirstOrDefault(item => item.ID == data.ID);
                    this.UpdateEntity(entity, data);
                }
                db.SaveChanges();
            }
        }

        #endregion

    }
}