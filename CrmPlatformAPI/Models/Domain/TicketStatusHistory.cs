using CrmPlatformAPI.Helpers.Enums;

namespace CrmPlatformAPI.Models.Domain
{
    public class TicketStatusHistory
    {
        public int Id { get; set; }

        public int TicketId { get; set; }
        public Ticket Ticket { get; set; }

        public TicketStatus Status { get; set; }

        public string? Message { get; set; }

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow; 

        public int? UpdatedByUserId { get; set; }
        public User UpdatedByUser { get; set; }

        public TicketUserRole TicketUserRole { get; set; } // Use enum for Creator/Handler

        public bool Seen { get; set; } = false;



    }


}

