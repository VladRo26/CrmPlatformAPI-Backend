﻿namespace CrmPlatformAPI.Models.DTO
{
    public class ImageDTO
    {
        public int Id { get; set; }

        public string? PublicId { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public DateTime UploadDate { get; set; }
    }
}
