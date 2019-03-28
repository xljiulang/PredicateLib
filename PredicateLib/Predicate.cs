using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace PredicateLib
{
    /// <summary>
    /// 提供谓词筛选表达式的生成
    /// </summary>
    public static class Predicate
    {
        /// <summary>
        /// Enumerable的Contains方法
        /// </summary>
        private static readonly MethodInfo containsMethod;

        /// <summary>
        /// 获取表示式参数名
        /// </summary>
        public static readonly string ParamterName = "item";

        /// <summary>
        /// 静态构造器
        /// </summary>
        static Predicate()
        {
            var q = from m in typeof(Enumerable).GetMethods()
                    where m.Name == "Contains" && m.IsGenericMethod
                    let parameters = m.GetParameters()
                    where parameters.Length == 2
                    let pLast = parameters.Last()
                    where pLast.ParameterType.GetTypeInfo().IsGenericType == false
                    select m;

            containsMethod = q.Single();
        }


        /// <summary>
        /// 返回默认为True的谓词筛选表达式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Expression<Func<T, bool>> True<T>()
        {
            return item => true;
        }

        /// <summary>
        /// 返回默认为False的谓词筛选表达式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Expression<Func<T, bool>> False<T>()
        {
            return item => false;
        }

        /// <summary>
        /// 将数组转换为Or的相等的谓词筛选表达式
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <typeparam name="TKey">属性类型</typeparam>
        /// <param name="keySelector">属性选择</param>
        /// <param name="values">包含的值</param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> CreateOrEqualPredicate<T, TKey>(Expression<Func<T, TKey>> keySelector, IEnumerable<TKey> values)
        {
            var p = keySelector.Parameters.Single();
            var equals = values.Select(value => (Expression)Expression.Equal(keySelector.Body, Expression.Constant(value, typeof(TKey))));
            var body = equals.Aggregate((accumulate, equal) => Expression.OrElse(accumulate, equal));
            return Expression.Lambda<Func<T, bool>>(body, p);
        }


        /// <summary>
        /// 将数组转换为Or的不等的谓词筛选表达式
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <typeparam name="TKey">>属性类型</typeparam>
        /// <param name="keySelector">属性选择</param>
        /// <param name="values">包含的值</param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> CreateOrNotEqualPredicate<T, TKey>(Expression<Func<T, TKey>> keySelector, IEnumerable<TKey> values)
        {
            var p = keySelector.Parameters.Single();
            var equals = values.Select(value => (Expression)Expression.NotEqual(keySelector.Body, Expression.Constant(value, typeof(TKey))));
            var body = equals.Aggregate((accumulate, equal) => Expression.AndAlso(accumulate, equal));
            return Expression.Lambda<Func<T, bool>>(body, p);
        }

        /// <summary>
        /// 生成In操作的谓词筛选表达式
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <typeparam name="TKey">属性类型</typeparam>
        /// <param name="keySelector">属性选择</param>
        /// <param name="values">包含的值</param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> CreateContainsPredicate<T, TKey>(Expression<Func<T, TKey>> keySelector, IEnumerable<TKey> values)
        {
            var method = containsMethod.MakeGenericMethod(typeof(TKey));
            var callBody = Expression.Call(null, method, Expression.Constant(values, typeof(IEnumerable<TKey>)), keySelector.Body);
            var paramExp = keySelector.Parameters.Single();
            return Expression.Lambda(callBody, paramExp) as Expression<Func<T, bool>>;
        }

        /// <summary>
        /// 根据属性名生成谓词筛选表达式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName">属性名</param>
        /// <param name="value">值</param>
        /// <param name="op">操作符</param>
        /// <exception cref="MissingFieldException"></exception>
        /// <returns></returns>
        public static Expression<Func<T, bool>> Create<T>(string propertyName, object value, Operator op)
        {
            var member = typeof(T).GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
            if (member == null)
            {
                throw new MissingFieldException(propertyName);
            }
            return Create<T>(member, value, op);
        }

        /// <summary>
        /// 根据属性生成谓词筛选表达式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="member">属性成员</param>
        /// <param name="value">值</param>
        /// <param name="op">操作符</param>
        /// <exception cref="MissingFieldException"></exception>
        /// <returns></returns>
        public static Expression<Func<T, bool>> Create<T>(MemberInfo member, object value, Operator op)
        {
            var paramExp = Expression.Parameter(typeof(T), ParamterName);
            var memberExp = Expression.MakeMemberAccess(paramExp, member);
            return Create<T>(paramExp, memberExp, value, op);
        }

        /// <summary>
        /// 根据属性生成谓词筛选表达式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="keySelector">属性选择</param>
        /// <param name="value">值</param>
        /// <param name="op">操作符</param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> Create<T, TKey>(Expression<Func<T, TKey>> keySelector, TKey value, Operator op)
        {
            return Create<T>(keySelector.Parameters.First(), (MemberExpression)keySelector.Body, value, op);
        }

        /// <summary>
        /// 生成表达式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="paramExp">参数表达式</param>
        /// <param name="memberExp">成员表达式</param>
        /// <param name="value">属性值</param>
        /// <param name="op">操作符</param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> Create<T>(ParameterExpression paramExp, MemberExpression memberExp, object value, Operator op)
        {
            switch (op)
            {
                case Operator.Contains:
                case Operator.EndWith:
                case Operator.StartsWith:
                    var method = typeof(string).GetMethod(op.ToString(), new Type[] { typeof(string) });
                    var callBody = Expression.Call(memberExp, method, Expression.Constant(value, typeof(string)));
                    return Expression.Lambda(callBody, paramExp) as Expression<Func<T, bool>>;

                default:
                    var valueType = (memberExp.Member as PropertyInfo).PropertyType;
                    var valueExp = Expression.Constant(value, valueType);
                    var expMethod = typeof(Expression).GetMethod(op.ToString(), new Type[] { typeof(Expression), typeof(Expression) });

                    var symbolBody = expMethod.Invoke(null, new object[] { memberExp, valueExp }) as Expression;
                    return Expression.Lambda(symbolBody, paramExp) as Expression<Func<T, bool>>;
            }
        }
    }
}
