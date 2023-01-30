namespace NorthwindSample.Condition
{
    public class FilterException : Exception
    {
        public FilterException() { }
        public FilterException(string message) : base(message) { }
    }
}
