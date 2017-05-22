

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
    public partial class BlackListTypeView : PageBase<SPhone_BlackListType>
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
                return "BlackListTypeID";
            }
        }

        #endregion

        #region Override Methods

        protected override SPhone_BlackListType LoadInfo(string id)
        {
            using (var db = DCHelper.SPhoneContext())
            {
                Guid entityID = new Guid(id);
                return db.SPhone_BlackListType.FirstOrDefault(item => item.BlackListTypeID == entityID);
            }
        }

        protected override void InitView(SPhone_BlackListType model)
        {
            if (model != null)
            {
                this.txtTypeName.Text = model.TypeName;
            }
        }

        protected override void CollectInfo(Dictionary<string, object> context)
        {
            base.CollectInfo(context);
            this.NewPrimaryValue = Guid.NewGuid().ToString();
            context["TypeName"] = this.txtTypeName.Text.Trim();
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

        protected override void CustomValidation(SPhone_BlackListType data, List<string> errorMessages)
        {
            if (string.IsNullOrEmpty(data.TypeName))
                errorMessages.Add("TypeName 不能为空！");
        }

        protected override void PersistData(SPhone_BlackListType data, bool isCreate)
        {
            using (var db = DCHelper.SPhoneContext())
            {
                if (isCreate)
                    db.SPhone_BlackListType.Add(data);
                else
                {
                    var entity = db.SPhone_BlackListType.FirstOrDefault(item => item.BlackListTypeID == data.BlackListTypeID);
                    this.UpdateEntity(entity, data);
                }
                db.SaveChanges();
            }
        }

        #endregion

    }
}