

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
    public partial class ChatTextView : PageBase<SPhone_ChatText>
    {
        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            this.RedirectUrl = "~/Pages/Chat/ChatTextList.aspx";
            if (!this.IsPostBack)
            {
                BindChatTextType();
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            this.SubmitData();
        }

        #endregion

        #region Properties

        private List<SPhone_ChatTextType> types = null;
        protected List<SPhone_ChatTextType> Types
        {
            get
            {
                if (types == null)
                {
                    using (var db = DCHelper.SPhoneContext())
                    {
                        types = db.SPhone_ChatTextType.ToList();
                    }
                }
                return types;
            }
        }

        protected override string PrimaryKey
        {
            get
            {
                return "ChatTextID";
            }
        }

        #endregion

        #region Override Methods

        protected override SPhone_ChatText LoadInfo(string id)
        {
            using (var db = DCHelper.SPhoneContext())
            {
                Guid entityID = new Guid(id);
                return db.SPhone_ChatText.FirstOrDefault(item => item.ChatTextID == entityID);
            }
        }

        protected override void InitView(SPhone_ChatText model)
        {
            if (model != null)
            {
                this.txtQueueName.Text = model.QueueName;
                this.txtSortID.Text = model.SortID.ToString();
                if (model.ChatTextTypeID != null)
                    this.ddlChatTextType.SelectedValue = model.ChatTextTypeID.ToString();
                this.txtChatContent.Text = model.ChatContent;
            }
        }

        protected override void CollectInfo(Dictionary<string, object> context)
        {
            base.CollectInfo(context);

            int sortID = 0;
            int.TryParse(this.txtSortID.Text, out sortID);

            this.NewPrimaryValue = Guid.NewGuid().ToString();
            context["QueueName"] = this.txtQueueName.Text.Trim();
            context["SortID"] = sortID;
            context["ChatTextTypeID"] = this.ddlChatTextType.SelectedValue;
            context["ChatContent"] = this.txtChatContent.Text.Trim();
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

        protected override void CustomValidation(SPhone_ChatText data, List<string> errorMessages)
        {
            if (string.IsNullOrEmpty(data.QueueName))
                errorMessages.Add("QueueName 不能为空！");
            if (string.IsNullOrEmpty(data.ChatContent))
                errorMessages.Add("ChatContent 不能为空！");
        }

        protected override void PersistData(SPhone_ChatText data, bool isCreate)
        {
            using (var db = DCHelper.SPhoneContext())
            {
                if (isCreate)
                    db.SPhone_ChatText.Add(data);
                else
                {
                    var entity = db.SPhone_ChatText.FirstOrDefault(item => item.ChatTextID == data.ChatTextID);
                    this.UpdateEntity(entity, data);
                }
                db.SaveChanges();
            }
        }

        #endregion

        #region BindChatTextType


        private void BindChatTextType()
        {
            this.ddlChatTextType.DataSource = this.Types;
            this.ddlChatTextType.DataTextField = "ChatTextTypeName";
            this.ddlChatTextType.DataValueField = "ChatTextTypeID";
            this.ddlChatTextType.DataBind();
            //this.ddlChatTextType.Items.Insert(0, new ListItem("——全部话术——", ""));
        }

        #endregion

    }
}