using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SoftPhone.Entity;
using Tele.Common;
using Tele.DataLibrary;

namespace SoftPhone.Business
{
    public class SPhone_RoleBLL
    {

        public static Dictionary<string, int> GetSelectedIDs(Guid roleID)
        {
            Dictionary<string, int> result = new Dictionary<string, int>();
            List<SPhone_RoleModule> roleModules = new List<SPhone_RoleModule>();
            using (var db = DCHelper.SPhoneContext())
            {
                roleModules = db.SPhone_RoleModule.Where(item => item.RoleID == roleID).ToList();
            }
            roleModules.ForEach(rm => result.Add(rm.ModuleID.ToString(), 1));
            return result;
        }

        public static Dictionary<string, int> GetSelectedIDs(string ids)
        {
            Dictionary<string, int> dict = new Dictionary<string, int>();
            if (!string.IsNullOrEmpty(ids))
            {
                string[] array = ids.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                string id = string.Empty;
                int sign = 0;
                foreach (string item in array)
                {
                    id = TrimLast(item, "_1");
                    sign = 1;
                    if (id.Length == item.Length)
                    {
                        id = TrimLast(item, "_2");
                        sign = 2;
                    }
                    dict.Add(id, sign);
                }
            }
            return dict;
        }

        // 获取菜单树的值
        public static string GetTreeValue(Dictionary<string, int> selectedIDs)
        {
            List<SPhone_Module> modules = new List<SPhone_Module>();
            using (var db = DCHelper.SPhoneContext())
            {
                modules = db.SPhone_Module.ToList();
            }
            SPhone_Module root = new SPhone_Module() { ModuleID = Guid.Empty, ModuleName = "应用模块", ParentModuleID = Guid.Empty };
            if (root == null) return string.Empty;

            JSONTree tree = new JSONTree(root.ModuleID.ToString(), root.ModuleName);

            SPhone_Module parent = null;
            foreach (SPhone_Module item in modules.OrderBy(item => item.CreateTime))
            {
                parent = modules.Find(o => o.ModuleID == item.ParentModuleID);
                if (parent == null)
                    parent = root;
                int checkedSign = 0;
                string id = item.ModuleID.ToString();
                if (selectedIDs.ContainsKey(id)) checkedSign = selectedIDs[id];
                bool showCheckbox = !item.ParentModuleID.Equals(Guid.Empty);
                tree.Root.AppendNode(item.ParentModuleID.ToString(), id, item.ModuleName
                    , id, true, showCheckbox, checkedSign);
            }
            return tree.ToString();
        }


        #region Private Methods


        private static string TrimLast(string str1, string str2)
        {
            string result = str1;
            if (!string.IsNullOrEmpty(str2))
            {
                IEnumerable<char> chrs = str2.Reverse();
                foreach (char c in chrs)
                    result = result.TrimEnd(c);
            }
            return result;
        }

        #endregion


    }

}
