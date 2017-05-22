
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
    public partial class ChatTextList : PageBase<SPhone_ChatText>
    {

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                BindChatTextType();
            }
            BindRepeater();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindRepeater();
        }

        protected void pager1_OnPageChanged(object sender, EventArgs e)
        {
            BindRepeater();
        }

        protected void rptList_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            Guid id = Guid.Empty;
            Guid.TryParse((string)e.CommandArgument, out id);
            if (e.CommandName == "Status")
            {
                using (var db = DCHelper.SPhoneContext())
                {
                    SPhone_ChatText dn = db.SPhone_ChatText.Where(item => item.ChatTextID == id).FirstOrDefault();
                    if (dn != null)
                    {
                        dn.IsDeleted = 1;
                        db.SaveChanges();

                        BindRepeater();
                    }
                }
            }
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

        #endregion

        #region Protected Methods

        protected override void CollectInfo(Dictionary<string, object> context)
        {
            context["QueueName"] = this.txtQueueName.Text.Trim();
            context["IsDeleted"] = 0;
            if (!string.IsNullOrEmpty(this.ddlChatTextType.SelectedValue))
                context["ChatTextTypeID"] = this.ddlChatTextType.SelectedValue;
        }

        protected string GetTypeNameByID(string typeID)
        {
            Guid id = new Guid(typeID);
            SPhone_ChatTextType type = this.Types.Find(item => item.ChatTextTypeID == id);
            return (type != null) ? type.ChatTextTypeName : string.Empty;
        }

        #endregion

        #region Bind Controls

        private void BindChatTextType()
        {
            this.ddlChatTextType.DataSource = this.Types;
            this.ddlChatTextType.DataTextField = "ChatTextTypeName";
            this.ddlChatTextType.DataValueField = "ChatTextTypeID";
            this.ddlChatTextType.DataBind();
            this.ddlChatTextType.Items.Insert(0, new ListItem("——全部话术——", ""));
        }

        protected void BindRepeater()
        {
            var searcher = GetSearcher();
            using (var db = DCHelper.SPhoneContext())
            {
                var query = db.SPhone_ChatText.Where(item => item.IsDeleted == 0);
                if (!string.IsNullOrEmpty(searcher.QueueName))
                    query = query.Where(item => item.QueueName == searcher.QueueName);
                if (searcher.ChatTextTypeID.HasValue)
                    query = query.Where(item => item.ChatTextTypeID == searcher.ChatTextTypeID);


                this.pager1.RecordCount = query.Count();
                this.rptList.DataSource = query.OrderBy(item => item.ChatTextTypeID).ThenBy(item => item.SortID)
                    .Skip((this.pager1.CurrentPageIndex - 1) * this.pager1.PageSize).Take(this.pager1.PageSize).ToList();
                this.rptList.DataBind();
            }
        }


        #endregion

    }
}