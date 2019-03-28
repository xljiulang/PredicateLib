using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace System
{
    /// <summary>
    /// 表示条件项
    /// </summary>
    public class ConditionItem
    {
        /// <summary>
        /// 获取或设置属性名称
        /// </summary>
        public string MemberName { get; set; }

        /// <summary>
        /// 获取或设置条件值
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// 获取或设置比较操作符
        /// </summary>
        public Operator? Operator { get; set; }

        /// <summary>
        /// 转换为泛型的ConditionItem
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public ConditionItem<T> AsGeneric<T>()
        {
            var member = ConditionItem<T>
                .TypeProperties
                .FirstOrDefault(item => item.Name.Equals(this.MemberName, StringComparison.OrdinalIgnoreCase));

            if (member == null)
            {
                return null;
            }
            return new ConditionItem<T>(member, this.Value, this.Operator);
        }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{this.MemberName} {this.Operator} {this.Value}";
        }
    }

    /// <summary>
    /// 表示条件项
    /// </summary>
    public class ConditionItem<T>
    {
        /// <summary>
        /// 获取T类型的所有属性
        /// </summary>
        public readonly static PropertyInfo[] TypeProperties = typeof(T).GetProperties();

        /// <summary>
        /// 获取属性
        /// </summary>
        public PropertyInfo Member { get; }

        /// <summary>
        /// 获取条件值
        /// </summary>
        public object Value { get; }

        /// <summary>
        /// 获取或设置比较操作符
        /// </summary>
        public Operator Operator { get; set; }

        /// <summary>
        /// 条件项
        /// </summary>
        /// <param name="member">属性</param>
        /// <param name="value">条件值</param>
        /// <param name="operator">比较操作符</param>
        /// <exception cref="ArgumentNullException"></exception>
        public ConditionItem(PropertyInfo member, object value, Operator? @operator)
        {
            this.Member = member ?? throw new ArgumentNullException(nameof(member));
            this.Value = ConvertToType(value, member.PropertyType);
            if (@operator == null)
            {
                this.Operator = member.PropertyType == typeof(string) ? Operator.Contains : Operator.Equal;
            }
            else
            {
                this.Operator = @operator.Value;
            }
        }

        /// <summary>
        /// 将value转换为目标类型
        /// </summary>
        /// <param name="value">要转换的值</param>
        /// <param name="targetType">转换的目标类型</param>
        /// <exception cref="NotSupportedException"></exception>
        /// <returns></returns>
        private static object ConvertToType(object value, Type targetType)
        {
            if (value == null)
            {
                return null;
            }

            if (value.GetType() == targetType)
            {
                return value;
            }

            var underlyingType = Nullable.GetUnderlyingType(targetType);
            if (underlyingType != null)
            {
                targetType = underlyingType;
            }

            if (value is IConvertible convertible)
            {
                return convertible.ToType(targetType, null);
            }

            var valueString = value.ToString();
            if (targetType.GetTypeInfo().IsEnum == true)
            {
                return Enum.Parse(targetType, valueString, true);
            }

            if (typeof(Guid) == targetType)
            {
                return Guid.Parse(valueString);
            }

            throw new NotSupportedException();
        }


        /// <summary>
        /// 转换为谓词筛选表达式
        /// </summary>
        /// <returns></returns>
        public Expression<Func<T, bool>> ToPredicate()
        {
            return Predicate.Create<T>(this.Member, this.Value, this.Operator);
        }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.ToPredicate().ToString();
        }
    }
}
