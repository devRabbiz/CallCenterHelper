/*
 * Json序列化反序列化住手
 * author:zhangsl
 * data:2013.03.07
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Json;
using System.IO;

namespace VoiceOCX.Utils
{
    public class JsonHelper
    {
        public static string Serializer(object obj)
        {
            DataContractJsonSerializer s = new DataContractJsonSerializer(obj.GetType());
            using (MemoryStream ms = new MemoryStream())
            {
                s.WriteObject(ms, obj);
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }

        public static T Deserialize<T>(string jsonString) where T : class
        {
            DataContractJsonSerializer s = new DataContractJsonSerializer(typeof(T));
            var buffer = Encoding.UTF8.GetBytes(jsonString);
            using (MemoryStream ms = new MemoryStream(buffer))
            {
                return (T)s.ReadObject(ms);
            }
        }
    }
}
