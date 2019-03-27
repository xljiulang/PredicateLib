using System.Collections.Generic;

namespace System
{
    /// <summary>
    /// 提供查询条件扩展
    /// </summary>
    public static class ConditionExtensions
    {
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
                    yield return new KeyValuePair<string, string>(kv[0], kv[1]);
                }
            }
        }
    }
}
