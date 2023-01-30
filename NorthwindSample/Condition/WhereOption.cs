using System.Collections;

namespace NorthwindSample.Condition
{
    public class WhereOption
    {
        public string Column { get; set; } = string.Empty;
        public OperatorCustom Operator { get; set; }
        public object Value { get; set; }
        public bool Not { get; set; }

        public void ValidateColumn()
        {
            if (string.IsNullOrEmpty(Column))
                throw new FilterException("ColumnNullError");
            if (Value == null && !(Operator == OperatorCustom.IsNull || Operator == OperatorCustom.IsNullOrEmpty))
                throw new FilterException("ValueNullError");
            if (Operator == OperatorCustom.IsBetween
                && !(Value is ICollection && ((ICollection)Value).Count == 2))
                throw new FilterException("BetweenValueError");
            if (Operator == OperatorCustom.In
                 && !(Value is ICollection && ((ICollection)Value).Count > 0))
                throw new FilterException("InValueError");
        }
    }
}