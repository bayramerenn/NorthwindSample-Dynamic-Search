namespace NorthwindSample.SearchHelper
{
    public class Search
    {
        public List<WhereOptions> Where { get; set; } = new List<WhereOptions>();
        public Paging Paging { get; set; } = new Paging();
    }
}