using System;
using System.Collections.Generic;
using System.Text;

namespace Tele.Common
{
    public class JSONTree
    {
        public JSONTreeNode Root { get; private set; }

        public JSONTree(string id, string title)
        {
            this.Root = new JSONTreeNode(id, title, true);
        }

        public override string ToString()
        {
            return string.Format("[{0}]", this.Root.ToString());
        }

        public static string ToString(JSONTreeNode node)
        {
            return string.Format("[{0}]", node.ToString());
        }

        public static string ToString(List<JSONTreeNode> nodes)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            foreach (JSONTreeNode node in nodes)
            {
                sb.Append(node.ToString());
            }
            sb.Append("]");
            return sb.ToString();
        }
    }

    public class JSONTreeNode
    {
        #region Properties

        public string ID { get; set; }
        public string Title { get; set; }
        public bool HasChild { get; set; }
        public bool IsExpand { get; set; }
        public bool Loaded { get; set; }
        public bool ShowCheckBox { get; set; }
        public int CheckedSign { get; set; }
        public string Value { get; set; }
        public List<JSONTreeNode> Nodes { get; set; }

        private string ParentID { get; set; }
        private List<JSONTreeNode> TempNodes { get; set; }

        #endregion

        #region Constructors

        public JSONTreeNode(string id, string title)
            : this(id, title, false)
        { }

        public JSONTreeNode(string id, string title, bool isExpand)
            : this(id, title, isExpand, false)
        {
        }

        public JSONTreeNode(string id, string title, bool isExpand, bool showCheckBox)
        {
            this.ID = id;
            this.Title = title;
            this.HasChild = false;
            this.IsExpand = isExpand;
            this.ShowCheckBox = showCheckBox;
            this.Nodes = new List<JSONTreeNode>();
            this.TempNodes = new List<JSONTreeNode>();
        }

        #endregion

        #region Public Methods

        #region ToString

        public override string ToString()
        {
            ModifyTempNodes();

            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            sb.AppendFormat("!d!:!{0}!", this.ID);
            sb.AppendFormat(",!t!:!{0}!", this.Title);
            sb.AppendFormat(",!v!:!{0}!", this.Value ?? this.ID);
            if (this.ShowCheckBox)
            {
                if (this.CheckedSign > 0)
                    sb.AppendFormat(",!c!:true,!cs!:{0}", this.CheckedSign);
                else
                    sb.Append(",!c!:true");
            }
            if (this.HasChild) sb.Append(",!h!:true");
            if (this.IsExpand) sb.Append(",!isexpand!:true");
            if (this.Loaded) sb.Append(",!p!:true");
            if (this.Nodes.Count > 0)
            {
                sb.Append(",n:[");
                this.Nodes.ForEach(item => sb.Append(item.ToString()));
                sb.Append("]");
            }
            sb.Append("},");
            return sb.ToString();
        }

        #endregion

        #region Find

        public JSONTreeNode Find(string id)
        {
            return Find(this.Nodes, id);
        }

        public JSONTreeNode Find(List<JSONTreeNode> sourceList, string id)
        {
            JSONTreeNode result = null;
            result = sourceList.Find(n => n.ID == id);
            if (result == null)
            {
                foreach (JSONTreeNode child in sourceList)
                {
                    result = child.Find(id);
                    if (result != null) break;
                    else if (child.Nodes.Count > 0)
                    {
                        result = Find(child.Nodes, id);
                        if (result != null) break;
                    }
                }
            }
            return result;
        }
        #endregion

        #region AppendNode

        public void AppendNode(string id, string title)
        {
            AppendNode(id, title, false);
        }

        public void AppendNode(string id, string title, bool isExpand)
        {
            AppendNode(this.ID, id, title, isExpand);
        }

        public void AppendNode(string pid, string id, string title)
        {
            AppendNode(pid, id, title, false);
        }

        public void AppendNode(string pid, string id, string title, bool isExpand)
        {
            AppendNode(pid, id, title, isExpand, false, 0);
        }

        public void AppendNode(string pid, string id, string title, bool isExpand, bool showCheckBox, int checkedSign)
        {
            AppendNode(pid, id, title, id, isExpand, showCheckBox, checkedSign);
        }

        public void AppendNode(string pid, string id, string title, string value, bool isExpand, bool showCheckBox, int checkedSign)
        {
            JSONTreeNode node = this;
            if (node.ID != pid) node = this.Find(pid);
            if (node == null)
            {
                if (this.TempNodes.Find(n => n.ID == id) != null) return;
                // 若父节点为空，把此节点放入临时节点集合里
                node = new JSONTreeNode(id, title, isExpand, showCheckBox);
                if (showCheckBox) node.CheckedSign = checkedSign;
                node.ParentID = pid;
                node.Value = value;
                this.TempNodes.Add(node);
            }
            else
            {
                //若存在此ID的节点，更新节点信息
                JSONTreeNode child = this.Find(id);
                if (child == null)
                {
                    child = new JSONTreeNode(id, title, isExpand, showCheckBox);
                    if (showCheckBox && checkedSign > 0)
                    {
                        child.CheckedSign = checkedSign;
                    }
                    child.Value = value;
                    node.Nodes.Add(child);
                    this.TempNodes.RemoveAll(n => n.ID == id);
                }
                else
                {
                    child.Title = title;
                    child.IsExpand = isExpand;
                }
                node.HasChild = true;
                node.Loaded = true;
            }
        }
        #endregion

        #region ClearNodes

        public void ClearNodes()
        {
            this.Nodes.Clear();
        }

        public void ClearNodes(string pid)
        {
            JSONTreeNode node = this;
            if (node.ID != pid) node = this.Nodes.Find(n => n.ID == pid);
            if (node != null) node.Nodes.Clear();
        }

        #endregion

        #endregion

        #region Private Methods

        private void ModifyTempNodes()
        {
            int currentCount = this.TempNodes.Count;
            JSONTreeNode[] nodes = new JSONTreeNode[currentCount];
            this.TempNodes.CopyTo(nodes);
            foreach (JSONTreeNode item in nodes)
            {
                AppendNode(item.ParentID, item.ID, item.Title, item.IsExpand, item.ShowCheckBox, item.CheckedSign);
            }
            if (currentCount != this.TempNodes.Count)
            {
                ModifyTempNodes();
            }
        }

        #endregion
    }



}
