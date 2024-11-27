using System.ComponentModel.DataAnnotations;

namespace CrmPlatformAPI.Models.DTO
{
    public class FeedbackDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "FromUserId is required.")]
        public int FromUserId { get; set; }

        [Required(ErrorMessage = "ToUserId is required.")]
        public int ToUserId { get; set; }

        public int? TicketId { get; set; } // Nullable since feedback might not be tied to a ticket

        [MaxLength(1000, ErrorMessage = "Content cannot exceed 1000 characters.")]
        public string? Content { get; set; }

        [Required(ErrorMessage = "Rating is required.")]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Rating { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
