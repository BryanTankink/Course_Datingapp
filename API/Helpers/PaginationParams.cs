namespace API.Helpers
{
    public class PaginationParams
    {
        private const int MaxPageSize = 50;
        public int PageNumber { get; set; } = 1;
        private int _pageSize = 10;
        
        public int PageSize {
            get {
                return _pageSize;
            }
            set {
                if(value > MaxPageSize) _pageSize = MaxPageSize;
                else _pageSize = value;
            }
        }

    }
}