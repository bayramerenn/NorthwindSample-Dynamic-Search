using LinqKit;
using NorthwindSample.SearchHelper;
using System.Linq.Expressions;
using System.Reflection;

namespace NorthwindSample.Extensions
{
    public static class ExpressionHelper
    {
        public static Expression<Func<T, bool>> GetPredicateChain<T>(List<WhereOptions> whereOptions)
        {
            var predicateChain = PredicateBuilder.New<T>();
            Expression<Func<T, bool>> predicate;

            foreach (var item in whereOptions)
            {
                switch (item.Operator)
                {
                    case Operator.Equal:
                        predicate = GetExpressionEqual<T>(item.Column, item.Value, item.SearchValueType);
                        predicateChain.And(predicate);
                        break;

                    case Operator.NotEqual:
                        predicate = GetExpressionNotEqual<T>(item.Column, item.Value, item.SearchValueType);
                        predicateChain.And(predicate);
                        break;

                    case Operator.GreaterThan:
                        predicate = GetExpressionGreaterThan<T>(item.Column, item.Value, item.SearchValueType);
                        predicateChain.And(predicate);
                        break;

                    case Operator.LessThan:
                        predicate = GetExpressionLessThan<T>(item.Column, item.Value, item.SearchValueType);
                        predicateChain.And(predicate);
                        break;

                    case Operator.Like:
                        if (item.SearchValueType != SearchValueType.String)
                            throw new ArgumentException();

                        var like = GetExpressionContains<T>(item.Column, item.Value, item.SearchValueType);
                        predicateChain.And(like);
                        break;

                    default:
                        break;
                }
            }

            return predicateChain;
        }

        public static Expression<Func<T, bool>> GetExpressionContains<T>(string propertyName, string propertyValue, SearchValueType searchValueType)
        {
            var type = typeof(T);
            var parameter = Expression.Parameter(type, "x");

            var property = type.GetProperty(propertyName);

            CheckPropertyType(property!, searchValueType);

            var propertyExp = Expression.Property(parameter, propertyName);
            MethodInfo method = typeof(string).GetMethod("Contains", new[] { typeof(string) })!;
            var someValue = Expression.Constant(propertyValue, typeof(string));
            var containsMethodExp = Expression.Call(propertyExp, method, someValue);

            return Expression.Lambda<Func<T, bool>>(containsMethodExp, parameter);
        }

        public static Expression<Func<T, bool>> GetExpressionEqual<T>(string propertyName, string propertyValue, SearchValueType searchValueType)
        {
            var type = typeof(T);
            var parameter = Expression.Parameter(typeof(T), "x");
            var property = type.GetProperty(propertyName)!;

            CheckPropertyType(property, searchValueType);

            BinaryExpression binaryExpression;

            switch (searchValueType)
            {
                case SearchValueType.Int:
                    binaryExpression = Expression.Equal(Expression.MakeMemberAccess(parameter, property), Expression.Constant(Convert.ToInt32(propertyValue), GetPropertyType<int>(property.PropertyType)));
                    break;

                case SearchValueType.Double:
                    binaryExpression = Expression.Equal(Expression.MakeMemberAccess(parameter, property), Expression.Constant(Convert.ToDouble(propertyValue), GetPropertyType<double>(property.PropertyType)));
                    break;

                case SearchValueType.DateTime:
                    binaryExpression = Expression.Equal(Expression.MakeMemberAccess(parameter, property), Expression.Constant(Convert.ToDateTime(propertyValue), GetPropertyType<DateTime>(property.PropertyType)));
                    break;

                case SearchValueType.Bool:
                    binaryExpression = Expression.Equal(Expression.MakeMemberAccess(parameter, property), Expression.Constant(Convert.ToBoolean(propertyValue), GetPropertyType<bool>(property.PropertyType)));
                    break;

                case SearchValueType.String:
                    binaryExpression = Expression.Equal(Expression.MakeMemberAccess(parameter, property), Expression.Constant(propertyValue));
                    break;

                default:
                    throw new ArgumentException($"{searchValueType}");
            }

            return Expression.Lambda<Func<T, bool>>(binaryExpression, parameter);
        }

