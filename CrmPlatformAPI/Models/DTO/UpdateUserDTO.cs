using CrmPlatformAPI.Helpers.Enums;
using System.ComponentModel.DataAnnotations;

namespace CrmPlatformAPI.Models.DTO
{
    public class UpdateUserDTO
    {
        [Required]
        public string UserName { get; set; } = string.Empty;
        [Required]
        public string? FirstName { get; set; }
        [Required]
        public string? Address { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }

        [Required]
        [StringLength(12, MinimumLength = 4)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [StringLength(12, MinimumLength = 4)]
        public string PasswordConfirm { get; set; } = string.Empty;

    }
}
