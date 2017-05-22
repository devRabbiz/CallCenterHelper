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
    public partial class ChatTextTypeView : PageBase<SPhone_ChatTextType>
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

        #region Override Methods

        protected override SPhone_ChatTextType LoadInfo(string id)
        {
            using (var db = DCHelper.SPhoneContext())
            {
                Guid entityID = new Guid(id);
                return db.SPhone_ChatTextType.FirstOrDefault(item => item.ChatTextTypeID == entityID);
            }
        }

        protected override void InitView(SPhone_ChatTextType model)
        {
            if (model != null)
            {
                this.txtChatTextTypeName.Text = model.ChatTextTypeName;
                this.txtSN.Text = model.SN.ToString();
            }
        }

        protected override void CollectInfo(Dictionary<string, object> context)
        {
            base.CollectInfo(context);
            this.NewPrimaryValue = Guid.NewGuid().ToString();
            context["ChatTextTypeName"] = this.txtChatTextTypeName.Text.Trim();
            context["SN"] = this.txtSN.Text.Trim();
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

        protected override void CustomValidation(SPhone_ChatTextType data, List<string> errorMessages)
        {
            if (string.IsNullOrEmpty(data.ChatTextTypeName))
                errorMessages.Add("ChatTextTypeName 不能为空！");
            if (data.SN == null)
                errorMessages.Add("SN 不能为空！");
        }

        protected override void PersistData(SPhone_ChatTextType data, bool isCreate)
        {
            using (var db = DCHelper.SPhoneContext())
            {
                if (isCreate)
                    db.SPhone_ChatTextType.Add(data);
                else
                {
                    var entity = db.SPhone_ChatTextType.FirstOrDefault(item => item.ChatTextTypeID == data.ChatTextTypeID);
                    this.UpdateEntity(entity, data);
                }
                db.SaveChanges();
            }
        }

        #endregion

    }
}