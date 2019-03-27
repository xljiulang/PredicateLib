using System.Collections.Generic;
using System.Text;

namespace System
{
    /// <summary>
    /// 提供查询条件扩展
    /// </summary>
    public static class ConditionExtensions
    {
        /// <summary>
        /// 默认的编码
        /// </summary>
        private static readonly Encoding defaultEncoding = Encoding.UTF8;

        /// <summary>
        /// 转换为T类型的查询条件 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uri">请求Uri，从query解析出查询条件</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static Condition<T> AsCondition<T>(this Uri uri)
        {
            return uri.GetQueryValues().AsCondition<T>();
        }

        /// <summary>
        /// 转换为T类型的查询条件 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyValues">查询条件</param>
        /// <returns></returns>
        public static Condition<T> AsCondition<T>(this IEnumerable<KeyValuePair<string, string>> keyValues)
        {
            return new Condition<T>(keyValues);
        }

        /// <summary>
        /// 获取Query参数值
        /// </summary>
        /// <param name="uri"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        private static IEnumerable<KeyValuePair<string, string>> GetQueryValues(this Uri uri)
        {
            if (uri == null)
            {
                throw new ArgumentNullException(nameof(uri));
            }

            if (string.IsNullOrEmpty(uri.Query))
            {
                yield break;
            }

            var query = uri.Query.TrimStart('?').Split('&');
            foreach (var q in query)
            {
                var kv = q.Split('=');
                if (kv.Length == 2)
                {
                    var key = HttpUtility.UrlDecode(kv[0], defaultEncoding);
                    var value = HttpUtility.UrlDecode(kv[1], defaultEncoding);
                    yield return new KeyValuePair<string, string>(key, value);
                }
            }
        }
    }
}
