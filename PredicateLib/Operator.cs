using System.Linq.Expressions;

namespace PredicateLib
{
    /// <summary>
    /// 比较操作符
    /// </summary>
    public enum Operator
    {
        /// <summary>
        /// 等于
        /// </summary>
        Equal = ExpressionType.Equal,

        /// <summary>
        /// 不等于
        /// </summary>
        NotEqual = ExpressionType.NotEqual,

        /// <summary>
        /// 大于或等于
        /// </summary>
        GreaterThanOrEqual = ExpressionType.GreaterThanOrEqual,

        /// <summary>
        /// 小于或等于
        /// </summary>
        LessThanOrEqual = ExpressionType.LessThanOrEqual,

        /// <summary>
        /// 大于
        /// </summary>
        GreaterThan = ExpressionType.GreaterThan,

        /// <summary>
        /// 小于
        /// </summary>
        LessThan = ExpressionType.LessThan,

        /// <summary>
        /// 包含
        /// like '%{value}%'
        /// </summary>
        Contains,

        /// <summary>
        /// 结束于
        /// like '%{value}'
        /// </summary>
        EndWith,

        /// <summary>
        /// 开始于
        /// like '{value}%'
        /// </summary>
        StartsWith
    }
}
