using CrmPlatformAPI.Helpers.Enums;
using System.ComponentModel.DataAnnotations;

namespace CrmPlatformAPI.Models.DTO
{
    public class TicketStatusHistoryDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        [MaxLength(50, ErrorMessage = "Status cannot exceed 50 characters.")]
        public string Status { get; set; }

        [MaxLength(500, ErrorMessage = "Message cannot exceed 500 characters.")]
        public string? Message { get; set; }

        [Required(ErrorMessage = "Updated by username is required.")]
        public string UpdatedByUsername { get; set; } // Changed from ID to username

        public DateTime UpdatedAt { get; set; } 


        [Required(ErrorMessage = "Role of the updater is required.")]
        public string TicketUserRole { get; set; } // Enum for Creator/Handler

        public bool Seen { get; set; } = false;
    }
}
