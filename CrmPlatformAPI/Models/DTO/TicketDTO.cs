using CrmPlatformAPI.Helpers.Enums;
using System.ComponentModel.DataAnnotations;

namespace CrmPlatformAPI.Models.DTO
{
    public class TicketDTO
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Title is required.")]
        [MaxLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
        public string? Title { get; set; }

        [MaxLength(500, ErrorMessage = "Description cannot exceed 1000 characters.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        public string Status { get; set; } 

        [Required(ErrorMessage = "Priority is required.")]
        [MaxLength(50, ErrorMessage = "Priority cannot exceed 50 characters.")]
        public string? Priority { get; set; }

        [MaxLength(50, ErrorMessage = "Type cannot exceed 50 characters.")]
        public string? Type { get; set; }

        [Required(ErrorMessage = "Contract ID is required.")]
        public int ContractId { get; set; }

        public int CreatorId { get; set; }

        public int? HandlerId { get; set; }

        public string Language { get; set; }

        public string CountryCode { get; set; }

        public string LanguageCode { get; set; }
    }
}
