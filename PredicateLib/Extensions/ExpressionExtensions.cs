using PredicateLib;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace System
{
    /// <summary>
    /// 谓词筛选表达式扩展
    /// 用于分步实现不确定条件个数的谓词筛选表达式逻辑运算  
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
            if (expRight == null)
            {
                return expLeft;
            }
            var parameter = Expression.Parameter(typeof(T), Predicate.ParamterName);
            var left = new ParameterReplacer(parameter).Replace(expLeft.Body);
            var right = new ParameterReplacer(parameter).Replace(expRight.Body);

            var body = Expression.AndAlso(left, right);
            return Expression.Lambda<Func<T, bool>>(body, parameter);
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
            if (expRight == null)
            {
                return expLeft;
            }
            var parameter = Expression.Parameter(typeof(T), Predicate.ParamterName);
            var left = new ParameterReplacer(parameter).Replace(expLeft.Body);
            var right = new ParameterReplacer(parameter).Replace(expRight.Body);

            var body = Expression.OrElse(left, right);
            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }


        /// <summary>
        /// 与逻辑运算
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="expLeft">表达式1</param>
        /// <param name="propertyName">属性名称</param>
        /// <param name="value">属性值</param>
        /// <param name="operator">操作符</param>
        /// <exception cref="MissingFieldException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <returns></returns>
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expLeft, string propertyName, object value, Operator @operator)
        {
            var expRight = Predicate.Create<T>(propertyName, value, @operator);
            return expLeft.And(expRight);
        }

        /// <summary>
        /// 与逻辑运算
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <typeparam name="TKey">属性类型</typeparam>
        /// <param name="expLeft">表达式1</param>
        /// <param name="keySelector">属性选择</param>
        /// <param name="value">值</param>
        /// <param name="operator">操作符</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <returns></returns>
        public static Expression<Func<T, bool>> And<T, TKey>(this Expression<Func<T, bool>> expLeft, Expression<Func<T, TKey>> keySelector, TKey value, Operator @operator)
        {
            if (keySelector == null)
            {
                throw new ArgumentNullException(nameof(keySelector));
            }
            var expRight = Predicate.Create(keySelector, value, @operator);
            return expLeft.And(expRight);
        }

        /// <summary>
        /// 与逻辑运算
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="expLeft">表达式1</param>
        /// <param name="keySelector">属性选择</param>
        /// <param name="values">值</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static Expression<Func<T, bool>> AndIn<T, TKey>(this Expression<Func<T, bool>> expLeft, Expression<Func<T, TKey>> keySelector, IEnumerable<TKey> values)
        {
            if (keySelector == null)
            {
                throw new ArgumentNullException(nameof(keySelector));
            }
            if (values == null || values.Any() == false)
            {
                return expLeft;
            }
            var expRight = Predicate.CreateOrEqual(keySelector, values);
            return expLeft.And(expRight);
        }

        /// <summary>
        /// 与逻辑运算
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <typeparam name="TKey">属性类型</typeparam>
        /// <param name="expLeft">表达式1</param>
        /// <param name="keySelector">属性选择</param>
        /// <param name="values">值</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static Expression<Func<T, bool>> AndNotIn<T, TKey>(this Expression<Func<T, bool>> expLeft, Expression<Func<T, TKey>> keySelector, IEnumerable<TKey> values)
        {
            if (keySelector == null)
            {
                throw new ArgumentNullException(nameof(keySelector));
            }
            if (values == null || values.Any() == false)
            {
                return expLeft;
            }
            var expRight = Predicate.CreateAndNotEqual(keySelector, values);
            return expLeft.And(expRight);
        }

        /// <summary>
        /// 将表达式参数类型转换
        /// </summary>
        /// <typeparam name="TNew">新类型</typeparam>
        /// <param name="expression">表达式</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static Expression<Func<TNew, bool>> Cast<TNew>(this LambdaExpression expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }
            var parameter = Expression.Parameter(typeof(TNew), Predicate.ParamterName);
            var parameterReplacer = new ParameterReplacer(parameter);

            var body = parameterReplacer.Replace(expression.Body);
            return Expression.Lambda<Func<TNew, bool>>(body, parameter);
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
