﻿namespace CrmPlatformAPI.Helpers
{
    public class CompanyParams
    {
        private const int MaxPageSize = 50;
        public int PageNumber { get; set; } = 1;
        private int pageSize = 3;
        public int PageSize
        {
            get => pageSize;
            set => pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }
        public string? CompanyName { get; set; }
        public string? OrderBy { get; set; }
    }

}
