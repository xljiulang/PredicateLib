using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace PredicateLib
{
    /// <summary>
    /// 表示查询条件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Condition<T>
    {
        /// <summary>
        /// 获取查询条件项
        /// </summary>
        public IList<ConditionItem<T>> Items { get; } = new List<ConditionItem<T>>();

        /// <summary>
        /// 查询条件
        /// </summary>
        /// <param name="conditionItems">查询条件项</param>
        public Condition(IEnumerable<KeyValuePair<string, string>> conditionItems)
            : this(GetConditionItems(conditionItems))
        {
        }

        /// <summary>
        /// 查询条件
        /// </summary>
        /// <param name="conditionItems">查询条件项</param>
        public Condition(IEnumerable<KeyValuePair<string, object>> conditionItems)
            : this(GetConditionItems(conditionItems))
        {
        }

        /// <summary>
        /// 查询条件
        /// </summary>
        /// <param name="conditionItems">查询条件项</param>
        /// <exception cref="NotSupportedException"></exception>
        public Condition(IEnumerable<ConditionItem> conditionItems)
            : this(conditionItems.Select(item => item.AsGeneric<T>()))
        {
        }

        /// <summary>
        /// 查询条件
        /// </summary>
        /// <param name="conditionItems">查询条件项</param>
        public Condition(IEnumerable<ConditionItem<T>> conditionItems)
        {
            if (conditionItems == null)
            {
                return;
            }

            foreach (var item in conditionItems)
            {
                if (item != null)
                {
                    this.Items.Add(item);
                }
            }
        }


        /// <summary>
        /// 转换条件值
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="keyValues">条件值</param>
        /// <returns></returns>
        private static IEnumerable<ConditionItem> GetConditionItems<TValue>(IEnumerable<KeyValuePair<string, TValue>> keyValues)
        {
            if (keyValues == null)
            {
                yield break;
            }

            foreach (var keyValue in keyValues)
            {
                yield return new ConditionItem
                {
                    MemberName = keyValue.Key,
                    Value = keyValue.Value
                };
            }
        }

        /// <summary>
        /// 配置忽略的条件
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="keySelector">属性选择</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public Condition<T> IgnoreFor<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            if (keySelector == null)
            {
                throw new ArgumentNullException(nameof(keySelector));
            }

            if (keySelector.Body.NodeType != ExpressionType.MemberAccess)
            {
                throw new ArgumentException("要求表达式主体必须为MemberAccess表达式", nameof(keySelector));
            }

            var exp = keySelector.Body as MemberExpression;
            var targets = this.Items.Where(item => item.Member == exp.Member).ToArray();
            foreach (var item in targets)
            {
                this.Items.Remove(item);
            }
            return this;
        }

        /// <summary>
        /// 配置比较操作符
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="keySelector">属性选择</param>
        /// <param name="operator">操作符</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public Condition<T> OperatorFor<TKey>(Expression<Func<T, TKey>> keySelector, Operator @operator)
        {
            if (keySelector == null)
            {
                throw new ArgumentNullException(nameof(keySelector));
            }

            if (keySelector.Body.NodeType != ExpressionType.MemberAccess)
            {
                throw new ArgumentException("要求表达式主体必须为MemberAccess表达式", nameof(keySelector));
            }

            var exp = keySelector.Body as MemberExpression;
            foreach (var item in this.Items)
            {
                if (item.Member == exp.Member)
                {
                    item.Operator = @operator;
                }
            }
            return this;
        }

        /// <summary>
        /// 转换为And连接的谓词筛选表达式
        /// </summary>
        /// <param name="trueWhenNull">当生成的表达式为null时返回true表达式</param>
        /// <returns></returns>
        public Expression<Func<T, bool>> ToAndPredicate(bool trueWhenNull = true)
        {
            if (this.Items.Count == 0)
            {
                return trueWhenNull ? Predicate.True<T>() : null;
            }

            return this.Items
                .Select(item => item.ToPredicate())
                .Aggregate((left, right) => left.And(right));
        }

        /// <summary>
        /// 转换为Or连接的谓词筛选表达式
        /// </summary>
        /// <param name="falseWhenNull">当生成的表达式为null时返回false表达式</param>
        /// <returns></returns>
        public Expression<Func<T, bool>> ToOrPredicate(bool falseWhenNull = true)
        {
            if (this.Items.Count == 0)
            {
                return falseWhenNull ? Predicate.False<T>() : null;
            }

            return this.Items
                .Select(item => item.ToPredicate())
                .Aggregate((left, right) => left.Or(right));
        }
    }
}
