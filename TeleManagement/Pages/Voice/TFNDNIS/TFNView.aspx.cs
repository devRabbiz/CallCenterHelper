using SoftPhone.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Tele.Management.Pages.Voice.TFNDNIS
{
    public partial class TFNView : PageBase<SPhone_TFN>
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.RedirectUrl = "~/Pages/Voice/TFNDNIS/TFNList.aspx";
            if (!IsPostBack)
            {
                var qType = dc.SPhone_NameValue.Where(x => x.TypeCode == "TFNBusi" && ((x.IsDel.HasValue && x.IsDel != 1) || x.IsDel == null)).OrderBy(x => x.Sort).ToList();
                txtTFNBusi.DataValueField = "Value";
                txtTFNBusi.DataTextField = "Name";
                txtTFNBusi.DataSource = qType;
                txtTFNBusi.DataBind();
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            this.SubmitData();
        }

        protected override SPhone_TFN LoadInfo(string id)
        {
            var id2 = long.Parse(id);
            return dc.SPhone_TFN.FirstOrDefault(x => x.TFNID == id2);
        }

        protected override void InitView(SPhone_TFN model)
        {
            if (!IsPostBack)
            {

            }
            if (model != null)
            {
                txtTFNName.Text = model.TFNName;
                txtTFNBusi.Text = model.TFNBusi;
                txtTFNCallFlow.Text = model.TFNCallFlow;
            }
        }

        protected override void CollectInfo(Dictionary<string, object> context)
        {
            base.CollectInfo(context);
            context["TFNName"] = txtTFNName.Text;
            context["TFNBusi"] = txtTFNBusi.Text;
            context["TFNCallFlow"] = txtTFNCallFlow.Text;

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

        protected override void CustomValidation(SPhone_TFN data, List<string> errorMessages)
        {
            if (string.IsNullOrEmpty(data.TFNName))
            {
                errorMessages.Add("大号 不能为空！");
            }
            else
            {
                if (string.IsNullOrEmpty(this.PrimaryValue))
                {
                    var info = dc.SPhone_DNIS.FirstOrDefault(x => x.DNISName == data.TFNName);
                    if (info != null)
                    {
                        errorMessages.Add("大号 已经存在！");
                    }
                }
            } if (string.IsNullOrEmpty(data.TFNCallFlow))
            {
                errorMessages.Add("应用文件名 不能为空！");
            }
        }

        protected override void PersistData(SPhone_TFN data, bool isCreate)
        {

            if (isCreate)
            {
                dc.SPhone_TFN.Add(data);
            }
            else
            {
                var entity = dc.SPhone_TFN.FirstOrDefault(x => x.TFNID == data.TFNID);
                this.UpdateEntity(entity, data);
            }
            dc.SaveChanges();

        }
    }
}