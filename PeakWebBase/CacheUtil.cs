using System;
using System.Web;
using System.Web.Caching;

namespace PeakWebBase
{
    public class CacheUtil
    {
        /// <summary>
        ///     设定绝对的过期时间（超过多少天后过期，单位是天）
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="objObject"></param>
        /// <param name="days">超过多少天后过期，单位是天</param>
        public static void SetCacheDateTime(string cacheKey, object objObject, long days)
        {
            var objCache = HttpRuntime.Cache;
            //objCache.Insert(CacheKey, objObject, null, DateTime.UtcNow.AddDays(days), TimeSpan.Zero);
            objCache.Insert(cacheKey, objObject, null, DateTime.Now.AddDays(days), Cache.NoSlidingExpiration);
        }

        /// <summary>
        /// </summary>
        /// <param name="cacheKey"></param>
        public static TEntity GetCacheDateTime<TEntity>(string cacheKey) where TEntity : class
        {
            var objCache = HttpRuntime.Cache;
            return objCache.Get(cacheKey) as TEntity;
        }

        /// <summary>
        ///     设置当前应用程序指定包含相对过期时间Cache值（超过多少天不调用就失效，单位是天）
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="objObject"></param>
        /// <param name="days">超过多少天不调用就失效，单位是天</param>
        public static void SetCacheTimeSpan(string cacheKey, object objObject, long days)
        {
            var objCache = HttpRuntime.Cache;
            objCache.Insert(cacheKey, objObject, null, DateTime.MaxValue, TimeSpan.FromDays(days));
        }
    }
}