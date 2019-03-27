using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace System
{
    /// <summary>
    /// 表示查询条件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Condition<T>
    {
        /// <summary>
        /// 类型的所有属性
        /// </summary>
        private readonly static PropertyInfo[] typeProperties = typeof(T).GetProperties();

        /// <summary>
        /// 表达式
        /// </summary>
        private readonly Expression<Func<T, bool>> predicate;

        /// <summary>
        /// 查询条件
        /// </summary>
        private readonly IDictionary<PropertyInfo, string> queryValues;

        /// <summary>
        /// 忽略配置
        /// </summary>
        private readonly List<MemberInfo> ignoreConfigs = new List<MemberInfo>();

        /// <summary>
        /// Operator配置
        /// </summary>
        private readonly Dictionary<MemberInfo, Operator> operatorConfigs = new Dictionary<MemberInfo, Operator>();

        /// <summary>
        /// 查询条件
        /// </summary>
        /// <param name="predicate">基础条件</param>
        /// <param name="queryValues">查询条件</param>
        /// <exception cref="ArgumentNullException"></exception>
        public Condition(Expression<Func<T, bool>> predicate, IEnumerable<KeyValuePair<string, string>> queryValues)
        {
            this.predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
            this.queryValues = this.CastToDictionary(queryValues);
        }

        /// <summary>
        /// 将查询条件转换为模型属性与值的字典
        /// </summary>
        /// <param name="queryValues"></param>
        /// <returns></returns>
        private Dictionary<PropertyInfo, string> CastToDictionary(IEnumerable<KeyValuePair<string, string>> queryValues)
        {
            var dic = new Dictionary<PropertyInfo, string>();
            if (queryValues != null)
            {
                foreach (var q in queryValues)
                {
                    var p = typeProperties.FirstOrDefault(item => item.Name.Equals(q.Key, StringComparison.OrdinalIgnoreCase));
                    if (p != null)
                    {
                        dic.Add(p, q.Value);
                    }
                }
            }
            return dic;
        }

        /// <summary>
        /// 配置忽略的条件
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="keySelector">键</param>
        /// <returns></returns>
        public Condition<T> IgnoreFor<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            var exp = keySelector.Body as MemberExpression;
            this.ignoreConfigs.Add(exp.Member);
            return this;
        }

        /// <summary>
        /// 配置比较操作符
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="keySelector">键</param>
        /// <param name="operator">操作符</param>
        /// <returns></returns>
        public Condition<T> OperatorFor<TKey>(Expression<Func<T, TKey>> keySelector, Operator @operator)
        {
            var exp = keySelector.Body as MemberExpression;
            this.operatorConfigs[exp.Member] = @operator;
            return this;
        }

        /// <summary>
        /// 转换为And连接的条件表达式
        /// </summary>
        /// <returns></returns>
        public Expression<Func<T, bool>> ToAndPredicate()
        {
            return this.ToPredicate(and: true);
        }

        /// <summary>
        /// 转换为Or连接的条件表达式
        /// </summary>
        /// <returns></returns>
        public Expression<Func<T, bool>> ToOrPredicate()
        {
            return this.ToPredicate(and: false);
        }

        /// <summary>
        /// 转换为And Or连接的条件表达式
        /// </summary>
        /// <param name="and"></param>
        /// <returns></returns>
        private Expression<Func<T, bool>> ToPredicate(bool and)
        {
            var exp = this.predicate;
            foreach (var kv in this.queryValues)
            {
                var member = kv.Key;
                if (this.ignoreConfigs.Contains(member) == false)
                {
                    var value = Convert(kv.Value, member.PropertyType);
                    var op = this.GetOperator(member);
                    var expRight = Predicate.Create<T>(member, value, op);
                    exp = and ? exp.And(expRight) : exp.Or(expRight);
                }
            }
            return exp;
        }

        /// <summary>
        /// 获取操作符
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        private Operator GetOperator(PropertyInfo member)
        {
            if (this.operatorConfigs.TryGetValue(member, out Operator @operator) == true)
            {
                return @operator;
            }
            return member.PropertyType == typeof(string) ? Operator.Contains : Operator.Equal;
        }


        /// <summary>
        /// 将value转换为目标类型
        /// </summary>
        /// <param name="value">要转换的值</param>
        /// <param name="targetType">转换的目标类型</param>
        /// <exception cref="NotSupportedException"></exception>
        /// <returns></returns>
        private static object Convert(string value, Type targetType)
        {
            if (value == null)
            {
                return null;
            }

            var underlyingType = Nullable.GetUnderlyingType(targetType);
            if (underlyingType != null)
            {
                targetType = underlyingType;
            }

            if (targetType.GetTypeInfo().IsEnum == true)
            {
                return Enum.Parse(targetType, value, true);
            }

            if (typeof(string) == targetType)
            {
                return value;
            }

            if (typeof(Guid) == targetType)
            {
                return Guid.Parse(value);
            }

            var convertible = value as IConvertible;
            if (convertible != null && typeof(IConvertible).IsAssignableFrom(targetType) == true)
            {
                return convertible.ToType(targetType, null);
            }

            throw new NotSupportedException();
        }
    }
}
