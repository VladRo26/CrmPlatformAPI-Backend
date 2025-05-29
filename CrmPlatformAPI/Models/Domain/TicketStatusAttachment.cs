namespace CrmPlatformAPI.Models.Domain
{
    public class TicketStatusAttachment
    {
        public int Id { get; set; }

        public int TicketStatusHistoryId { get; set; }
        public TicketStatusHistory TicketStatusHistory { get; set; }

        public string FileName { get; set; }
        public string FileType { get; set; }
        public string Url { get; set; }
        public string PublicId { get; set; }

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    }
}
