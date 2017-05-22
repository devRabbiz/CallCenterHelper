using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Web.Script.Serialization;

namespace Tele.Common.Helper
{

    public class JsonHelper
    {
        public static string Serializer(object obj)
        {
            var j = new JavaScriptSerializer();
            return j.Serialize(obj);
        }

        public static T Deserialize<T>(string jsonString) where T : class
        {
            var j = new JavaScriptSerializer();
            return j.Deserialize<T>(jsonString);
        }
    }
}
