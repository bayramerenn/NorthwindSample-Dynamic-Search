using LinqKit;
using NorthwindSample.Condition;
using System.Collections;
using System.Linq.Expressions;
using System.Reflection;

namespace NorthwindSample.Extensions
{
    public static partial class ConditionExpresssionExtension
    {
        public static Expression<Func<TEntity, bool>> GetLambdaExpression<TEntity>(this IList<WhereOption> conditions)
        {
            var predicateChain = PredicateBuilder.New<TEntity>(true);
            WhereOption condition;

            foreach (WhereOption item in conditions)
            {
                if (item == null) return null;

                var parameterExpression = Expression.Parameter(typeof(TEntity));

                condition = item.ConvertToValueType<TEntity>();

                var expression = GetExpression(parameterExpression, condition);

                if (expression is not null)
                {
                    var entityExpression = GetLambda<TEntity>(expression, parameterExpression);
                    predicateChain.And(entityExpression);
                }
            }
            return predicateChain;
        }

        public static Expression<Func<TEntity, bool>> GetLambdaExpression<TEntity>(this WhereOption condition)
        {
            if (condition == null) return null;

            var parameterExpression = Expression.Parameter(typeof(TEntity));

            condition = condition.ConvertToValueType<TEntity>();

            var expression = GetExpression(parameterExpression, condition);

            return expression == null ? null : GetLambda<TEntity>(expression, parameterExpression);
        }

        private static Expression GetExpression<TEntity>(this WhereOption condition)
        {
            if (condition == null) return null;

            var parameterExpression = Expression.Parameter(typeof(TEntity));

            condition = condition.ConvertToValueType<TEntity>();

            return GetExpression(parameterExpression, condition);
        }

        private static Expression GetExpression(ParameterExpression parameterExpression, WhereOption condition)
        {
            Expression expression = Expression.Default(typeof(object));

            SetExpression(ref expression, parameterExpression, condition);

            return expression;
        }

        private static void SetExpression(ref Expression expression, ParameterExpression parameterExpression, WhereOption condition)
        {
            if (condition.Operator == OperatorCustom.Equals)
                expression = Expression.Equal(Expression.PropertyOrField(parameterExpression, condition.Column), Expression.Constant(condition.Value, Expression.PropertyOrField(parameterExpression, condition.Column).Type));
            else if (condition.Operator == OperatorCustom.NotEquals)
                expression = Expression.NotEqual(Expression.PropertyOrField(parameterExpression, condition.Column), Expression.Constant(condition.Value, Expression.PropertyOrField(parameterExpression, condition.Column).Type));
            else if (condition.Operator == OperatorCustom.IsGreaterThan) SetComparableExpression(ref expression, parameterExpression, condition, ExpressionType.GreaterThan);
            else if (condition.Operator == OperatorCustom.IsGreaterThanOrEqualto) SetComparableExpression(ref expression, parameterExpression, condition, ExpressionType.GreaterThanOrEqual);
            else if (condition.Operator == OperatorCustom.IsLessThan) SetComparableExpression(ref expression, parameterExpression, condition, ExpressionType.LessThan);
            else if (condition.Operator == OperatorCustom.IsLessThanOrEqualto) SetComparableExpression(ref expression, parameterExpression, condition, ExpressionType.LessThanOrEqual);
            else if (condition.Operator == OperatorCustom.IsBetween) SetBetweenExpression(ref expression, parameterExpression, condition);
            else if (condition.Operator == OperatorCustom.Contains) SetStringExpression(ref expression, parameterExpression, condition, nameof(string.Contains));
            else if (condition.Operator == OperatorCustom.StartsWith) SetStringExpression(ref expression, parameterExpression, condition, nameof(string.StartsWith));
            else if (condition.Operator == OperatorCustom.EndsWith) SetStringExpression(ref expression, parameterExpression, condition, nameof(string.EndsWith));
            else if (condition.Operator == OperatorCustom.IsNull) expression = Expression.Equal(Expression.PropertyOrField(parameterExpression, condition.Column), Expression.Constant(null));
            else if (condition.Operator == OperatorCustom.IsNullOrEmpty)
            {
                expression = Expression.Call(
                        typeof(string).GetMethod(nameof(string.IsNullOrEmpty), new Type[] { typeof(string) }),
                        Expression.PropertyOrField(parameterExpression, condition.Column));
            }
            else if (condition.Operator == OperatorCustom.In)
            {
                MemberExpression memberExpression = Expression.PropertyOrField(parameterExpression, condition.Column);

                expression = Expression.Call(
                     typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public)
                     .Single(m => m.Name == nameof(Enumerable.Contains)
                                          && m.GetParameters().Count() == 2)
                     .MakeGenericMethod(memberExpression.Type)
                     , new Expression[]
                     {
                             Expression.Constant(condition.Value),
                             memberExpression
                     });
            }

