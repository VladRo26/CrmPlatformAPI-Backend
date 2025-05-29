namespace CrmPlatformAPI.Models.DTO
{
    public class TicketAttachmentDTO
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public string Url { get; set; }
        public DateTime UploadedAt { get; set; }
    }
}
