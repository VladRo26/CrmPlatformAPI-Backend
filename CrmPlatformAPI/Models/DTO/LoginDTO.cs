using Microsoft.Identity.Client;

namespace CrmPlatformAPI.Models.DTO
{
    public class LoginDTO
    {
        public required string UserName { get; set; }

        public required string Password { get; set; }

        public string? PhotoUrl { get; set; }


    }
}
