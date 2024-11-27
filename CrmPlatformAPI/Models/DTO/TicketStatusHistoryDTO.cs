using CrmPlatformAPI.Helpers.Enums;
using System.ComponentModel.DataAnnotations;

namespace CrmPlatformAPI.Models.DTO
{
    public class TicketStatusHistoryDTO
    {

        [Required(ErrorMessage = "Status is required.")]
        [MaxLength(50, ErrorMessage = "Status cannot exceed 50 characters.")]
        public string Status { get; set; }

        [MaxLength(500, ErrorMessage = "Message cannot exceed 500 characters.")]
        public string? Message { get; set; }

        [Required(ErrorMessage = "Updated by user is required.")]
        public int? UpdatedByUserId { get; set; }

        [Required(ErrorMessage = "Role of the updater is required.")]
        public TicketUserRole TicketUserRole { get; set; } // Enum for Creator/Handler
    }
}
