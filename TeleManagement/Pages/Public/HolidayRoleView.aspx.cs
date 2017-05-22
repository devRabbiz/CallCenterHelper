

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
    public partial class HolidayRoleView : PageBase<SPhone_HolidayRole>
    {
        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            this.RedirectUrl = "~/Pages/Public/HolidayList.aspx";
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
                return "HolidayRoleID";
            }
        }

        #endregion

        #region Override Methods

        protected override SPhone_HolidayRole LoadInfo(string id)
        {
            using (var db = DCHelper.SPhoneContext())
            {
                Guid entityID = new Guid(id);
                return db.SPhone_HolidayRole.FirstOrDefault(item => item.HolidayRoleID == entityID);
            }
        }

        protected override void InitView(SPhone_HolidayRole model)
        {
            if (model != null)
            {
                this.txtHolidayRoleName.Text = model.HolidayRoleName;
                //this.txtHolidayBegin.Text = model.HolidayBegin;
                //this.txtHolidayEnd.Text = model.HolidayEnd;
            }
        }

        protected override void CollectInfo(Dictionary<string, object> context)
        {
            base.CollectInfo(context);
            this.NewPrimaryValue = Guid.NewGuid().ToString();
            context["HolidayRoleName"] = this.txtHolidayRoleName.Text.Trim();
            context["HolidayBegin"] = this.txtHolidayBegin.Text.Trim();
            context["HolidayEnd"] = this.txtHolidayEnd.Text.Trim();
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

        protected override void CustomValidation(SPhone_HolidayRole data, List<string> errorMessages)
        {
            if (string.IsNullOrEmpty(data.HolidayRoleName))
                errorMessages.Add("HolidayRoleName 不能为空！");
            //if (string.IsNullOrEmpty(data.HolidayBegin))
            //    errorMessages.Add("HolidayBegin 不能为空！");
            //if (string.IsNullOrEmpty(data.HolidayEnd))
            //    errorMessages.Add("HolidayEnd 不能为空！");
        }

        protected override void PersistData(SPhone_HolidayRole data, bool isCreate)
        {
            using (var db = DCHelper.SPhoneContext())
            {
                if (isCreate)
                    db.SPhone_HolidayRole.Add(data);
                else
                {
                    var entity = db.SPhone_HolidayRole.FirstOrDefault(item => item.HolidayRoleID == data.HolidayRoleID);
                    this.UpdateEntity(entity, data);
                }
                db.SaveChanges();
            }
        }

        #endregion

    }
}