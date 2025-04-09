namespace Captive.Model.Response
{
    public class PaginatedResponse <T> where T : class
    {
        public IEnumerable<T> Data { get; set; }
        public int TotalRecords { get; set; }
    }
}
