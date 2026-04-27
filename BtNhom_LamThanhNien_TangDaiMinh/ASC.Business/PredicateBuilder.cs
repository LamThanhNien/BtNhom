using System.Linq.Expressions;

namespace ASC.Business
{
    public static class PredicateBuilder
    {
        public static Expression<Func<T, bool>> True<T>() => _ => true;

        public static Expression<Func<T, bool>> False<T>() => _ => false;

        public static Expression<Func<T, bool>> And<T>(
            this Expression<Func<T, bool>> first,
            Expression<Func<T, bool>> second)
        {
            return Compose(first, second, Expression.AndAlso);
        }

        public static Expression<Func<T, bool>> Or<T>(
            this Expression<Func<T, bool>> first,
            Expression<Func<T, bool>> second)
        {
            return Compose(first, second, Expression.OrElse);
        }

        private static Expression<Func<T, bool>> Compose<T>(
            Expression<Func<T, bool>> first,
            Expression<Func<T, bool>> second,
            Func<Expression, Expression, BinaryExpression> merge)
        {
            var parameter = Expression.Parameter(typeof(T));
            var left = ReplaceParameter(first.Body, first.Parameters[0], parameter);
            var right = ReplaceParameter(second.Body, second.Parameters[0], parameter);
            return Expression.Lambda<Func<T, bool>>(merge(left, right), parameter);
        }

        private static Expression ReplaceParameter(Expression body, ParameterExpression source, ParameterExpression target)
        {
            return new ReplaceExpressionVisitor(source, target).Visit(body);
        }

        private sealed class ReplaceExpressionVisitor : ExpressionVisitor
        {
            private readonly ParameterExpression _source;
            private readonly ParameterExpression _target;

            public ReplaceExpressionVisitor(ParameterExpression source, ParameterExpression target)
            {
                _source = source;
                _target = target;
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                return node == _source ? _target : base.VisitParameter(node);
            }
        }
    }
}
