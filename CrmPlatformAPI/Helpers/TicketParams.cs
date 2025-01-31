namespace CrmPlatformAPI.Helpers
{
    public class TicketParams
    {
        private const int MaxPageSize = 50;
        public int PageNumber { get; set; } = 1;
        private int pageSize = 2;
        public int PageSize
        {
            get { return pageSize; }
            set { pageSize = (value > MaxPageSize) ? MaxPageSize : value; }
        }

        public string? Username { get; set; } // For filtering by username
        public string? Status { get; set; } // Filter by ticket status (e.g., Open, Closed)
        public string? Priority { get; set; } // Filter by priority
        public string? Title { get; set; } // Search by ticket title
        public string? OrderBy { get; set; } // Sorting option (e.g., by date)

        public string SortDirection { get; set; } = "desc";

    }
}
