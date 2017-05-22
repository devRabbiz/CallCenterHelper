using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using Aspose.Cells;
using Tele.Common;

namespace Tele.Management
{
    public static class AsposeHelper
    {

        #region DataTableToExcel

        public static BatchDownloadResponse DataTableToExcel(string templateFileName, string fileName, DataTable dt)
        {
            if (!string.IsNullOrEmpty(templateFileName)) templateFileName = templateFileName.Replace("\\\\", "\\");
            if (!string.IsNullOrEmpty(fileName)) fileName = fileName.Replace("\\\\", "\\");
            BatchDownloadResponse response = new BatchDownloadResponse(fileName);
            if (dt == null)
            {
                response.AddError(0, "数据为空，请检查！");
                return response;
            }
            string licPath = System.Configuration.ConfigurationManager.AppSettings["AsposeLicPath"];
            License lic = new License();
            if (!string.IsNullOrEmpty(licPath)) lic.SetLicense(licPath);
            WorkbookDesigner wd = new WorkbookDesigner();
            wd.Open(templateFileName);
            wd.SetDataSource(dt);
            wd.Process();
            wd.Save(fileName, FileFormatType.Excel2007Xlsx);
            wd = null;
            FileInfo fi = new FileInfo(fileName);
            response.FileName = fi.Name;
            return response;
        }

        #endregion

        #region ExcelToDataTable

        /// <summary>
        /// Excel文件转换为DataTable.
        /// </summary>
        public static BatchUploadResponse ExcelToDataTable(string templateFileName, string fileName, DataTable dt)
        {
            BatchUploadResponse response = new BatchUploadResponse(fileName);
            if (!File.Exists(fileName))
            {
                response.AddError(0, "文件不存在！");
                return response;
            }

            Workbook workbook = new Workbook();
            // 打开模板
            workbook.Open(templateFileName);
            Worksheet worksheet = workbook.Worksheets[dt.TableName];
            if (worksheet == null)
            {
                response.AddError(0, "模板和配置文件不匹配，请联系管理员。");
                return response;
            }
            int startRow = worksheet.Cells.MaxDataRow;
            if (startRow > 0) startRow += 1;
            // 打开文件
            workbook.Open(fileName);
            worksheet = workbook.Worksheets[dt.TableName];
            if (worksheet == null)
            {
                response.AddError(0, "上传的文件和模板不匹配，请下载最新模板。");
                return response;
            }

            int maxRow = worksheet.Cells.MaxDataRow;
            int maxCol = worksheet.Cells.MaxDataColumn;
            //datatable = worksheet.Cells.ExportDataTable(0, 0, worksheet.Cells.MaxRow + 1, worksheet.Cells.MaxColumn + 1);

            for (int ii = startRow; ii <= maxRow; ii++)
            {
                DataRow dr = dt.NewRow();
                try
                {
                    for (int jj = 0; jj <= dt.Columns.Count - 1; jj++)
                    {
                        DataColumn col = dt.Columns[jj];
                        object value = worksheet.Cells[ii, jj].Value;
                        if (!string.IsNullOrEmpty(col.Namespace))
                            value = GetMappingValue(col.Namespace, value);
                        dr[jj] = Utils.DataConvert(col.DataType, value);
                    }
                    dt.Rows.Add(dr);
                }
                catch (Exception ex)
                {
                    response.AddError(ii, ex.Message);
                }
            }
            response.Data = dt;
            return response;
        }

        private static object GetMappingValue(string mappingInfo, object value)
        {
            string result = value.ToString();
            Dictionary<string, string> dicts = new Dictionary<string, string>();
            string[] items = mappingInfo.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string item in items)
            {
                string[] kv = item.Split(':');
                if (kv.Length <= 1) continue;
                dicts[kv[0].Trim()] = kv[1].Trim();
            }
            if (dicts.ContainsKey(result)) result = dicts[result];
            return result;
        }

        #endregion

        #region DownloadFile

