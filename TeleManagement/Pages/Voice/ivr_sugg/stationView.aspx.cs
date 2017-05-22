using SoftPhone.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Tele.DataLibrary;

namespace Tele.Management.Pages.Voice.ivr_sugg
{
    public partial class stationView : PageBase<ivr_sugg_station>
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.RedirectUrl = "~/Pages/Voice/ivr_sugg/stationList.aspx";
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            this.SubmitData();
        }

        protected override ivr_sugg_station LoadInfo(string id)
        {
            using (var db = DCHelper.SPhoneContext())
            {
                var a = id.Split(',')[0];
                var b = id.Split(',')[1];
                return db.ivr_sugg_station.FirstOrDefault(x => x.PRODUCT_TYPE == b && x.AGENCY_ID == a && x.VALID == "1");
            }
        }

        protected override void InitView(ivr_sugg_station model)
        {
            if (!IsPostBack)
            {
                var qType = dc.SPhone_NameValue.Where(x => x.TypeCode == "StationProductType" && ((x.IsDel.HasValue && x.IsDel != 1) || x.IsDel == null)).OrderBy(x => x.Sort).ToList();
                txtPRODUCT_TYPE.DataValueField = "Value";
                txtPRODUCT_TYPE.DataTextField = "Name";
                txtPRODUCT_TYPE.DataSource = qType;
                txtPRODUCT_TYPE.DataBind();
            }
            if (model != null && !string.IsNullOrWhiteSpace(model.AGENCY_ID))
            {
                txtPRODUCT_TYPE.Enabled = txtAGENCY_ID.Enabled = false;

                txtAGENCY_ID.Text = model.AGENCY_ID.Trim();
                txtPRODUCT_TYPE.Text = model.PRODUCT_TYPE.Trim();
                txtAGENCY_NAME.Text = model.AGENCY_NAME.Trim();
                //txtAGENCY_NAME_WAV_FILENAME.Text = model.AGENCY_NAME_WAV_FILENAME;
                txtAGENCY_COVER_AREA.Text = model.AGENCY_COVER_AREA.Trim();
                txtAGENCY_INFO.Text = model.AGENCY_INFO.Trim();
                //txtFAX_AGENCY_INFO.Text = model.FAX_AGENCY_INFO;
                //txtAGENCY_WAV_FILENAME.Text = model.AGENCY_WAV_FILENAME;
                //txtVALID.Text = model.VALID;
                //txtRECORD_DATE.Text = model.RECORD_DATE;
                txtCATEGORY.Text = model.CATEGORY.Trim();
                txtADD_TYPE.Text = model.ADD_TYPE.Trim();
            }
        }

        protected override void CollectInfo(Dictionary<string, object> context)
        {
            base.CollectInfo(context);
            context["AGENCY_ID"] = txtAGENCY_ID.Text;
            context["PRODUCT_TYPE"] = txtPRODUCT_TYPE.Text;
            context["AGENCY_NAME"] = txtAGENCY_NAME.Text;
            //context["AGENCY_NAME_WAV_FILENAME"] = txtAGENCY_NAME_WAV_FILENAME.Text;
            context["AGENCY_COVER_AREA"] = txtAGENCY_COVER_AREA.Text;
            context["AGENCY_INFO"] = txtAGENCY_INFO.Text;
            //context["FAX_AGENCY_INFO"] = txtFAX_AGENCY_INFO.Text;
            //context["AGENCY_WAV_FILENAME"] = txtAGENCY_WAV_FILENAME.Text;
            //context["VALID"] = txtVALID.Text;
            //context["RECORD_DATE"] = txtRECORD_DATE.Text;
            context["CATEGORY"] = txtCATEGORY.Text;
            context["ADD_TYPE"] = txtADD_TYPE.Text;

        }

        protected override void CustomValidation(ivr_sugg_station data, List<string> errorMessages)
        {
            if (string.IsNullOrEmpty(this.PrimaryValue))
            {
                var c = dc.ivr_sugg_station.Count(x => x.PRODUCT_TYPE == data.PRODUCT_TYPE && x.AGENCY_ID == data.AGENCY_ID);
                if (c > 0) {
                    errorMessages.Add("维修站编号+产线 已经存在，不允许添加！");
                }
            }
            if (string.IsNullOrEmpty(data.AGENCY_ID))
                errorMessages.Add("维修站编号 不能为空！");
            if (string.IsNullOrEmpty(data.PRODUCT_TYPE))
                errorMessages.Add("产线 不能为空！");
            if (string.IsNullOrEmpty(data.AGENCY_COVER_AREA))
                errorMessages.Add("区号 不能为空！");
        }

        protected override void PersistData(ivr_sugg_station data, bool isCreate)
        {
            using (var db = DCHelper.SPhoneContext())
            {
                if (isCreate)
                {
                    data.VALID = "1";
                    data.AGENCY_NAME_WAV_FILENAME = data.AGENCY_ID + "s.WAV";
                    data.AGENCY_WAV_FILENAME = data.AGENCY_ID + ".WAV";
                    data.RECORD_DATE = DateTime.Now;
                    db.ivr_sugg_station.Add(data);
                }
                else
                {
                    var entity = db.ivr_sugg_station.FirstOrDefault(x => x.AGENCY_ID == data.AGENCY_ID && x.PRODUCT_TYPE == data.PRODUCT_TYPE);

                    entity.AGENCY_COVER_AREA = data.AGENCY_COVER_AREA;
                    entity.AGENCY_NAME = data.AGENCY_NAME;
                    entity.AGENCY_INFO = data.AGENCY_INFO;
                    entity.ADD_TYPE = data.ADD_TYPE;
                    entity.CATEGORY = data.CATEGORY;

                    //entity.AGENCY_NAME_WAV_FILENAME = data.AGENCY_ID + "s.WAV";
                    //entity.AGENCY_WAV_FILENAME = data.AGENCY_ID + ".WAV";
                    entity.RECORD_DATE = DateTime.Now;
                }
                db.SaveChanges();
            }
        }
    }
}