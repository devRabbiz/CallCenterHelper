using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Tele.Common
{
    public class Utils
    {

        #region Added by Jeff

        #region 日期格式化

        /// <summary>
        /// 根据当前时间，返回时间段名称
        /// [凌晨][上午][下午][晚上]
        /// </summary>
        /// <returns></returns>
        public static string GetDateKey()
        {
            string key = "";
            int hour = DateTime.Now.Hour;
            if (hour < 6) key = "凌晨";
            else if (hour < 12) key = "上午";
            else if (hour < 18) key = "下午";
            else
                key = "晚上";
            return key;
        }

        #endregion

        #region 截取字符串

        /// <summary>
        /// 截取字符串
        /// </summary>
        /// <param name="inputString">字符串</param>
        /// <param name="len">最大长度</param>
        public static string SubstringPlus(string inputString, int len)
        {
            string addString = "...";
            return SubstringPlus(inputString, len, addString);
        }

        /// <summary>
        /// 截取字符串
        /// </summary>
        /// <param name="inputString">字符串</param>
        /// <param name="len">最大长度</param>
        /// <param name="addString">若截取过，在字符串后添加的字符串</param>
        public static string SubstringPlus(string inputString, int len, string addString)
        {
            string result = string.Empty;
            if (string.IsNullOrEmpty(inputString)) return result;
            result = inputString;
            int byteLength = Encoding.Default.GetByteCount(inputString);
            if (len > 0 && byteLength > len)
            {
                int length = 0;
                char[] chars = inputString.ToCharArray();
                for (int ii = len / 2; ii <= chars.Length - 1; ii++)
                {
                    length = Encoding.Default.GetByteCount(chars, 0, ii + 1);
                    if (length >= len)
                    {
                        result = inputString.Substring(0, ii);
                        break;
                    }
                }
            }
            if (result.Length < inputString.Length)
                result += addString;
            return result;
        }

        #endregion

        #region 数据类型转换

        /// <summary>
        /// 常用类型转换函数
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static object DataConvert(Type type, object value)
        {
            object result = null;
            if (type.IsGenericType)
                result = GenericDataConvert(type, value);
            else
                result = CommonDataConvert(type, value);
            return result;
        }


        /// <summary>
        /// 通用类型转换函数
        /// </summary>
        /// <param name="value">传入对象</param>
        /// <param name="defaultValue">默认值</param>
        public static T DataConvert<T>(object value, T defaultValue)
            where T : IConvertible
        {
            T result = defaultValue;
            if (value == null) value = string.Empty;
            if (!string.IsNullOrEmpty(value.ToString()))
            {
                Type type = typeof(T);
                if (result != null) type = result.GetType();
                try
                {
                    object cValue = Convert.ChangeType(value, type, System.Globalization.CultureInfo.CurrentCulture);
                    result = (T)cValue;
                }
                catch { }
            }
            return result;
        }

        /// <summary>
        /// 从字典实例化类
        /// </summary>
        public static T CreateInstanceFromDictionary<T>(T data, Dictionary<string, object> context)
            where T : class,new()
        {
            if (data == null) data = new T();
            PropertyInfo[] fields = data.GetType().GetProperties();
            foreach (PropertyInfo field in fields)
            {
                if (!context.Keys.Contains(field.Name) || !field.CanWrite) continue;
                object fieldValue = context[field.Name];
                object value = DataConvert(field.PropertyType, fieldValue);
                if (value != null)
                    field.SetValue(data, value, null);
            }
            return data;
        }


        /// <summary>
        /// 转换DataTable对象到指定的T类型对象
        /// </summary>
        /// <param name="dt">对象</param>
        /// <returns>T对象的列表</returns>
        public static List<T> GetListFromDataTable<T>(DataTable dt)
            where T : class,new()
        {
            List<T> result = new List<T>();
            if (dt == null || dt.Columns.Count == 0) return result;
            object objInfo = null;

            Type objType = typeof(T);
            PropertyInfo[] fields = objType.GetProperties();
            foreach (DataRow dr in dt.Rows)
            {
                objInfo = Activator.CreateInstance(objType);
                foreach (PropertyInfo field in fields)
                {
                    // 若Table里不包含指定字段，或T类型中的该字段不可写，则不转换
                    if (!dt.Columns.Contains(field.Name) || !field.CanWrite) continue;
                    object fieldValue = dr[field.Name];
                    object value = DataConvert(field.PropertyType, fieldValue);
                    if (value != null)
                        field.SetValue(objInfo, value, null);
                }
                result.Add((T)objInfo);
            }
            return result;
        }


        public static bool ToBooleanValue(string value)
        {
            return ToBooleanValue(value, false);
        }
        public static bool ToBooleanValue(string value, bool defaultValue)
        {
            bool result = defaultValue;
            if (string.IsNullOrEmpty(value)) return defaultValue;
            switch (value.Trim().ToUpper())
            {
                case "0":
                case "N":
                case "NO":
                case "NOT":
                case "FALSE":
                case "FAILURE":
                case "CANCEL":
                    result = false;
                    break;
                case "1":
                case "Y":
                case "YES":
                case "TRUE":
                case "SUCESS":
                case "OK":
                    result = true;
                    break;
                default:
                    result = defaultValue;
                    break;
            }
            return result;
        }

        #endregion

        #region 过滤


        /// <summary>
        /// 转换js中特殊字符
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static string CheckAlterMessage(string message)
        {
            message = message.Replace("'", "\\'");
            message = message.Replace("\"", "\\\"");
            message = message.Replace("<", "\\<");
            message = message.Replace(">", "\\>");
            message = message.Replace("&", "\\&");
            message = message.Replace("\r\n", "\\r\\n");
            message = message.Replace("\r", "\\r");
            return message;
        }


        #endregion

        #region 针对Entity的框架方法

        /// <summary>
        /// 比较两个泛型类是否相同
        /// 仅比较Keys中包含的属性
        /// 如果属性为字符串类型，自动右模糊查询
        /// </summary>
        public static bool IsMatch<T>(T data1, T data2, List<string> keys)
            where T : class,new()
        {
            if (data1 == null && data2 == null) return true;
            if (data1 == null || data2 == null) return false;

            bool match = true;
            PropertyInfo[] fields = data1.GetType().GetProperties();
            Dictionary<string, object> conditions = new Dictionary<string, object>();
            foreach (PropertyInfo field in fields)
            {
                if (!keys.Contains(field.Name) || !field.CanRead) continue;
                object fieldValue1 = field.GetValue(data1, null);
                object fieldValue2 = field.GetValue(data2, null);
                bool isDefaultValue = false;
                if (field.PropertyType.IsValueType)
                    isDefaultValue = (fieldValue2 == Activator.CreateInstance(field.PropertyType));
                isDefaultValue = (fieldValue1 == null || fieldValue2 == null);
                if (!isDefaultValue)
                {
                    string value1 = fieldValue1.ToString();
                    string value2 = fieldValue2.ToString();
                    if (field.PropertyType.Name == "String")
                        match &= (value1.IndexOf(value2, StringComparison.CurrentCultureIgnoreCase) != -1);
                    else
                        match &= (value1.Equals(value2, StringComparison.CurrentCultureIgnoreCase));
                }
                if (!match) break;
            }
            return match;
        }

        public static void UpdateEntity<T>(T data1, T data2)
            where T : class,new()
        {
            if (data1 == null || data2 == null) return;
            PropertyInfo[] fields = data1.GetType().GetProperties();
            foreach (PropertyInfo field in fields)
            {
                if (!field.CanWrite || field.PropertyType.IsAbstract || field.PropertyType.IsSecurityCritical) continue;
                object fieldValue = field.GetValue(data2, null);
                if (fieldValue != null)
                    field.SetValue(data1, fieldValue, null);
            }
        }

        /// <summary>
        /// 通用Entity更新函数
        /// 如果original参数中有些字段在data参数中不存在，请单独赋值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="original">需要更新的Entity，无需数据</param>
        /// <param name="data">包含数据的Entity</param>
        public static void UpdateEntity<T, V>(T original, V data)
            where T : class,new()
            where V : class,new()
        {
            if (original == null || data == null) return;
            PropertyInfo[] fields_ori = original.GetType().GetProperties();
            PropertyInfo[] fields_data = data.GetType().GetProperties();
            foreach (PropertyInfo field1 in fields_ori)
            {
                if (!field1.CanWrite || field1.PropertyType.IsAbstract || field1.PropertyType.IsSecurityCritical) continue;
                PropertyInfo field2 = fields_data.ToList().Find(item => item.Name == field1.Name);
                if (field2 == null) continue;
                object fieldValue = field2.GetValue(data, null);
                if (fieldValue != null)
                    field1.SetValue(original, fieldValue, null);
            }
        }
        #endregion

        #region 获取真正发生错误的错误对象

        /// <summary>
        /// 从Exception对象中，获取真正发生错误的错误对象。
        /// </summary>
        /// <param name="ex">Exception对象</param>
        /// <returns>真正发生错误的错误对象</returns>
        public static Exception GetRealException(Exception ex)
        {
            System.Exception lastestEx = ex;
            while (ex != null &&
                (ex is System.Web.HttpUnhandledException || ex is System.Web.HttpException || ex is TargetInvocationException))
            {
                if (ex.InnerException != null)
                    lastestEx = ex.InnerException;
                else
                    lastestEx = ex;

                ex = ex.InnerException;
            }
            return lastestEx;
        }

        #endregion

        #region Private Methods


        /// <summary>
        /// 常用类型转换函数
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static object CommonDataConvert(Type type, object value)
        {
            object result = null;
            string typeName = type.Name;
            try
            {
                switch (typeName)
                {
                    case "Int16":
                        result = DataConvert<short>(value, default(short));
                        break;
                    case "Int32":
                        result = DataConvert<int>(value, default(int));
                        break;
                    case "Int64":
                        result = DataConvert<long>(value, default(long));
                        break;
                    case "String":
                        result = DataConvert<string>(value, string.Empty);
                        break;
                    case "DateTime":
                        result = DataConvert<DateTime>(value, default(DateTime));
                        break;
                    case "Guid":
                        Guid gValue = default(Guid);
                        if (!Guid.TryParse(value.ToString(), out gValue))
                            gValue = default(Guid);
                        result = gValue;
                        break;
                    default:
                        result = value;
                        break;
                }
            }
            catch { }
            return result;
        }

        /// <summary>
        /// 泛型类型转换
        /// </summary>
        /// <param name="field"></param>
        /// <param name="fieldValue"></param>
        /// <returns></returns>
        private static object GenericDataConvert(Type type, object value)
        {
            object result = null;
            if (value != null)
            {
                if (type == typeof(Nullable<short>))
                    result = DataConvert<short>(value, default(short));
                else if (type == typeof(Nullable<int>))
                    result = DataConvert<int>(value, default(int));
                else if (type == typeof(Nullable<long>))
                    result = DataConvert<long>(value, default(long));
                else if (type == typeof(Nullable<DateTime>))
                    result = DataConvert<DateTime>(value, default(DateTime));
                else if (type == typeof(Nullable<Guid>))
                {
                    Guid gValue = default(Guid);
                    if (!Guid.TryParse(value.ToString(), out gValue))
                        gValue = default(Guid);
                    result = gValue;
                }
            }
            return result;
        }


        #endregion

        #endregion

        #region
        public static string NoHtml(string strHtml)
        {
            if (strHtml != "")
            {
                strHtml = Regex.Replace(strHtml, @"<\/?[^>]*>", "", RegexOptions.IgnoreCase);
                strHtml = strHtml.Replace("&nbsp;", " ");
                strHtml = strHtml.Replace("\n", " ");
                strHtml = strHtml.Replace("\r", " ");
                strHtml = Regex.Replace(strHtml, "( )+", " ");
            }
            return strHtml;
        }

        /// <summary>
        /// 方法：按长度取字符，len为长度
        /// </summary>
        public static string CutStr(string inputString, int len)
        {
            #region
            System.Text.ASCIIEncoding ascii = new System.Text.ASCIIEncoding();
            int tempLen = 0;
            string tempString = "";
            byte[] s = ascii.GetBytes(inputString);
            for (int i = 0; i < s.Length; i++)
            {
                if ((int)s[i] == 63)
                {
                    tempLen += 2;
                }
                else
                {
                    tempLen += 1;
                }

                try
                {
                    tempString += inputString.Substring(i, 1);
                }
                catch
                {
                    break;
                }

                if (tempLen >= len)
                    break;
            }
            //如果截过则加上..
            byte[] mybyte = System.Text.Encoding.Default.GetBytes(inputString);
            if (mybyte.Length > len)
                tempString += "..";
            return tempString;
            #endregion
        }
        #endregion

        public static string SHA1Bytes(byte[] buffer)
        {
            SHA1 sha = SHA1.Create();
            buffer = sha.ComputeHash(buffer);
            sha.Clear();
            return BitConverter.ToString(buffer).Replace("-", "");
        }

        public static string SHA1Stream(Stream stream)
        {
            int len = (int)stream.Length;
            byte[] buffer = new byte[len];
            stream.Read(buffer, 0, len);

            SHA1 sha = SHA1.Create();
            buffer = sha.ComputeHash(buffer, 0, len);
            return BitConverter.ToString(buffer).Replace("-", "");
        }
    }
}
