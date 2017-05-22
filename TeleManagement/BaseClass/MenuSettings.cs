using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Web;
using System.Xml.Linq;
using System.Linq;

namespace Tele.Management
{
    public class MenuSettings
    {
        #region Constructors

        private static MenuSettings Instance = null;
        private MenuSettings() { }

        public static MenuSettings GetInstance()
        {
            if (MenuSettings.Instance == null)
                MenuSettings.Instance = new MenuSettings();
            return MenuSettings.Instance;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 获取匹配的链接
        /// &lt;a href="..." target="..." &gt;菜单名称&lt;/a&gt;
        /// </summary>
        /// <param name="urls">用户可访问的Url集合</param>
        /// <param name="addSeparator">是否添加分隔符--</param>
        /// <returns></returns>
        public List<string> GetMatchLinks(string moduleID, List<string> urls, ShowType type)
        {
            List<string> links = new List<string>();
            List<MenuGroup> groups = GetMenuGroups(moduleID);
            groups.ForEach(g => links.AddRange(g.GetGroupLinks(urls, type)));
            return links;
        }

        /// <summary>
        /// 获取菜单组集合
        /// </summary>
        /// <param name="moduleID">模块ID</param>
        /// <returns></returns>
        public static List<MenuGroup> GetMenuGroups(string moduleID)
        {
            List<MenuGroup> groups = new List<MenuGroup>();
            XElement module = LoadMenuElement(moduleID);
            foreach (XElement ele in module.Elements("group"))
            {
                MenuGroup group = new MenuGroup(ele);
                if (group.Links.Count > 0)
                    groups.Add(group);
            }
            return groups;
        }

        public static XElement LoadMenuElement(string moduleID)
        {
            string fileName = string.Format(@"{0}\Documents\Config\menu.xml", HttpContext.Current.Request.PhysicalApplicationPath);
            XElement result = null;
            try
            {
                XDocument doc = XDocument.Load(fileName, LoadOptions.None);
                result = doc.Elements("modules").Elements("module")
                    .Where(e => e.Attribute("id").Value == moduleID).FirstOrDefault();
            }
            catch { }
            return result;
        }


        #endregion

        #region Private Methods

        public static string GetValidValue(XAttribute att)
        {
            return GetValidValue(att, string.Empty);
        }

        public static string GetValidValue(XAttribute att, string defaultValue)
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

    /// <summary>
    /// 菜单组
    /// </summary>
    public class MenuGroup
    {
        #region Properties

        /// <summary>
        /// 组标识
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 组名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 目标窗体名称（向下派生）
        /// </summary>
        public string Target { get; set; }

        /// <summary>
        /// 链接集合
        /// </summary>
        public List<MenuLink> Links { get; set; }

        #endregion

        #region Constructors

        public MenuGroup(XElement element)
        {
            this.ID = MenuSettings.GetValidValue(element.Attribute("id"));
            this.Name = MenuSettings.GetValidValue(element.Attribute("name"));
            this.Target = MenuSettings.GetValidValue(element.Attribute("target"));
            this.Links = new List<MenuLink>();

            List<XElement> lst = element.Elements("link").ToList();
            foreach (XElement item in lst)
            {
                MenuLink link = new MenuLink(item);
                if (!string.IsNullOrEmpty(link.Name))
                    this.Links.Add(link);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// 获取组下的链接
        /// </summary>
        /// <returns></returns>
        public List<string> GetGroupLinks(List<string> urls, ShowType type)
        {
            List<string> links = new List<string>();
            this.Links.ForEach(l =>
            {
                if (l.IsVisible(urls))
                    links.Add(l.ToString(this.Target));
            });
            if (links.Count > 0)
                switch (type)
                {
                    case ShowType.Hr:
                        links.Insert(0, "<hr />");
                        break;
                    case ShowType.GroupName:
                        links.Insert(0, string.Format("<b>{0}</b>", this.Name));
                        break;
                }
            return links;
        }

        #endregion

    }

    /// <summary>
    /// 菜单项
    /// </summary>
    public class MenuLink
    {
        #region Properties

        /// <summary>
        /// 菜单标识
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 菜单名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 菜单链接
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 目标窗体名称（如未设置，自动从Group继承
        /// </summary>
        public string Target { get; set; }
        #endregion

        #region Constructors

        public MenuLink(XElement element)
        {
            this.ID = MenuSettings.GetValidValue(element.Attribute("id"));
            this.Name = MenuSettings.GetValidValue(element.Attribute("name"));
            this.Target = MenuSettings.GetValidValue(element.Attribute("target"));
            this.Url = MenuSettings.GetValidValue(element.Attribute("url"));
        }

        #endregion

        #region Methods

        /// <summary>
        /// 链接是否可见
        /// </summary>
        /// <param name="urls"></param>
        /// <returns></returns>
        public bool IsVisible(List<string> urls)
        {
            bool visible = urls.Exists(url => this.Url.IndexOf(url, StringComparison.CurrentCultureIgnoreCase) >= 0);
            return visible;
        }

        public override string ToString()
        {
            return this.ToString("Content");
        }

        /// <summary>
        /// 返回链接字符串
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        internal string ToString(string target)
        {
            string realUrl = this.Url;
            Uri uri = new Uri(this.Url, UriKind.RelativeOrAbsolute);
            if (!uri.IsAbsoluteUri && !string.IsNullOrEmpty(this.Url))
            {
                realUrl = string.Format("{0}{1}"
                    , HttpContext.Current.Request.Url.GetComponents(UriComponents.SchemeAndServer, UriFormat.SafeUnescaped)
                    , this.Url.TrimStart('~'));
            }
            return string.Format("<a id='{0}' href='{1}' target='{2}'>{3}</a>"
                , this.ID, realUrl, (string.IsNullOrEmpty(this.Target)) ? target : this.Target, this.Name);
        }

        #endregion
    }


    public enum ShowType
    {
        Hr = 1,
        GroupName = 2,
        None = 4,
    }

}
