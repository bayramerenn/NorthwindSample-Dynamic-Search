using NorthwindSample.Condition;
using System.Collections;
using System.Globalization;

namespace NorthwindSample.Extensions
{
    public static class ConditionExtension
    {
        public static WhereOption ConvertToValueType<TEntity>(this WhereOption condition)
        {
            // Valuekind dolayı eklenmiştir.
            condition.Value = condition.Value.ToString();

            if (condition == null) return condition;

            if (condition.Value is string && (condition.Operator == OperatorCustom.In || condition.Operator == OperatorCustom.IsBetween))
                condition.Value = condition.Value.ToString().Split(new string[] { ", " }, StringSplitOptions.None).ToArray();

            condition.ValidateColumn();
            var property = typeof(TEntity).GetProperty(condition.Column);
            if (property != null && condition.Value != null)
            {
                var propertyType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                if (condition.Value is ICollection)
                {
                    if (!typeof(ICollection<>).MakeGenericType(property.PropertyType).IsAssignableFrom(condition.Value.GetType()))
                    {
                        var array = ((ICollection)condition.Value).OfType<object>();
                        var listType = typeof(List<>).MakeGenericType(property.PropertyType);
                        var list = Activator.CreateInstance(listType, array.Count());
                        var method = listType.GetMethod("Add");
                        for (int i = 0; i < array.Count(); i++)
                            method.Invoke(list, new object[] { array.ElementAt(i) == null ? null : GetValue(array.ElementAt(i), propertyType) });
                        condition.Value = list;
                    }
                }
                else if (condition.Value.GetType() != propertyType)
                    condition.Value = GetValue(condition.Value, propertyType);
            }
            return condition;
        }

        private static object GetValue(object value, Type propertyType)
        {
            if (propertyType.IsEnum)
            {
                return Enum.Parse(propertyType, value.ToString());
            }
            else
            {
                return Convert.ChangeType(value.ToString(), propertyType, CultureInfo.InvariantCulture);
            }
        }
    }
}