using PredicateLib;
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
        /// 转换为指定类型的查询条件 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uri">请求Uri，从query解析出查询条件</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static Condition<T> AsCondition<T>(this Uri uri)
        {
            return uri.AsCondition<T>(null);
        }

        /// <summary>
        /// 转换为指定类型的查询条件 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uri">请求Uri，从query解析出查询条件</param>
        /// <param name="encoding">编码</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static Condition<T> AsCondition<T>(this Uri uri, Encoding encoding)
        {
            return uri.GetQueryValues(encoding).AsCondition<T>();
        }

        /// <summary>
        /// 转换为指定类型的查询条件 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyValues">查询条件</param>
        /// <returns></returns>
        public static Condition<T> AsCondition<T>(this IEnumerable<KeyValuePair<string, string>> keyValues)
        {
            return new Condition<T>(keyValues);
        }

        /// <summary>
        /// 转换为指定类型的查询条件 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyValues">查询条件</param>
        /// <returns></returns>
        public static Condition<T> AsCondition<T>(this IEnumerable<KeyValuePair<string, object>> keyValues)
        {
            return new Condition<T>(keyValues);
        }

    }
}
