namespace System
{
    /// <summary>
    /// 比较操作符
    /// </summary>
    public enum Operator
    {
        /// <summary>
        /// 等于
        /// </summary>
        Equal,

        /// <summary>
        /// 不等于
        /// </summary>
        NotEqual,

        /// <summary>
        /// 大于或等于
        /// </summary>
        GreaterThanOrEqual,

        /// <summary>
        /// 小于或等于
        /// </summary>
        LessThanOrEqual,

        /// <summary>
        /// 大于
        /// </summary>
        GreaterThan,

        /// <summary>
        /// 小于
        /// </summary>
        LessThan,

        /// <summary>
        /// 包含，like '%{value}%'
        /// </summary>
        Contains,

        /// <summary>
        /// 结束于，like '%{value}'
        /// </summary>
        EndWith,

        /// <summary>
        /// 开始于，like '{value}%'
        /// </summary>
        StartsWith
    }
}
