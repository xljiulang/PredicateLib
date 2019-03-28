namespace System
{
    /// <summary>
    /// 类型扩展
    /// </summary>
    static class TypeExtensions
    {

#if !NETSTANDARD1_3
        /// <summary>
        /// 返回type的详细类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Type GetTypeInfo(this Type type)
        {
            return type;
        }

        /// <summary>
        /// 转换为Type类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Type AsType(this Type type)
        {
            return type;
        }
#endif

    }
}