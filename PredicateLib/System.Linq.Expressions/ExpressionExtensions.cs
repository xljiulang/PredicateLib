using System.Collections.Generic;

namespace System.Linq.Expressions
{
    /// <summary>
    /// Where条件扩展
    /// 用于分步实现不确定条件个数的逻辑运算组织   
    /// </summary>
    public static class ExpressionExtensions
    {
        /// <summary>
        /// 与逻辑运算
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="expLeft">表达式1</param>
        /// <param name="expRight">表达式2</param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expLeft, Expression<Func<T, bool>> expRight)
        {
            var candidateExpr = Expression.Parameter(typeof(T), Predicate.ParamterName);
            var left = new ParameterReplacer(candidateExpr).Replace(expLeft.Body);
            var right = new ParameterReplacer(candidateExpr).Replace(expRight.Body);

            var body = Expression.AndAlso(left, right);
            return Expression.Lambda<Func<T, bool>>(body, candidateExpr);
        }

        /// <summary>
        /// 或逻辑运算
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="expLeft">表达式1</param>
        /// <param name="expRight">表达式2</param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> expLeft, Expression<Func<T, bool>> expRight)
        {
            var candidateExpr = Expression.Parameter(typeof(T), Predicate.ParamterName);
            var left = new ParameterReplacer(candidateExpr).Replace(expLeft.Body);
            var right = new ParameterReplacer(candidateExpr).Replace(expRight.Body);

            var body = Expression.OrElse(left, right);
            return Expression.Lambda<Func<T, bool>>(body, candidateExpr);
        }


        /// <summary>
        /// 与逻辑运算
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="expLeft">表达式1</param>
        /// <param name="fieldName">键2</param>
        /// <param name="value">值</param>
        /// <param name="op">操作符</param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expLeft, string fieldName, object value, Operator op)
        {
            var expRight = Predicate.Create<T>(fieldName, value, op);
            return expLeft.And(expRight);
        }

        /// <summary>
        /// 与逻辑运算
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <typeparam name="TKey">键类型</typeparam>
        /// <param name="expLeft">表达式1</param>
        /// <param name="keySelector">键</param>
        /// <param name="value">值</param>
        /// <param name="op">操作符</param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> And<T, TKey>(this Expression<Func<T, bool>> expLeft, Expression<Func<T, TKey>> keySelector, TKey value, Operator op)
        {
            var expRight = Predicate.Create(keySelector, value, op);
            return expLeft.And(expRight);
        }

        /// <summary>
        /// 与逻辑运算
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="expLeft">表达式1</param>
        /// <param name="keySelector">键选择</param>
        /// <param name="values">值</param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> AndIn<T, TKey>(this Expression<Func<T, bool>> expLeft, Expression<Func<T, TKey>> keySelector, IEnumerable<TKey> values)
        {
            var expRight = Predicate.CreateOrEqualPredicate(keySelector, values);
            return expLeft.And(expRight);
        }

        /// <summary>
        /// 与逻辑运算
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="expLeft">表达式1</param>
        /// <param name="keySelector">键选择</param>
        /// <param name="values">值</param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> AndNotIn<T, TKey>(this Expression<Func<T, bool>> expLeft, Expression<Func<T, TKey>> keySelector, IEnumerable<TKey> values)
        {
            var expRight = Predicate.CreateOrNotEqualPredicate(keySelector, values);
            return expLeft.And(expRight);
        }


        /// <summary>
        /// 表达式参数类型转换
        /// </summary>
        /// <typeparam name="TNew">新类型</typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public static Expression<Func<TNew, bool>> Cast<TNew>(this LambdaExpression expression)
        {
            var candidateExpr = Expression.Parameter(typeof(TNew), Predicate.ParamterName);
            var parameterReplacer = new ParameterReplacer(candidateExpr);

            var body = parameterReplacer.Replace(expression.Body);
            return Expression.Lambda<Func<TNew, bool>>(body, candidateExpr);
        }

        /// <summary>
        /// 参数替换对象
        /// </summary>
        private class ParameterReplacer : ExpressionVisitor
        {
            /// <summary>
            /// 表达式的参数
            /// </summary>
            public ParameterExpression ParameterExpression { get; private set; }

            /// <summary>
            /// 参数替换对象
            /// </summary>
            /// <param name="exp">表达式的参数</param>
            public ParameterReplacer(ParameterExpression exp)
            {
                this.ParameterExpression = exp;
            }

            /// <summary>
            /// 将表达式调度到此类中更专用的访问方法之一
            /// </summary>
            /// <param name="exp">表达式</param>
            /// <returns></returns>
            public Expression Replace(Expression exp)
            {
                return this.Visit(exp);
            }

            /// <summary>
            /// 获取表达式的参数
            /// </summary>
            /// <param name="p"></param>
            /// <returns></returns>
            protected override Expression VisitParameter(ParameterExpression p)
            {
                return this.ParameterExpression;
            }
        }
    }
}
