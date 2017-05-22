using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using Tele.Common;
using System.Reflection;
using System.Web.UI;
using Aspose.Cells;
using Tele.DataLibrary;

namespace Tele.Management
{
    public class PageBase<T> : System.Web.UI.Page
        where T : class,new()
    {
        // 当前用户
        protected IUser UserInfo = null;
        // 菜单【菜单名，地址】
        protected Dictionary<string, string> MenuDict = null;

        #region Properties

        /// <summary>
        /// 保存主键键值的Key名称
        /// </summary>
        protected virtual string QueryStringID
        {
            get
            {
                return "id";
            }
        }

        /// <summary>
        /// 加载后的实体
        /// </summary>
        protected T Model { get; private set; }

        /// <summary>
        /// 主键
        /// </summary>
        protected virtual string PrimaryKey
        {
            get
            {
                return "ID";
            }
        }

        /// <summary>
        /// 主键值
        /// </summary>
        protected string PrimaryValue { get; set; }

        /// <summary>
        /// 新增时的主键值
        /// </summary>
        protected string NewPrimaryValue { get; set; }

        /// <summary>
        /// 页面是否只读
        /// </summary>
        protected bool IsReadOnly { get; set; }

        /// <summary>
        /// 提交后跳转到的页面链接
        /// </summary>
        protected string RedirectUrl { get; set; }

        /// <summary>
        /// 提交完执行的提示信息
        /// </summary>
        protected string AfterSubmitMessage { get; set; }

        /// <summary>
        /// 提交完执行的客户端脚本
        /// </summary>
        protected string AfterSubmitScript { get; set; }

        /// <summary>
        /// 是否超级管理员
        /// </summary>
        public bool IsSuperAdmin
        {
            get
            {
                bool result = false;
                if (this.UserInfo != null)
                    result = this.UserInfo.IsInRole("SUPER_ADMIN", "超级管理员");
                return result;
            }
        }

        // 检查页面访问权限
        protected bool HasPermission
        {
            get
            {
                bool enableViewPage = false;
                enableViewPage = this.IsSuperAdmin;
                if (!enableViewPage)
                {
                    string currentPageUrl = Request.Url.GetComponents(UriComponents.SchemeAndServer | UriComponents.Path, UriFormat.SafeUnescaped);
                    if (this.UserInfo != null)
                        enableViewPage = this.UserInfo.HasPagePermission(currentPageUrl);
                }
                return enableViewPage;
            }
        }

        #endregion

        #region Methods

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            // 用户信息
            this.UserInfo = AuthSession.GetCurrentUserInfo();
            // 用户权限
            if (!this.HasPermission)
                throw new Exception("拒绝访问，您没有页面的访问权限。");
            // 初始化页面数据
            this.InitPageData();
            Literal ltlNav = FindControlRecursive(this, "ltlCurrentPageType") as Literal;
            if (ltlNav != null)
                ltlNav.Text = string.IsNullOrEmpty(this.PrimaryValue) ? "新增" : "修改";
        }

        /// <summary>
        /// 提交数据
        /// </summary>
        /// <param name="data"></param>
        protected void SubmitData()
        {
            Dictionary<string, object> context = new Dictionary<string, object>();
            try
            {
                CollectInfo(context);
            }
            catch (System.ArgumentException) { }
            bool isCreate = string.IsNullOrEmpty(this.PrimaryValue);
            context[this.PrimaryKey] = isCreate ? this.NewPrimaryValue : this.PrimaryValue;
            T data = null;
            // 加载数据
            if (!isCreate && Model == null)
                Model = this.LoadInfo(this.PrimaryValue);
            data = Model;
            data = Utils.CreateInstanceFromDictionary<T>(data, context);
            List<string> errorMessages = new List<string>();
            CustomValidation(data, errorMessages);
            if (errorMessages != null && errorMessages.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                errorMessages.ForEach(msg => sb.AppendLine(msg));
                if (sb.Length > 0)
                {
                    //弹框
                    ShowClientMsg(this, sb.ToString());
                    return;
                }
            }
            this.PersistData(data, isCreate);
            AfterSubmit();
        }

        /// <summary>
        /// 生成查询条件对象
        /// </summary>
        /// <returns></returns>
        protected T GetSearcher()
        {
            Dictionary<string, object> context = new Dictionary<string, object>();
            try
            {
                CollectInfo(context);
            }
            catch (System.ArgumentException) { }
            T data = null;
            data = Utils.CreateInstanceFromDictionary<T>(data, context);
            return data;
        }

        /// <summary>
        /// 更新Entity
        /// </summary>
        protected void UpdateEntity(T entity1, T entity2)
        {
            Utils.UpdateEntity<T>(entity1, entity2);
        }

        #endregion

        #region Virtual Methods

        /// <summary>
        /// 根据主键ID加载信息
        /// </summary>
        /// <param name="id"></param>
        protected virtual T LoadInfo(string id)
        {
            return new T();
        }

