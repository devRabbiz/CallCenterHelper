using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Tele.DataLibrary;
using SoftPhone.Entity;

namespace Tele.Management.Pages.Public
{
    public partial class IpdnView : PageBase<SPhone_IPDN>
    {
        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            this.RedirectUrl = "~/Pages/Voice/IpdnList.aspx";
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            this.SubmitData();
        }

        #endregion


        #region Override Methods

        protected override SPhone_IPDN LoadInfo(string id)
        {
            using (var db = DCHelper.SPhoneContext())
            {
                Guid entityID = new Guid(id);
                return db.SPhone_IPDN.FirstOrDefault(item => item.ID == entityID);
            }
        }

        protected override void InitView(SPhone_IPDN model)
        {
            if (model != null)
            {
                this.txtPlace.Text = model.Place;
                this.txtPlaceIP.Text = model.PlaceIP;
                this.txtDNNo.Text = model.DNNo;
                this.txtPhoneIP.Text = model.PhoneIP;
                if (!string.IsNullOrEmpty(model.BindType))
                    this.ddlBindType.SelectedValue = model.BindType;
                if (!string.IsNullOrEmpty(model.PhoneType))
                    this.ddlPhoneType.SelectedValue = model.PhoneType;
                this.ddlIsSayEmpNO.Text = model.IsSayEmpNO.ToString();
                this.ddlIsRealDN.Text = model.IsRealDN.ToString();
            }
        }

        protected override void CollectInfo(Dictionary<string, object> context)
        {
            base.CollectInfo(context);
            this.NewPrimaryValue = Guid.NewGuid().ToString();
            context["IsValid"] = this.ddlIsValid.SelectedValue == "1" ? 1 : 0;
            context["Place"] = this.txtPlace.Text.Trim();
            context["PlaceIP"] = this.txtPlaceIP.Text.Trim();
            context["DNNo"] = this.txtDNNo.Text.Trim();
            context["PhoneIP"] = this.txtPhoneIP.Text.Trim();
            context["PhoneType"] = this.ddlPhoneType.SelectedValue;
            context["BindType"] = this.ddlBindType.SelectedValue;
            context["IsSayEmpNO"] = int.Parse(this.ddlIsSayEmpNO.Text);
            context["IsRealDN"] = int.Parse( this.ddlIsRealDN.Text);

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

        protected override void CustomValidation(SPhone_IPDN data, List<string> errorMessages)
        {
            if (string.IsNullOrEmpty(data.Place))
                errorMessages.Add("Place 不能为空！");
            if (string.IsNullOrEmpty(data.DNNo))
                errorMessages.Add("DN 不能为空！");
            if (string.IsNullOrEmpty(data.PlaceIP))
                errorMessages.Add("IP 不能为空！");

        }

        protected override void PersistData(SPhone_IPDN data, bool isCreate)
        {
            using (var db = DCHelper.SPhoneContext())
            {
                if (isCreate)
                    db.SPhone_IPDN.Add(data);
                else
                {
                    var entity = db.SPhone_IPDN.FirstOrDefault(item => item.ID == data.ID);
                    this.UpdateEntity(entity, data);
                }
                db.SaveChanges();
            }
        }

        #endregion

    }
}