            if (condition.Not)
                expression = Expression.Not(expression);
        }

        private static void SetComparableExpression(ref Expression expression, ParameterExpression parameterExpression, WhereOption condition, ExpressionType expressionType)
        {
            //typeof(IComparable).IsAssignableFrom(condition.Value.GetType())
            if (condition.Value is IComparable)
            {
                if (condition.Value.GetType().Equals(typeof(string)))
                {
                    expression = Expression.Call(
                    Expression.PropertyOrField(parameterExpression, condition.Column),
                    condition.Value.GetType().GetMethod(nameof(IComparable.CompareTo), new[] { typeof(object) }),
                    Expression.Constant(condition.Value));
                    expression = Expression.MakeBinary(expressionType, expression, Expression.Constant(0));
                }
                else
                    expression = Expression.MakeBinary(expressionType,
                    Expression.PropertyOrField(parameterExpression, condition.Column),
                    Expression.Constant(condition.Value, Expression.PropertyOrField(parameterExpression, condition.Column).Type));
            }
            else
                throw new FilterException(message: $"ImplementError { condition.Value.GetType() } {nameof(IComparable)}");
        }

        private static void SetBetweenExpression(ref Expression expression, ParameterExpression parameterExpression, WhereOption condition)
        {
            var array = ((ICollection)condition.Value).OfType<object>();
            var startValue = Expression.Constant(array.ElementAt(0), Expression.PropertyOrField(parameterExpression, condition.Column).Type);
            var endValue = Expression.Constant(array.ElementAt(1), Expression.PropertyOrField(parameterExpression, condition.Column).Type);

            //typeof(IComparable).IsAssignableFrom(array.ElementAt(0).GetType())
            if (array.ElementAt(0) is IComparable)
            {
                if (condition.Value is ICollection<string>)
                {
                    var start = Expression.Call(
                    Expression.PropertyOrField(parameterExpression, condition.Column),
                    typeof(string).GetMethod(nameof(string.CompareTo), new[] { typeof(string) }),
                    startValue);
                    var end = Expression.Call(
                    Expression.PropertyOrField(parameterExpression, condition.Column),
                    typeof(string).GetMethod(nameof(string.CompareTo), new[] { typeof(string) }),
                    endValue);
                    expression = Expression.AndAlso(Expression.GreaterThanOrEqual(start, Expression.Constant(0)),
                                                Expression.LessThanOrEqual(end, Expression.Constant(0)));
                }
                else
                    expression = Expression.AndAlso(
                        Expression.GreaterThanOrEqual(Expression.PropertyOrField(parameterExpression, condition.Column), startValue),
                        Expression.LessThanOrEqual(Expression.PropertyOrField(parameterExpression, condition.Column), endValue)
                        );
            }
            else
                throw new FilterException(message: $"ImplementError {condition.Value.GetType()} {nameof(IComparable)}");
        }

        private static void SetStringExpression(ref Expression expression, ParameterExpression parameterExpression, WhereOption condition, string methodName)
        {
            expression = Expression.Call(Expression.PropertyOrField(parameterExpression, condition.Column),
                         typeof(string).GetMethod(methodName, new[] { typeof(string) }),
                         Expression.Constant(condition.Value));
        }

        private static Expression<Func<TEntity, bool>> GetLambda<TEntity>(Expression expression, ParameterExpression parameterExpression)
       => (Expression<Func<TEntity, bool>>)Expression.Lambda(expression, parameterExpression);
    }
}