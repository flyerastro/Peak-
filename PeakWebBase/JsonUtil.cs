using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web.Script.Serialization;

namespace PeakWebBase
{
    public static class JsonUtil
    {
        /// <summary>
        ///     对象转JSON
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns>JSON格式的字符串</returns>
        public static string ObjectToJson(this object obj)
        {
            var jss = new JavaScriptSerializer
            {
                MaxJsonLength = int.MaxValue,
                RecursionLimit = int.MaxValue
            };
            try
            {
                return jss.Serialize(obj);
            }
            catch (Exception ex)
            {
                throw new Exception("JSONHelper.ObjectToJSON(): " + ex.Message);
            }
        }

        /// <summary>
        ///     数据表转键值对集合 
        ///     把DataTable转成 List集合, 存每一行
        ///     集合中放的是键值对字典,存每一列
        /// </summary>
        /// <param name="dt">数据表</param>
        /// <returns>哈希表数组</returns>
        public static List<Dictionary<string, object>> DataTableToList(DataTable dt)
        {
            var list
                = new List<Dictionary<string, object>>();

            foreach (DataRow dr in dt.Rows)
            {
                var dic = new Dictionary<string, object>();
                foreach (DataColumn dc in dt.Columns)
                    dic.Add(dc.ColumnName, dr[dc.ColumnName]);
                list.Add(dic);
            }
            return list;
        }

        /// <summary>
        ///     数据集转键值对数组字典
        /// </summary>
        /// <param name="ds">数据集</param>
       /// <returns>键值对数组字典</returns>
        public static Dictionary<string, List<Dictionary<string, object>>> DataSetToDic(DataSet ds)
        {
            var result = new Dictionary<string, List<Dictionary<string, object>>>();

            foreach (DataTable dt in ds.Tables)
                result.Add(dt.TableName, DataTableToList(dt));

            return result;
        }

        /// <summary>
        ///     数据表转JSON
        /// </summary>
        /// <param name="dt">数据表</param>
        /// <returns>JSON字符串</returns>
        public static string DataTableToJson(DataTable dt)
        {
            return ObjectToJson(DataTableToList(dt));
        }

        /// <summary>
        ///     JSON文本转对象,泛型方法
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="jsonText">JSON文本</param>
        /// <returns>指定类型的对象</returns>
        public static T JsonToObject<T>(string jsonText)
        {
            var jss = new JavaScriptSerializer
            {
                MaxJsonLength = int.MaxValue,
                RecursionLimit = int.MaxValue
            };
            try
            {
                return jss.Deserialize<T>(jsonText);
            }
            catch (Exception ex)
            {
                throw new Exception("JSONHelper.JSONToObject(): " + ex.Message);
            }
        }

        /// <summary>
        ///     将JSON文本转换为数据表数据
        /// </summary>
        /// <param name="jsonText">JSON文本</param>
        /// <returns>数据表字典</returns>
        public static Dictionary<string, List<Dictionary<string, object>>> TablesDataFromJson(string jsonText)
        {
            return JsonToObject<Dictionary<string, List<Dictionary<string, object>>>>(jsonText);
        }

        /// <summary>
        ///     将JSON文本转换成数据行
        /// </summary>
        /// <param name="jsonText">JSON文本</param>
        /// <returns>数据行的字典</returns>
        public static Dictionary<string, object> DataRowFromJson(string jsonText)
        {
            return JsonToObject<Dictionary<string, object>>(jsonText);
        }


        /// <summary>
        ///     将接送对象字符串转化成对象集合
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="jsonText"></param>
        /// <returns></returns>
        public static List<TEntity> ListFromJson<TEntity>(string jsonText)
        {
            var modellist = new List<TEntity>();
            var json = new DataContractJsonSerializer(modellist.GetType());
            var _Using = Encoding.UTF8.GetBytes(jsonText);
            var memoryStream = new MemoryStream(_Using) {Position = 0};
            modellist = (List<TEntity>) json.ReadObject(memoryStream);
            return modellist;
        }
    }
}