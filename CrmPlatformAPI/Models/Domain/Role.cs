using Microsoft.AspNetCore.Identity;
using System.Collections;

namespace CrmPlatformAPI.Models.Domain
{
    public class Role : IdentityRole
    {
        public ICollection<Role> Roles { get; set; } = [];

    }
}
