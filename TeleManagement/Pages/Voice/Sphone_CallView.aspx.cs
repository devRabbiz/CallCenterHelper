

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
    public partial class Sphone_CallView : PageBase<Sphone_Call>
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
                return "CallID";
            }
        }

        #endregion

        #region Override Methods

        protected override Sphone_Call LoadInfo(string id)
        {
            using (var db = DCHelper.SPhoneContext())
            {
                return db.Sphone_Call.FirstOrDefault(item => item.CallID == id);
            }
        }

        protected override void InitView(Sphone_Call model)
        {
            if (model != null)
            {
                this.txtEmployeeID.Text = model.EmployeeID;
                //this.txtCustomerID.Text = model.CustomerID;
                //this.txtCallBeginTime.Text = model.CallBeginTime;
                //this.txtCallEndTime.Text = model.CallEndTime;
                //this.txtDeskTime.Text = model.DeskTime;
                //this.txtConnectionID.Text = model.ConnectionID;
                //this.txtInOut.Text = model.InOut;
                this.txtCurrentQueueName.Text = model.CurrentQueueName;
                this.txtFromQueueName.Text = model.FromQueueName;
                this.txtNextQueueName.Text = model.NextQueueName;
                this.txtANI.Text = model.ANI;
                this.txtDNIS.Text = model.DNIS;
                this.txtPlaceIP.Text = model.PlaceIP;
                //this.txtIsConference.Text = model.IsConference;
                //this.txtIsTransfer.Text = model.IsTransfer;
                //this.txtIsTransferEPOS.Text = model.IsTransferEPOS;
            }
        }

        protected override void CollectInfo(Dictionary<string, object> context)
        {
            base.CollectInfo(context);
            this.NewPrimaryValue = Guid.NewGuid().ToString();
            context["EmployeeID"] = this.txtEmployeeID.Text.Trim();
            context["CustomerID"] = this.txtCustomerID.Text.Trim();
            context["CallBeginTime"] = this.txtCallBeginTime.Text.Trim();
            context["CallEndTime"] = this.txtCallEndTime.Text.Trim();
            context["DeskTime"] = this.txtDeskTime.Text.Trim();
            context["ConnectionID"] = this.txtConnectionID.Text.Trim();
            context["InOut"] = this.txtInOut.Text.Trim();
            context["CurrentQueueName"] = this.txtCurrentQueueName.Text.Trim();
            context["FromQueueName"] = this.txtFromQueueName.Text.Trim();
            context["NextQueueName"] = this.txtNextQueueName.Text.Trim();
            context["ANI"] = this.txtANI.Text.Trim();
            context["DNIS"] = this.txtDNIS.Text.Trim();
            context["PlaceIP"] = this.txtPlaceIP.Text.Trim();
            context["IsConference"] = this.txtIsConference.Text.Trim();
            context["IsTransfer"] = this.txtIsTransfer.Text.Trim();
            context["IsTransferEPOS"] = this.txtIsTransferEPOS.Text.Trim();
            context["CreateBy"] = this.txtCreateBy.Text.Trim();
            context["CreateTime"] = this.txtCreateTime.Text.Trim();
            context["UpdateBy"] = this.txtUpdateBy.Text.Trim();
            context["UpdateTime"] = this.txtUpdateTime.Text.Trim();
        }

        protected override void CustomValidation(Sphone_Call data, List<string> errorMessages)
        {
        }

        protected override void PersistData(Sphone_Call data, bool isCreate)
        {
            using (var db = DCHelper.SPhoneContext())
            {
                if (isCreate)
                    db.Sphone_Call.Add(data);
                else
                {
                    var entity = db.Sphone_Call.FirstOrDefault(item => item.CallID == data.CallID);
                    this.UpdateEntity(entity, data);
                }
                db.SaveChanges();
            }
        }

        #endregion

    }
}