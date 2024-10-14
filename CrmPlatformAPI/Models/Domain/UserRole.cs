using Microsoft.AspNetCore.Identity;

namespace CrmPlatformAPI.Models.Domain
{
    public class UserRole : IdentityUserRole<string>
    {
        public User User { get; set; }
        public Role Role { get; set; }
    }
}
