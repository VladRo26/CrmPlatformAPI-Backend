namespace CrmPlatformAPI.Helpers
{
    public class TicketContractsParams
    {
        private const int MaxPageSize = 50;

        public int PageNumber { get; set; } = 1;
        private int pageSize = 10;
        public int PageSize
        {
            get => pageSize;
            set => pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }

        public string? SortBy { get; set; }
        public string SortDirection { get; set; } = "desc";

        // Search filter by handler username
        public string? HandlerUsername { get; set; }
    }
}

