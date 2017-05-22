

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
    public partial class BlackListView : PageBase<SPhone_BlackList>
    {
        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            this.RedirectUrl = "~/Pages/Public/BlackListList.aspx";
            if (!this.IsPostBack)
            {
                BindBlackListType();
            }
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
                return "BlackListID";
            }
        }

        #endregion

        #region Override Methods

        protected override SPhone_BlackList LoadInfo(string id)
        {
            using (var db = DCHelper.SPhoneContext())
            {
                Guid entityID = new Guid(id);
                return db.SPhone_BlackList.FirstOrDefault(item => item.BlackListID == entityID);
            }
        }

        protected override void InitView(SPhone_BlackList model)
        {
            if (model != null)
            {
                this.ddlBlackListType.SelectedValue = model.BlackListTypeID.ToString();
                this.txtBillNo.Text = model.BillNo;
            }
        }

        protected override void CollectInfo(Dictionary<string, object> context)
        {
            base.CollectInfo(context);
            this.NewPrimaryValue = Guid.NewGuid().ToString();
            context["BlackListTypeID"] = this.ddlBlackListType.SelectedValue;
            context["BillNo"] = this.txtBillNo.Text.Trim();
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

        protected override void CustomValidation(SPhone_BlackList data, List<string> errorMessages)
        {
            if (string.IsNullOrEmpty(data.BillNo))
                errorMessages.Add("BillNo 不能为空！");
        }

        protected override void PersistData(SPhone_BlackList data, bool isCreate)
        {
            using (var db = DCHelper.SPhoneContext())
            {
                if (isCreate)
                    db.SPhone_BlackList.Add(data);
                else
                {
                    var entity = db.SPhone_BlackList.FirstOrDefault(item => item.BlackListID == data.BlackListID);
                    this.UpdateEntity(entity, data);
                }
                db.SaveChanges();
            }
        }

        #endregion

        #region DataBind

        private void BindBlackListType()
        {
            List<SPhone_BlackListType> lst = null;
            using (var db = DCHelper.SPhoneContext())
            {
                lst = db.SPhone_BlackListType.ToList();
            }
            this.ddlBlackListType.DataSource = lst;
            this.ddlBlackListType.DataTextField = "TypeName";
            this.ddlBlackListType.DataValueField = "BlackListTypeID";
            this.ddlBlackListType.DataBind();
        }

        #endregion


    }
}