        /// <summary>
        /// 根据主键ID加载信息
        /// </summary>
        /// <param name="id"></param>
        protected virtual void InitView(T model)
        {
        }

        /// <summary>
        /// 收集表单数据
        /// </summary>
        /// <param name="context"></param>
        protected virtual void CollectInfo(Dictionary<string, object> context)
        {
        }

        /// <summary>
        /// 自定义校验
        /// </summary>
        /// <param name="data">当前表单数据</param>
        /// <param name="errorMessages">错误信息列表</param>
        protected virtual void CustomValidation(T data, List<string> errorMessages)
        {
        }

        /// <summary>
        /// 持久化数据前对表单数据进行更改
        /// 操作对象 this.FormData
        /// </summary>
        /// <param name="wfFormData"></param>
        protected virtual void PersistData(T data, bool isCreate)
        {
        }

        /// <summary>
        /// 提交完信息后的事件
        /// </summary>
        protected virtual void AfterSubmit()
        {
            string msg = string.Empty;
            if (!string.IsNullOrEmpty(this.AfterSubmitMessage)) msg = this.AfterSubmitMessage;
            else
                msg = "数据保存成功，将返回列表页。";
            string script = this.AfterSubmitScript ?? string.Empty;
            if (!string.IsNullOrEmpty(script.TrimEnd()) && !script.TrimEnd().EndsWith(";")) script += ";";
            if (!string.IsNullOrEmpty(this.RedirectUrl))
            {
                this.RedirectUrl = this.RedirectUrl.ToLower().Replace("~", HttpContext.Current.Request.ApplicationPath.ToLower());
                script += string.Format("self.document.location.href='{0}';", this.RedirectUrl.Replace("//", "/"));
            }
            ShowClientMsg(this, msg, false, script);
        }

        /// <summary>
        /// 查询的数据提供者
        /// </summary>
        /// <param name="searcher"></param>
        /// <returns></returns>
        protected virtual void SearchProvider(T searcher, out IQueryable<T> query)
        {
            query = new List<T>().AsQueryable<T>();
        }

        #endregion

        #region Private Methods

        private void InitPageData()
        {
            string qsID = Request.QueryString[this.QueryStringID];
            string qsAction = Request.QueryString["act"];
            if (!string.IsNullOrEmpty(qsID))
            {
                this.PrimaryValue = qsID;
                if (!string.IsNullOrEmpty(qsAction) && qsAction.IndexOfAny(new char[] { 'r', 'v' }) != -1)
                    this.IsReadOnly = true;
            }
            if (!string.IsNullOrEmpty(this.PrimaryValue))
                Model = LoadInfo(this.PrimaryValue);
            InitView(Model);
        }

        private void SetReadOnly(ControlCollection ctrls)
        {
            WebControl wctrl = null;
            foreach (Control ctrl in ctrls)
            {
                if (ctrl is WebControl)
                {
                    wctrl = ctrl as WebControl;
                    string controlType = ctrl.GetType().Name.ToLower();
                    switch (controlType)
                    {
                        case "textbox":
                        case "dropdownlist":
                        case "checkbox":
                        case "radio":
                            wctrl.Enabled = false;
                            break;
                        case "button":
                            wctrl.Visible = false;
                            break;
                    }
                }
                if (ctrl.HasControls()) SetReadOnly(ctrl.Controls);
            }
        }

        /// <summary>
        /// 根据控件ID深度查找控件
        /// 尤其适用于模板嵌套页面
        /// </summary>
        private static Control FindControlRecursive(Control root, string id)
        {
            if (root.ID == id) return root;
            foreach (Control item in root.Controls)
            {
                Control ctrl = FindControlRecursive(item, id);
                if (ctrl != null) return ctrl;
            }
            return null;
        }

        #region ClientMessage

        public static void ShowClientMsg(Page page, string msg, params string[] parameters)
        {
            ShowClientMsg(page, msg, false, string.Empty, parameters);
        }

        public static void ShowClientMsg(Page page, string msg, bool needConfirm, string yesMethod, params string[] parameters)
        {
            if (parameters != null)
            {
                try
                {
                    msg = string.Format(msg, parameters);
                }
                catch { }
            }
            msg = msg.Replace('"', '\'').Replace("<", "\\<").Replace(">", "\\>");
            msg = msg.Replace("\r\n", "\\r\\n").Replace("\r", "\\r");
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("var msg=\"{0}\";\r\n", msg);
            if (!needConfirm) sb.AppendLine("alert(msg);" + yesMethod.TrimEnd(';') + ";");
            else
            {
                sb.AppendLine("if(confirm(msg)) {");
                sb.AppendLine(yesMethod.TrimEnd(';') + ";");
                sb.AppendLine("}");
            }
            page.ClientScript.RegisterStartupScript(page.GetType(), "", sb.ToString(), true);
        }

        #endregion

        #endregion

        private SPhoneEntities _dc;

        protected SPhoneEntities dc
        {
            get
            {
                if (_dc == null)
                {
                    _dc = DCHelper.SPhoneContext();
                }
                return _dc;
            }
        }
    }
}