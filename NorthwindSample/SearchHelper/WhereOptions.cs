namespace NorthwindSample.SearchHelper
{
    public class WhereOptions
    {
        public string Column { get; set; } = string.Empty;
        public Operator Operator { get; set; }
        public string Value { get; set; } = string.Empty;
        public SearchValueType SearchValueType { get; set; }
    }

    public enum Operator
    {
        Equal,
        NotEqual,
        GreaterThan,
        LessThan,
        Like
    }

    public enum SearchValueType
    {
        String,
        Int,
        Double,
        DateTime,
        Bool
    }
}