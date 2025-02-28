namespace CrmPlatformAPI.Models.Domain
{
    public class Feedback
    {
        public int Id { get; set; } 

        public int? FromUserId { get; set; }
        public User? FromUser { get; set; }

        public int? ToUserId { get; set; }
        public User? ToUser { get; set; }

        public int? TicketId { get; set; }
        public Ticket? Ticket { get; set; }

        public string? Content { get; set; }

        public int Rating { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
