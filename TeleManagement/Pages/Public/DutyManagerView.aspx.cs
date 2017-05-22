

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
    public partial class DutyManagerView : PageBase<SPhone_DutyManager>
    {
        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            this.RedirectUrl = "~/Pages/Public/DutyManagerList.aspx";
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
                return "DutyManagerID";
            }
        }

        #endregion

        #region Override Methods

        protected override SPhone_DutyManager LoadInfo(string id)
        {
            using (var db = DCHelper.SPhoneContext())
            {
                Guid entityID = new Guid(id);
                return db.SPhone_DutyManager.FirstOrDefault(item => item.DutyManagerID == entityID);
            }
        }

        protected override void InitView(SPhone_DutyManager model)
        {
            if (model != null)
            {
                this.txtManagerName.Text = model.ManagerName;
                this.txtPhoneNo.Text = model.PhoneNo;
                this.txtBeginDate.Text = string.Format("{0:yyyy-MM-dd}", model.DutyDate);
            }
        }

        protected override void CollectInfo(Dictionary<string, object> context)
        {
            base.CollectInfo(context);
            this.NewPrimaryValue = Guid.NewGuid().ToString();
            context["ManagerName"] = this.txtManagerName.Text.Trim();
            context["PhoneNo"] = this.txtPhoneNo.Text.Trim();
            context["DutyDate"] = this.txtBeginDate.Text.Trim();
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

        protected override void CustomValidation(SPhone_DutyManager data, List<string> errorMessages)
        {
            if (string.IsNullOrEmpty(data.ManagerName))
                errorMessages.Add("ManagerName 不能为空！");
            if (string.IsNullOrEmpty(data.PhoneNo))
                errorMessages.Add("PhoneNo 不能为空！");
            if (data.DutyDate == null)
                errorMessages.Add("DutyDate 不能为空！");
        }

        protected override void PersistData(SPhone_DutyManager data, bool isCreate)
        {
            using (var db = DCHelper.SPhoneContext())
            {
                if (isCreate)
                    db.SPhone_DutyManager.Add(data);
                else
                {
                    var entity = db.SPhone_DutyManager.FirstOrDefault(item => item.DutyManagerID == data.DutyManagerID);
                    this.UpdateEntity(entity, data);
                }
                db.SaveChanges();
            }
        }

        #endregion

    }
}