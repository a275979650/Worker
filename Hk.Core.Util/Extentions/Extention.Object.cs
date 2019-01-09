using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace Hk.Core.Util.Extentions
{
    public static partial class Extention
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        static Extention()
        {
            JsonSerializerSettings setting = new JsonSerializerSettings();
            JsonConvert.DefaultSettings = new Func<JsonSerializerSettings>(() =>
            {
                //日期类型默认格式化处理
                setting.DateFormatHandling = DateFormatHandling.MicrosoftDateFormat;
                setting.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                return setting;
            });
        }
        public static bool Equal<T>(this T x, T y)
        {
            return ((IComparable)(x)).CompareTo(y) == 0;
        }
        #region 数值转换
        /// <summary>
        /// 转换为整型
        /// </summary>
        /// <param name="data">数据</param>
        public static int ToInt(this object data)
        {
            if (data == null)
                return 0;
            int result;
            var success = int.TryParse(data.ToString(), out result);
            if (success)
                return result;
            try
            {
                return Convert.ToInt32(ToDouble(data, 0));
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// 转换为可空整型
        /// </summary>
        /// <param name="data">数据</param>
        public static int? ToIntOrNull(this object data)
        {
            if (data == null)
                return null;
            int result;
            bool isValid = int.TryParse(data.ToString(), out result);
            if (isValid)
                return result;
            return null;
        }

        /// <summary>
        /// 转换为双精度浮点数
        /// </summary>
        /// <param name="data">数据</param>
        public static double ToDouble(this object data)
        {
            if (data == null)
                return 0;
            double result;
            return double.TryParse(data.ToString(), out result) ? result : 0;
        }

        /// <summary>
        /// 转换为双精度浮点数,并按指定的小数位4舍5入
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="digits">小数位数</param>
        public static double ToDouble(this object data, int digits)
        {
            return Math.Round(ToDouble(data), digits);
        }

        /// <summary>
        /// 转换为可空双精度浮点数
        /// </summary>
        /// <param name="data">数据</param>
        public static double? ToDoubleOrNull(this object data)
        {
            if (data == null)
                return null;
            double result;
            bool isValid = double.TryParse(data.ToString(), out result);
            if (isValid)
                return result;
            return null;
        }

        /// <summary>
        /// 转换为高精度浮点数
        /// </summary>
        /// <param name="data">数据</param>
        public static decimal ToDecimal(this object data)
        {
            if (data == null)
                return 0;
            decimal result;
            return decimal.TryParse(data.ToString(), out result) ? result : 0;
        }

        /// <summary>
        /// 转换为高精度浮点数,并按指定的小数位4舍5入
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="digits">小数位数</param>
        public static decimal ToDecimal(this object data, int digits)
        {
            return Math.Round(ToDecimal(data), digits);
        }

        /// <summary>
        /// 转换为可空高精度浮点数
        /// </summary>
        /// <param name="data">数据</param>
        public static decimal? ToDecimalOrNull(this object data)
        {
            if (data == null)
                return null;
            decimal result;
            bool isValid = decimal.TryParse(data.ToString(), out result);
            if (isValid)
                return result;
            return null;
        }

        /// <summary>
        /// 转换为可空高精度浮点数,并按指定的小数位4舍5入
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="digits">小数位数</param>
        public static decimal? ToDecimalOrNull(this object data, int digits)
        {
            var result = ToDecimalOrNull(data);
            if (result == null)
                return null;
            return Math.Round(result.Value, digits);
        }

        #endregion
        /// <summary>
        /// 将一个object对象序列化，返回一个byte[]         
        /// </summary> 
        /// <param name="obj">能序列化的对象</param>
        /// <returns></returns> 
        public static byte[] ToBytes(this object obj)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                return ms.GetBuffer();
            }
        }


        /// <summary>
        /// 判断是否为Null或者空
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this object obj)
        {
            if (obj == null)
                return true;
            else
            {
                string objStr = obj.ToString();
                return string.IsNullOrEmpty(objStr);
            }
        }

        /// <summary>
        /// 实体类转json数据，速度快
        /// </summary>
        /// <param name="t">实体类</param>
        /// <returns></returns>
        public static string EntityToJson(this object t)
        {
            if (t == null)
                return null;
            string jsonStr = "";
            jsonStr += "{";
            PropertyInfo[] infos = t.GetType().GetProperties();
            for (int i = 0; i < infos.Length; i++)
            {
                jsonStr = jsonStr + "\"" + infos[i].Name + "\":\"" + infos[i].GetValue(t).ToString() + "\"";
                if (i != infos.Length - 1)
                    jsonStr += ",";
            }
            jsonStr += "}";
            return jsonStr;
        }

        /// <summary>
        /// 深复制
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        public static T DeepClone<T>(this T obj) where T : class
        {
            if (obj == null)
                return null;

            return obj.ToJson().ToObject<T>();
        }

        /// <summary>
        /// 将对象序列化为XML字符串
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        public static string ToXmlStr<T>(this T obj)
        {
            var jsonStr = obj.ToJson();
            var xmlDoc = JsonConvert.DeserializeXmlNode(jsonStr);
            string xmlDocStr = xmlDoc.InnerXml;

            return xmlDocStr;
        }

        /// <summary>
        /// 将对象序列化为XML字符串
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="obj">对象</param>
        /// <param name="rootNodeName">根节点名(建议设为xml)</param>
        /// <returns></returns>
        public static string ToXmlStr<T>(this T obj, string rootNodeName)
        {
            var jsonStr = obj.ToJson();
            var xmlDoc = JsonConvert.DeserializeXmlNode(jsonStr, rootNodeName);
            string xmlDocStr = xmlDoc.InnerXml;

            return xmlDocStr;
        }

        /// <summary>
        ///     取得对象指定属性的值
        /// </summary>
        /// <param name="predicate">要取值的属性</param>
        /// <returns></returns>
        public static object GetPropertyValue<T, TProperty>(this T obj, Expression<Func<T, TProperty>> predicate)
        {
            var propertyName = predicate.GetPropertyName(); //属性名称

            return obj.GetPropertyValue(propertyName);
        }

        /// <summary>
        ///     取对象属性值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyName">支持“.”分隔的多级属性取值。</param>
        /// <returns></returns>
        public static object GetPropertyValue<T>(this T obj, string propertyName)
        {
            var strs = propertyName.Split('.');

            PropertyInfo property = null;
            object value = obj;

            for (var i = 0; i < strs.Length; i++)
            {
                property = value.GetType().GetProperty(strs[i]);
                value = property.GetValue(value, null);
            }
            return value;
        }

        /// <summary>
        ///     设置对象指定属性的值
        /// </summary>
        /// <param name="predicate">要设置值的属性</param>
        /// <param name="value">设置值</param>
        /// <returns>是否设置成功</returns>
        public static bool SetPropertyValue<T, TProperty>(this T obj, Expression<Func<T, TProperty>> predicate,
            object value)
        {
            var propertyName = predicate.GetPropertyName(); //属性名称

            return obj.SetPropertyValue(propertyName, value);
        }

        /// <summary>
        ///     设置对象属性值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyName">propertyName1.propertyName2.propertyName3</param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool SetPropertyValue<T>(this T obj, string propertyName, object value)
        {
            var strs = propertyName.Split('.');

            PropertyInfo property = null;
            object target = obj;

            for (var i = 0; i < strs.Length; i++)
            {
                property = target.GetType().GetProperty(strs[i]);
                if (i < strs.Length - 1)
                    target = property.GetValue(target, null);
            }

            var flag = false; //设置成功标记
            if (property != null && property.CanWrite)
            {
                if (false == property.PropertyType.IsGenericType) //非泛型
                {
                    if (property.PropertyType.IsEnum)
                    {
                        property.SetValue(target, Convert.ChangeType(value, typeof(int)));
                        flag = true;
                    }
                    else if (value.ToString() != property.PropertyType.ToString())
                    {
                        //property.SetValue(target, string.IsNullOrEmpty(value) ? null : Convert.ChangeType(value, property.PropertyType), null);
                        property.SetValue(target,
                            value == null ? null : Convert.ChangeType(value, property.PropertyType),
                            null);
                        flag = true;
                    }
                }
                else //泛型Nullable<>
                {
                    var genericTypeDefinition = property.PropertyType.GetGenericTypeDefinition();
                    if (genericTypeDefinition == typeof(Nullable<>))
                    {
                        //property.SetValue(target, string.IsNullOrEmpty(value) ? null : Convert.ChangeType(value, Nullable.GetUnderlyingType(property.PropertyType)), null);
                        property.SetValue(target,
                            value == null
                                ? null
                                : Convert.ChangeType(value, Nullable.GetUnderlyingType(property.PropertyType)),
                            null);
                        flag = true;
                    }
                }
            }

            return flag;
        }
        /// <summary>
        /// 将对象序列化成Json字符串
        /// </summary>
        /// <param name="obj">需要序列化的对象</param>
        /// <returns></returns>
        //public static string ToJson(this object obj)
        //{
        //    return JsonConvert.SerializeObject(obj);
        //}

        //public static object ToJson(this string Json)
        //{
        //    return Json == null ? null : JsonConvert.DeserializeObject(Json);
        //}
        public static string ToJson(this object obj)
        {
            var timeConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" };
            return JsonConvert.SerializeObject(obj, timeConverter);
        }
        public static string ToJson(this object obj, string datetimeformats)
        {
            var timeConverter = new IsoDateTimeConverter { DateTimeFormat = datetimeformats };
            return JsonConvert.SerializeObject(obj, timeConverter);
        }
        //public static T ToObject<T>(this string Json)
        //{
        //    return Json == null ? default(T) : JsonConvert.DeserializeObject<T>(Json);
        //}
        //public static List<T> ToList<T>(this string Json)
        //{
        //    return Json == null ? null : JsonConvert.DeserializeObject<List<T>>(Json);
        //}
        public static DataTable ToTable(this string Json)
        {
            return Json == null ? null : JsonConvert.DeserializeObject<DataTable>(Json);
        }
        public static JObject ToJObject(this string Json)
        {
            return Json == null ? JObject.Parse("{}") : JObject.Parse(Json.Replace("&nbsp;", ""));
        }
    }
}