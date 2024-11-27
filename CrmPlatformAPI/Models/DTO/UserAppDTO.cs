using CrmPlatformAPI.Helpers.Enums;

namespace CrmPlatformAPI.Models.DTO
{
    public class UserAppDTO
    {
        public int Id { get; set; }
        public string? UserName { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public string? CompanyName { get; set; }
        public UserType UserType { get; set; }

        public DateOnly HireDate { get; set; }

        public string? Token { get; set; }

        public string? PhotoUrl { get; set; }

        public string? CompanyPhotoUrl { get; set; }

        public float Rating { get; set; }
    }
}
