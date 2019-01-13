using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Peak
{
    public static class StringUtil
    {
        /// <summary>
        ///     获取字符串二进制编码
        /// </summary>
        /// <param name="str"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string BinaryCoding(this string str, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.Unicode;
            var s = encoding.GetBytes(str);
            var bs = "";
            foreach (var item in s)
                bs += Convert.ToString(item, 2);
            return bs;
        }

        /// <summary>
        ///     二进制编码解码
        /// </summary>
        /// <param name="str"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string BinaryDecoding(this string str, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.Unicode;
            var cs =
                Regex.Match(str, @"([01]{8})+").Groups[1].Captures;
            var data = new byte[cs.Count];
            for (var i = 0; i < cs.Count; i++)
                data[i] = Convert.ToByte(cs[i].Value, 2);
            return encoding.GetString(data, 0, data.Length);
        }


        /// <summary>
        ///     以自定义编码返回字符串所对应的字节数组
        /// </summary>
        /// <param name="data">字符串</param>
        /// <param name="encoding">编码</param>
        /// <returns>字节数组</returns>
        public static byte[] GetBytes(this string data, Encoding encoding = null)
        {
            encoding = encoding ?? Encoding.Unicode;
            return encoding.GetBytes(data);
        }

        /// <summary>
        ///     将字符串以Base64方式编码
        /// </summary>
        /// <param name="value">字符串</param>
        /// <returns>Base64编码后的字符串</returns>
        public static string EncodeBase64(this string value)
        {
            return value.EncodeBase64(Encoding.Unicode);
        }

        /// <summary>
        ///     将字符串以Base64方式编码
        /// </summary>
        /// <param name="value">字符串</param>
        /// <param name="encoding">编码方式</param>
        /// <returns>将Base64方式编码后的字符串</returns>
        public static string EncodeBase64(this string value, Encoding encoding)
        {
            encoding = encoding ?? Encoding.Unicode;
            var bytes = encoding.GetBytes(value);

            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        ///     将Base64方式编码后的字符串解码
        /// </summary>
        /// <param name="encodedValue">Base64方式编码后的字符串</param>
        /// <returns>解码后的字符串</returns>
        public static string DecodeBase64(this string encodedValue)
        {
            return encodedValue.DecodeBase64(Encoding.Unicode);
        }

        /// <summary>
        ///     将Base64方式编码后的字符串解码
        /// </summary>
        /// <param name="encodedValue">Base64方式编码后的字符串</param>
        /// <param name="encoding">编码方式</param>
        /// <returns>解码后的字符串</returns>
        public static string DecodeBase64(this string encodedValue, Encoding encoding)
        {
            encoding = encoding ?? Encoding.Unicode;
            var bytes = Convert.FromBase64String(encodedValue);

            return encoding.GetString(bytes);
        }
    }
}