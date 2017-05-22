using System;
using System.ComponentModel;
using Wuqi.Webdiyer;

namespace Tele.Management.Statics
{
    public partial class Pager : System.Web.UI.UserControl
    {
        /// <summary>
        /// 显示【首页】【尾页】
        /// </summary>
        public bool ShowFirstLast
        {
            get { return AspNetPager1.ShowFirstLast; }
            set { AspNetPager1.ShowFirstLast = value; }
        }

        /// <summary>
        /// 显示【上一页】【下一页】
        /// </summary>
        public bool ShowPrevNext
        {
            get { return AspNetPager1.ShowPrevNext; }
            set { AspNetPager1.ShowPrevNext = value; }
        }
        /// <summary>
        /// 显示 Disabled 的按钮
        /// </summary>
        public bool ShowDisabledButtons
        {
            get { return AspNetPager1.ShowDisabledButtons; }
            set { AspNetPager1.ShowDisabledButtons = value; }
        }
        /// <summary>
        /// 记录条数
        /// </summary>
        public int RecordCount
        {
            get { return AspNetPager1.RecordCount; }
            set { AspNetPager1.RecordCount = value; }
        }
        /// <summary>
        /// 页面大小（一页显示多少数据）
        /// </summary>
        public int PageSize
        {
            get { return AspNetPager1.PageSize; }
            set { AspNetPager1.PageSize = value; }
        }
        /// <summary>
        /// 当前页面索引
        /// </summary>
        public int CurrentPageIndex
        {
            get { return AspNetPager1.CurrentPageIndex; }
            set { AspNetPager1.CurrentPageIndex = value; }
        }

        [Browsable(true)]
        /// <summary>
        /// 翻页后，currentIndex还没变化.
        /// </summary>
        public event EventHandler PageChanged;
        [Browsable(true)]
        /// <summary>
        /// 翻页后，currentIndex已经变化.
        /// </summary>
        public event PageChangingEventHandler PageChanging;
        protected void Page_Load(object sender, EventArgs e)
        {
            AspNetPager1.PageChanged += PageChanged;
            AspNetPager1.PageChanging += PageChanging;
        }
    }
}