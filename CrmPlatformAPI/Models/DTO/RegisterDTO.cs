using CrmPlatformAPI.Helpers.Enums;
using System.ComponentModel.DataAnnotations;

namespace CrmPlatformAPI.Models.DTO
{
    public class RegisterDTO
    {
        [Required]
        public string UserName { get; set; } = string.Empty;
        [Required]
        public string? FirstName { get; set; }
        [Required]
        public string? Address { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? CompanyName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public UserType UserType { get; set; }

        public DateOnly HireDate { get; set; }

        [Required]
        [StringLength(12, MinimumLength = 4)]
        public string Password { get; set; } = string.Empty;
    }
}

