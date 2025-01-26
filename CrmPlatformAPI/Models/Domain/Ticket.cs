using CrmPlatformAPI.Helpers.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrmPlatformAPI.Models.Domain
{
    public class Ticket
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public TicketStatus Status { get; set; } // Use enum for Status
        public TicketPriority Priority { get; set; }
        public TicketType Type { get; set; }
        public int ContractId { get; set; }

        public Contract? Contract { get; set; }

        [ForeignKey("Creator")]
        public int CreatorId { get; set; }
        public User Creator { get; set; }

        [ForeignKey("Handler")]
        public int? HandlerId { get; set; } 
        public User? Handler { get; set; }

        public DateTime CreatedAt { get; set; }

        public string Language { get; set; }

        public string LanguageCode { get; set; }


        public string CountryCode { get; set; }


        public ICollection<TicketStatusHistory> StatusHistory { get; set; } = new List<TicketStatusHistory>();

    }
}