        /// <summary>
        /// 下载文件
        /// </summary>
        public static void DownloadFile(string fileName)
        {
            if (!string.IsNullOrEmpty(fileName))
                fileName = fileName.Replace("\\\\", "\\").Replace("\\", "/");
            if (!File.Exists(fileName)) return;
            string outputFileName = fileName.Substring(fileName.LastIndexOf('/') + 1);
            byte[] binaryContent = System.IO.File.ReadAllBytes(fileName);

            HttpResponse response = HttpContext.Current.Response;
            response.Clear();
            response.ClearHeaders();
            response.Buffer = false;
            response.ContentType = "application/octet-stream";
            response.AddHeader("Content-Disposition", "attachment; filename=\""
                + HttpUtility.UrlEncode(outputFileName, System.Text.Encoding.UTF8) + "\"");
            response.AddHeader("Content-Length", binaryContent.Length.ToString());
            response.BinaryWrite(binaryContent);
            response.Flush();
            response.End();
        }
        #endregion

        #region CheckPath

        /// <summary>
        /// 检查路径
        /// </summary>
        /// <param name="path"></param>
        public static void CheckPath(string path)
        {
            if (!Path.IsPathRooted(path))
                path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "\\" + path;
            path = path.Replace('/', '\\').Replace("\\\\", "\\");
            if (path.Length > 7 && path.IndexOf(".", path.Length - 7) != -1)
                path = path.Substring(0, path.LastIndexOf('\\'));
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }


        #endregion


        #region TemplateXML

        // 获取模板文件路径
        public static string GetTemplateFileName(string documentID)
        {
            string fileName = string.Empty;
            XElement ele = LoadTemplateElement(documentID);
            if (ele != null)
            {
                fileName = GetValidValue(ele.Attribute("FilePath"));
                fileName = fileName.Replace("/", "\\").Replace("~", Tele.Management.Global.PhysicalApplicationPath);
            }
            return fileName;
        }

        // 获取模板文件的架构
        public static DataTable GetTemplateDataTable(string documentID)
        {
            DataTable dt = new DataTable();
            XElement doc = LoadTemplateElement(documentID);
            XElement sheet = doc.Element("WorkSheet");
            dt.TableName = GetValidValue(sheet.Attribute("SheetName"));

            List<XElement> lst = sheet.Elements("Column").ToList();
            foreach (XElement item in lst)
            {
                DataColumn col = new DataColumn();
                col.ColumnName = GetValidValue(item.Attribute("FieldName"));
                col.Caption = GetValidValue(item.Attribute("Name"), col.ColumnName);
                string typeName = GetValidValue(item.Attribute("Type"));
                if (!string.IsNullOrEmpty(typeName))
                    col.DataType = Type.GetType(typeName);
                col.Namespace = GetValidValue(item.Attribute("Mappings"));
                if (!string.IsNullOrEmpty(col.ColumnName))
                    dt.Columns.Add(col);
            }
            return dt;
        }

        public static XElement LoadTemplateElement(string documentID)
        {
            string fileName = string.Format(@"{0}\Documents\Config\Templates.xml", Tele.Management.Global.PhysicalApplicationPath);
            XElement result = null;
            try
            {
                XDocument doc = XDocument.Load(fileName, LoadOptions.None);
                result = doc.Elements("Documents").Elements("Document")
                    .Where(e => e.Attribute("ID").Value == documentID).FirstOrDefault();
            }
            catch { }
            return result;
        }

        #endregion

        #region Private Methods

        private static string GetValidValue(XAttribute att)
        {
            return GetValidValue(att, string.Empty);
        }

        private static string GetValidValue(XAttribute att, string defaultValue)
        {
            string reslut = string.Empty;
            if (att == null || string.IsNullOrEmpty(att.Value.Trim()))
            {
                reslut = defaultValue;
            }
            else
            {
                reslut = att.Value.Trim();
            }
            return reslut;
        }

        #endregion


    }


}