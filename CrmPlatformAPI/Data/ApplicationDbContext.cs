using CrmPlatformAPI.Models.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CrmPlatformAPI.Data
{
    public class ApplicationDbContext(DbContextOptions options) : IdentityDbContext<User,Role,int,IdentityUserClaim<int>,
        UserRole,IdentityUserLogin<int>,IdentityRoleClaim<int>,IdentityUserToken<int>>(options)
    {
        public DbSet<BeneficiaryCompany> BeneficiaryCompanies { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>()
                .HasMany(ur => ur.UserRoles)
                .WithOne(u => u.User)
                .HasForeignKey(ur => ur.UserId)
                .IsRequired();

            builder.Entity<Role>()
                .HasMany(ur => ur.UserRoles)
                .WithOne(r => r.Role)
                .HasForeignKey(ur => ur.RoleId)
                .IsRequired();
        }


    }
}
