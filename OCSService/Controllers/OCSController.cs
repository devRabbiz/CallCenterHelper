using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OCSService.Controllers
{
    public class OCSController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        #region LoadCampaign,StartDialing
        public ActionResult LoadCampaign(int campaignId, int groupId, int sessionId)
        {
            var errMsg = string.Empty;
            var r = false;
            var errCode = 0;
            r = SupportClass.OCSHelperInterface.GetHelper().LoadCampaign(campaignId, groupId, sessionId, ref errMsg);
            if (!r)
            {
                errCode = -1;
            }
            return Jsonp(new { errCode = errCode, errMsg = errMsg }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult StartDialing(int campaignId, int groupId, int sessionId)
        {
            var errMsg = string.Empty;
            var r = false;
            var errCode = 0;

            r = SupportClass.OCSHelperInterface.GetHelper().StartDialing(campaignId, groupId, sessionId, ref errMsg);
            if (!r)
            {
                errCode = -1;
            }
            return Jsonp(new { errCode = errCode, errMsg = errMsg }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region StopDailing UnloadCampaign
        public ActionResult StopDailing(int campaignId, int groupId, int sessionId)
        {
            var errMsg = string.Empty;
            var r = false;
            var errCode = 0;

            r = SupportClass.OCSHelperInterface.GetHelper().StopDailing(campaignId, groupId, sessionId, ref errMsg);
            if (!r)
            {
                errCode = -1;
            }
            return Jsonp(new { errCode = errCode, errMsg = errMsg }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult UnloadCampaign(int campaignId, int groupId, int sessionId)
        {
            var errMsg = string.Empty;
            var r = false;
            var errCode = 0;

            r = SupportClass.OCSHelperInterface.GetHelper().UnloadCampaign(campaignId, groupId, sessionId, ref errMsg);
            if (!r)
            {
                errCode = -1;
            }
            return Jsonp(new { errCode = errCode, errMsg = errMsg }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region GetCampaignStatus
        public ActionResult GetCampaignStatus(int campaignId, int groupId, int sessionId)
        {
            var errMsg = string.Empty;
            var r = "";
            var errCode = 0;

            r = SupportClass.OCSHelperInterface.GetHelper().GetCampaignStatus(campaignId, groupId, sessionId, ref errMsg);
            if (r == "")
            {
                errCode = -1;
            }
            return Jsonp(new { errCode = errCode, errMsg = errMsg, campaignStatus = r }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Update
        public ActionResult OCSTableUpdate(string TableName, DateTime DialSchedTime, string ActivityID, string SampleID)
        {
            var errCode = 0;
            var errMsg = string.Empty;
            try
            {
                SqlParameter[] pars =
                {
                    new SqlParameter("@TableName",SqlDbType.VarChar,50),
                    new SqlParameter("@DialSchedTime",SqlDbType.DateTime),
                    new SqlParameter("@ActivityID",SqlDbType.VarChar,50),
                    new SqlParameter("@SampleID",SqlDbType.VarChar,50)
                };
                pars[0].Value = TableName;
                pars[1].Value = DialSchedTime;
                pars[2].Value = ActivityID;
                pars[3].Value = SampleID;
                var dbi = SupportClass.SqlHelper.RunSP("SP_Update_OSCTable", pars);
                if (dbi == 0)
                {
                    errCode = -1;
                }
            }
            catch (Exception ex)
            {
                errCode = -1;
                errMsg = ex.Message;
            }

            return Jsonp(new { errCode = errCode, errMsg = errMsg }, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}
