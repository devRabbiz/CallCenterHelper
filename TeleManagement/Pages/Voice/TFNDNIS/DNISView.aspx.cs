using SoftPhone.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Tele.Management.Pages.Voice.TFNDNIS
{
    public partial class DNISView : PageBase<SPhone_DNIS>
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.RedirectUrl = "~/Pages/Voice/TFNDNIS/DNISList.aspx";
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            this.SubmitData();
        }

        protected override SPhone_DNIS LoadInfo(string id)
        {
            var id2 = long.Parse(id);
            return dc.SPhone_DNIS.FirstOrDefault(x => x.DNISID == id2);
        }

        protected override void InitView(SPhone_DNIS model)
        {
            if (!IsPostBack)
            {

            }
            if (model != null)
            {
                txtDNISName.Text = model.DNISName;
            }
        }

        protected override void CollectInfo(Dictionary<string, object> context)
        {
            base.CollectInfo(context);
            context["DNISName"] = txtDNISName.Text;

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

        protected override void CustomValidation(SPhone_DNIS data, List<string> errorMessages)
        {
            if (string.IsNullOrEmpty(data.DNISName))
                errorMessages.Add("小号 不能为空！");
            else
            {

                if (string.IsNullOrEmpty(this.PrimaryValue))
                {
                    var info = dc.SPhone_DNIS.FirstOrDefault(x => x.DNISName == data.DNISName);
                    if (info != null)
                    {
                        errorMessages.Add("小号 已经存在！");
                    }
                }
            }
        }

        protected override void PersistData(SPhone_DNIS data, bool isCreate)
        {

            if (isCreate)
            {
                dc.SPhone_DNIS.Add(data);
            }
            else
            {
                var entity = dc.SPhone_DNIS.FirstOrDefault(x => x.DNISID == data.DNISID);
                this.UpdateEntity(entity, data);
            }
            dc.SaveChanges();

        }
    }
}