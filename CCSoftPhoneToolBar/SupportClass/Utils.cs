using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;
using System.Text;
using System.Web.Mvc;

namespace SoftPhoneToolBar.SupportClass
{
    public class Utils
    {
        public static MvcHtmlString HightLine(string input, string key)
        {
            if (!string.IsNullOrEmpty(input) && !string.IsNullOrEmpty(key))
            {
                var newkey = new StringBuilder();
                foreach (var c in key.ToCharArray())
                {
                    newkey.Append("").Append(c);
                }
                var key2 = "(" + newkey.ToString() + ")";

                Regex r = new Regex(key2, RegexOptions.IgnoreCase);
                if (r.IsMatch(input))
                {
                    input = r.Replace(input, "<font color=red>$1</font>", 1);
                }
            }
            return new MvcHtmlString(input);
        }
    }
}