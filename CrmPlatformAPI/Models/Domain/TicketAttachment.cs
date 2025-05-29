namespace CrmPlatformAPI.Models.Domain
{
    public class TicketAttachment
    {
        public int Id { get; set; }

        public int TicketId { get; set; }
        public Ticket Ticket { get; set; }

        public string FileName { get; set; }
        public string FileType { get; set; } 
        public string Url { get; set; }      
        public string PublicId { get; set; } 
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    }
}