        public static Expression<Func<T, bool>> GetExpressionGreaterThan<T>(string propertyName, string propertyValue, SearchValueType searchValueType)
        {
            var type = typeof(T);
            var parameter = Expression.Parameter(typeof(T), "x");
            var property = type.GetProperty(propertyName)!;

            CheckPropertyType(property, searchValueType);

            BinaryExpression binaryExpression;

            switch (searchValueType)
            {
                case SearchValueType.Int:
                    binaryExpression = Expression.GreaterThan(Expression.MakeMemberAccess(parameter, property), Expression.Constant(Convert.ToInt32(propertyValue), GetPropertyType<int>(property.PropertyType)));
                    break;

                case SearchValueType.Double:
                    binaryExpression = Expression.GreaterThan(Expression.MakeMemberAccess(parameter, property), Expression.Constant(Convert.ToDouble(propertyValue), GetPropertyType<double>(property.PropertyType)));
                    break;

                case SearchValueType.DateTime:
                    binaryExpression = Expression.GreaterThan(Expression.MakeMemberAccess(parameter, property), Expression.Constant(Convert.ToDateTime(propertyValue), GetPropertyType<DateTime>(property.PropertyType)));
                    break;

                case SearchValueType.String:
                    binaryExpression = Expression.GreaterThan(Expression.MakeMemberAccess(parameter, property), Expression.Constant(propertyValue));
                    break;

                default:
                    throw new ArgumentException($"{searchValueType}");
            }

            return Expression.Lambda<Func<T, bool>>(binaryExpression, parameter);
        }

        public static Expression<Func<T, bool>> GetExpressionLessThan<T>(string propertyName, string propertyValue, SearchValueType searchValueType)
        {
            var type = typeof(T);
            var parameter = Expression.Parameter(typeof(T), "x");
            var property = type.GetProperty(propertyName)!;

            CheckPropertyType(property, searchValueType);

            BinaryExpression binaryExpression;

            switch (searchValueType)
            {
                case SearchValueType.Int:
                    binaryExpression = Expression.LessThan(Expression.MakeMemberAccess(parameter, property), Expression.Constant(Convert.ToInt32(propertyValue), GetPropertyType<int>(property.PropertyType)));
                    break;

                case SearchValueType.Double:
                    binaryExpression = Expression.LessThan(Expression.MakeMemberAccess(parameter, property), Expression.Constant(Convert.ToDouble(propertyValue), GetPropertyType<double>(property.PropertyType)));
                    break;

                case SearchValueType.DateTime:
                    binaryExpression = Expression.LessThan(Expression.MakeMemberAccess(parameter, property), Expression.Constant(Convert.ToDateTime(propertyValue), GetPropertyType<DateTime>(property.PropertyType)));
                    break;

                case SearchValueType.String:
                    binaryExpression = Expression.LessThan(Expression.MakeMemberAccess(parameter, property), Expression.Constant(propertyValue));
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
            var property = type.GetProperty(propertyName)!;

            CheckPropertyType(property, searchValueType);

            BinaryExpression binaryExpression;

            switch (searchValueType)
            {
                case SearchValueType.Int:
                    binaryExpression = Expression.NotEqual(Expression.MakeMemberAccess(parameter, property), Expression.Constant(Convert.ToInt32(propertyValue), GetPropertyType<int>(property.PropertyType)));
                    break;

                case SearchValueType.Double:
                    binaryExpression = Expression.NotEqual(Expression.MakeMemberAccess(parameter, property), Expression.Constant(Convert.ToDouble(propertyValue), GetPropertyType<double>(property.PropertyType)));
                    break;

                case SearchValueType.DateTime:
                    binaryExpression = Expression.NotEqual(Expression.MakeMemberAccess(parameter, property), Expression.Constant(Convert.ToDateTime(propertyValue), GetPropertyType<DateTime>(property.PropertyType)));
                    break;

                case SearchValueType.Bool:
                    binaryExpression = Expression.NotEqual(Expression.MakeMemberAccess(parameter, property), Expression.Constant(Convert.ToBoolean(propertyValue), GetPropertyType<bool>(property.PropertyType)));
                    break;

                case SearchValueType.String:
                    binaryExpression = Expression.NotEqual(Expression.MakeMemberAccess(parameter, property), Expression.Constant(propertyValue));
                    break;

                default:
                    throw new ArgumentException($"{searchValueType}");
            }

            return Expression.Lambda<Func<T, bool>>(binaryExpression, parameter);
        }

        public static Type GetPropertyType<T>(Type type) where T : struct
        {
            var checkProperyNull = Nullable.GetUnderlyingType(type);

            return checkProperyNull is null
                ? typeof(T)
                : typeof(T?);
        }

        public static void CheckPropertyType(PropertyInfo propertyInfo, SearchValueType searchValueType)
        {
            if (propertyInfo is null)
                throw new ArgumentException("The property of this class could not be found");

            if (!propertyInfo.PropertyType.FullName!.Contains(searchValueType.ToString()))
                throw new ArgumentException("The type sent in the request and the property type do not match");
        }
    }
}