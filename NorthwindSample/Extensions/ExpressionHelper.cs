using NorthwindSample.SearchHelper;
using System.Linq.Expressions;
using System.Reflection;

namespace NorthwindSample.Extensions
{
    public static class ExpressionHelper
    {
        public static Expression<Func<T, bool>> GetExpressionContains<T>(string propertyName, string propertyValue)
        {
            var parameterExp = Expression.Parameter(typeof(T), "x");
            var propertyExp = Expression.Property(parameterExp, propertyName);
            MethodInfo method = typeof(string).GetMethod("Contains", new[] { typeof(string) });
            var someValue = Expression.Constant(propertyValue, typeof(string));
            var containsMethodExp = Expression.Call(propertyExp, method, someValue);

            return Expression.Lambda<Func<T, bool>>(containsMethodExp, parameterExp);
        }

        public static Expression<Func<T, bool>> GetExpressionEqual<T>(string propertyName, string propertyValue, SearchValueType searchValueType)
        {
            var type = typeof(T);
            var parameter = Expression.Parameter(typeof(T), "x");
            var property = type.GetProperty(propertyName);
            BinaryExpression binaryExpression;

            switch (searchValueType)
            {
                case SearchValueType.Int:
                    binaryExpression = Expression.Equal(Expression.MakeMemberAccess(parameter, property), Expression.Constant(Convert.ToInt32(propertyValue), CheckPropertyNullable(property.PropertyType)));
                    break;

                case SearchValueType.Double:
                    binaryExpression = Expression.Equal(Expression.MakeMemberAccess(parameter, property), Expression.Constant(Convert.ToDouble(propertyValue), CheckPropertyNullable(property.PropertyType)));
                    break;

                case SearchValueType.DateTime:
                    binaryExpression = Expression.Equal(Expression.MakeMemberAccess(parameter, property), Expression.Constant(Convert.ToDateTime(propertyValue), CheckPropertyNullable(property.PropertyType)));
                    break;

                case SearchValueType.Bool:
                    binaryExpression = Expression.Equal(Expression.MakeMemberAccess(parameter, property), Expression.Constant(Convert.ToBoolean(propertyValue), CheckPropertyNullable(property.PropertyType)));
                    break;

                case SearchValueType.String:
                    binaryExpression = Expression.Equal(Expression.MakeMemberAccess(parameter, property), Expression.Constant(propertyValue, CheckPropertyNullable(property.PropertyType)));
                    break;

                default:
                    throw new ArgumentException($"{searchValueType}");
            }

            return Expression.Lambda<Func<T, bool>>(binaryExpression, parameter);
        }

        public static Expression<Func<T, bool>> GetExpressionNotEqual<T>(string propertyName, string propertyValue, SearchValueType searchValueType)
        {
            var type = typeof(T);
            var parameter = Expression.Parameter(typeof(T), "x");
            var property = type.GetProperty(propertyName);

            if (property is null)
                throw new ArgumentException($"{propertyName} does not exist.");

            BinaryExpression binaryExpression;

            switch (searchValueType)
            {
                case SearchValueType.Int:
                    binaryExpression = Expression.NotEqual(Expression.MakeMemberAccess(parameter, property), Expression.Constant(Convert.ToInt32(propertyValue), CheckPropertyNullable(property.PropertyType)));
                    break;

                case SearchValueType.Double:
                    binaryExpression = Expression.NotEqual(Expression.MakeMemberAccess(parameter, property), Expression.Constant(Convert.ToDouble(propertyValue), CheckPropertyNullable(property.PropertyType)));
                    break;

                case SearchValueType.DateTime:
                    binaryExpression = Expression.NotEqual(Expression.MakeMemberAccess(parameter, property), Expression.Constant(Convert.ToDateTime(propertyValue), CheckPropertyNullable(property.PropertyType)));
                    break;

                case SearchValueType.Bool:
                    binaryExpression = Expression.NotEqual(Expression.MakeMemberAccess(parameter, property), Expression.Constant(Convert.ToBoolean(propertyValue), CheckPropertyNullable(property.PropertyType)));
                    break;

                case SearchValueType.String:
                    binaryExpression = Expression.NotEqual(Expression.MakeMemberAccess(parameter, property), Expression.Constant(propertyValue, CheckPropertyNullable(property.PropertyType)));
                    break;

                default:
                    throw new ArgumentException($"{searchValueType}");
            }

            return Expression.Lambda<Func<T, bool>>(binaryExpression, parameter);
        }

        public static Type CheckPropertyNullable(Type type)
        {
            var checkProperyNull = Nullable.GetUnderlyingType(type);
            return checkProperyNull is null
                ? typeof(int)
                : typeof(int?);
        }
    }
}