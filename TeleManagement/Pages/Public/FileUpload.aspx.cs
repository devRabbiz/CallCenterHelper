using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;
using System.IO;
using System.Xml.Linq;
using Tele.Common;
using Tele.DataLibrary;
using SoftPhone.Business;
using SoftPhone.Entity;

namespace Tele.Management.Pages
{
    public partial class FileUpload : System.Web.UI.Page
    {

        private string documentID = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            documentID = Request.QueryString["docid"];
            if (!string.IsNullOrEmpty(documentID))
            {
                ViewState["docid"] = documentID;
            }
            if (this.IsPostBack)
            {
                if (string.IsNullOrEmpty(documentID) && ViewState["docid"] != null)
                {
                    documentID = ViewState["docid"].ToString();

                }
            }
        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            string fileName = file.PostedFile.FileName;
            if (string.IsNullOrEmpty(fileName)) return;
            FileInfo fi = new FileInfo(fileName);
            string[] exts = new string[] { ".xls", ".xlsx" };
            if (!exts.ToList().Exists(ext => ext == fi.Extension)) this.ltlError.Text = "请上传Excel文件。";
            else
            {
                //上传文件
                fileName = string.Format("{0}Documents\\TempFolder\\{1:ms}_{2}", this.Request.PhysicalApplicationPath, DateTime.Now, fi.Name);
                AsposeHelper.CheckPath(fileName);
                file.PostedFile.SaveAs(fileName);

                string templateFileName = AsposeHelper.GetTemplateFileName(documentID);
                DataTable dt = AsposeHelper.GetTemplateDataTable(documentID);
                BatchUploadResponse response = AsposeHelper.ExcelToDataTable(templateFileName, fileName, dt);
                if (response.IsVerified)
                    PersistData(response.Data);
                ShowResult(response.GetErrors());
            }
        }

        /// <summary>
        /// 下载模板
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnDownloadTemplate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(documentID))
            {
                throw new Exception("未传入参数：docid ！");
            }
            string templateFileName = AsposeHelper.GetTemplateFileName(documentID);
            if (!string.IsNullOrEmpty(templateFileName))
                AsposeHelper.DownloadFile(templateFileName);
        }

        #region 持久化数据

        private void PersistData(DataTable dt)
        {
            switch (dt.TableName)
            {
                case "IP和DN配置信息":
                    {
                        List<SPhone_IPDN> ipdns = Utils.GetListFromDataTable<SPhone_IPDN>(dt);
                        using (var db = DCHelper.SPhoneContext())
                        {
                            ipdns.ForEach(entity =>
                            {
                                entity.ID = Guid.NewGuid();
                                db.SPhone_IPDN.Add(entity);
                            });
                            db.SaveChanges();
                        }
                    }
                    break;
                case "值班信息":
                    {
                        List<SPhone_DutyManager> duties = Utils.GetListFromDataTable<SPhone_DutyManager>(dt);
                        using (var db = DCHelper.SPhoneContext())
                        {
                            duties.ForEach(entity =>
                            {
                                entity.DutyManagerID = Guid.NewGuid();
                                db.SPhone_DutyManager.Add(entity);
                            });
                            db.SaveChanges();
                        }
                    }
                    break;
                case "维修站信息":
                    {
                        List<ivr_sugg_station> list = Utils.GetListFromDataTable<ivr_sugg_station>(dt);
                        using (var db = DCHelper.SPhoneContext())
                        {
                            list.ForEach(entity =>
                            {
                                entity.AGENCY_ID = entity.AGENCY_ID.Trim();
                                entity.PRODUCT_TYPE = entity.PRODUCT_TYPE.Trim();
                                var info = db.ivr_sugg_station.Find(entity.AGENCY_ID, entity.PRODUCT_TYPE);
                                if (info == null)
                                {
                                    entity.AGENCY_NAME_WAV_FILENAME = entity.AGENCY_ID + "s.WAV";
                                    entity.AGENCY_WAV_FILENAME = entity.AGENCY_ID + ".WAV";
                                    entity.RECORD_DATE = DateTime.Now;
                                    entity.CATEGORY = "999";
                                    entity.VALID = "1";
                                    db.ivr_sugg_station.Add(entity);
                                }
                                else
                                {
                                    info.AGENCY_NAME = entity.AGENCY_NAME;
                                    info.AGENCY_COVER_AREA = entity.AGENCY_COVER_AREA;
                                    info.AGENCY_INFO = entity.AGENCY_INFO;
                                    info.ADD_TYPE = entity.ADD_TYPE;
                                    info.RECORD_DATE = DateTime.Now;
                                }
                            });
                            db.SaveChanges();
                        }
                    }
                    break;
                case "代理商信息":
                    {
                        List<ivr_sugg_agency> list = Utils.GetListFromDataTable<ivr_sugg_agency>(dt);
                        using (var db = DCHelper.SPhoneContext())
                        {
                            list.ForEach(entity =>
                            {
                                entity.AGENCY_ID = entity.AGENCY_ID.Trim();
                                entity.PRODUCT_TYPE = entity.PRODUCT_TYPE.Trim();
                                var info = db.ivr_sugg_agency.Find(entity.AGENCY_ID, entity.PRODUCT_TYPE);
                                if (info == null)
                                {
                                    entity.AGENCY_NAME_WAV_FILENAME = entity.AGENCY_ID + "s.WAV";
                                    entity.AGENCY_WAV_FILENAME = entity.AGENCY_ID + ".WAV";
                                    entity.RECORD_DATE = DateTime.Now;
                                    entity.CATEGORY = "999";
                                    entity.VALID = "1";
                                    db.ivr_sugg_agency.Add(entity);
                                }
                                else
                                {
                                    info.AGENCY_NAME = entity.AGENCY_NAME;
                                    info.AGENCY_COVER_AREA = entity.AGENCY_COVER_AREA;
                                    info.AGENCY_INFO = entity.AGENCY_INFO;
                                    info.ADD_TYPE = entity.ADD_TYPE;
                                    info.RECORD_DATE = DateTime.Now;
                                }
                            });
                            db.SaveChanges();
                        }
                    }
                    break;
            }
        }


        #endregion

        #region 私有方法

        private void ShowResult(List<string> htmls)
        {
            StringBuilder sb = new StringBuilder();

            if (htmls.Count > 0)
            {
                sb.Append("<br><span style='font-size:14px;color:red;'>数据上传失败，详细信息如下：</span>");
                foreach (string html in htmls)
                {
                    sb.Append(html);
                    sb.AppendLine();
                }
            }
            else
            {
                sb.Append("<br><span style='font-size:14px;color:blue;'>数据上传成功！</span>");
            }
            this.ltlError.Text = sb.ToString();
        }


        #endregion
    }
}
