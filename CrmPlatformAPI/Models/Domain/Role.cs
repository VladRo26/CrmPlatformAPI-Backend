using Microsoft.AspNetCore.Identity;
using System.Collections;

namespace CrmPlatformAPI.Models.Domain
{
    public class Role : IdentityRole<int>
    {
        public ICollection<UserRole> UserRoles { get; set; } = [];

    }
}
