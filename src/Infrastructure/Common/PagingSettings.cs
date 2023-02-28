namespace Infrastructure.Common
{
    public abstract class PagingSettings
    {
        const int maxPageSize = 100;

        public int PageNumber { get; set; } = 1;

        private int _pageSize = 50;

        public int PageSize
        {
            get
            {
                return _pageSize;
            }
            set
            {
                _pageSize = (value > maxPageSize) ? maxPageSize : value;
            }
        }
    }
}