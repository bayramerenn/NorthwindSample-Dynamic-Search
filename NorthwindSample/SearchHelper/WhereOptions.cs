using System.ComponentModel;

namespace NorthwindSample.SearchHelper
{
    public class WhereOptions
    {
        public string Column { get; set; }
        public Operator Operator { get; set; }
        public string Value { get; set; }
        public SearchValueType SearchValueType { get; set; }
    }

    public enum Operator
    {
        [Description("Equal")]
        Equal,

        [Description("NotEqual")]
        NotEqual,

        [Description("GreaterThan")]
        GreaterThan,

        [Description("LessThan")]
        LessThan,

        [Description("Like")]
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
