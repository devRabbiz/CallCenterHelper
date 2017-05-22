using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SoftPhone.Entity;
using Tele.Common;
using Tele.DataLibrary;

namespace SoftPhone.Business
{
    public class SPhone_UserPermissionBLL
    {

        public static Dictionary<string, int> GetSelectedIDs(Guid roleID)
        {
            Dictionary<string, int> result = new Dictionary<string, int>();
            List<SPhone_UserPermission> roleUsers = new List<SPhone_UserPermission>();
            using (var db = DCHelper.SPhoneContext())
            {
                roleUsers = db.SPhone_UserPermission.Where(item => item.RoleID == roleID).ToList();
            }
            roleUsers.ForEach(rm => result.Add(rm.EmployeeID.ToString(), 1));
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
        public static string GetTreeValue(string employeeID, Dictionary<string, int> selectedIDs)
        {
            List<string> allUsers = new List<string>();

            allUsers = GenesysBLL.Proc_GetCfgAdmin().Select(item => item.employee_id).ToList();

            if (!string.IsNullOrEmpty(employeeID))
                allUsers = allUsers.FindAll(id => id.IndexOf(employeeID, StringComparison.CurrentCultureIgnoreCase) != -1);
            string root = "所有用户";
            if (root == null) return string.Empty;

            JSONTree tree = new JSONTree(root, root);

            string parent = null;
            foreach (string eid in allUsers)
            {
                parent = root;
                int checkedSign = 0;
                if (selectedIDs.ContainsKey(eid)) checkedSign = selectedIDs[eid];
                tree.Root.AppendNode(parent, eid, eid, eid, true, true, checkedSign);
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
