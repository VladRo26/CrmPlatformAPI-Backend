namespace CrmPlatformAPI.Helpers
{
    public class UserParams
    {
        private const int MaxPageSize = 50;
        public int PageNumber { get; set; } = 1;
        private int pageSize = 10;
        public int PageSize
        {
            get { return pageSize; }
            set { pageSize = (value > MaxPageSize) ? MaxPageSize : value; }
        }
        public string? CurrentUserName { get; set; }

        public string? CompanyName { get; set; }
        public string? UserType { get; set; }
        public int? Rating { get; set; } 
        public string? Name { get; set; }

        public string? OrderBy { get; set; }


    }
}
