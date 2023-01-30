namespace NorthwindSample.Condition
{
    public class Search
    {
        public List<WhereOption> Where { get; set; } = new List<WhereOption>();
        public Paging Paging { get; set; } = new Paging();
    }
}