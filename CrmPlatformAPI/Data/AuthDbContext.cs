using CrmPlatformAPI.Models.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CrmPlatformAPI.Data
{
    public class AuthDbContext: IdentityDbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
        {

        }

        override protected void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //adding roles
            var roles = new List<IdentityRole>
            {
                new IdentityRole {Id = "1", Name = "Admin", NormalizedName = "ADMIN", ConcurrencyStamp = "1"},
                new IdentityRole {Id = "2", Name = "ServiceUser", NormalizedName = "SERVICEUSER", ConcurrencyStamp = "2"},
                new IdentityRole {Id = "3", Name = "Customer", NormalizedName = "CUSTOMER", ConcurrencyStamp = "3"}
            };

            modelBuilder.Entity<IdentityRole>().HasData(roles);

            var admin = new IdentityUser()
            {
                Id = "9714d24c-a7fe-4772-bf0d-320471b2f35a",
                UserName = "admin",
                NormalizedUserName = "ADMIN",
                Email = "admin@admin",
                NormalizedEmail = "ADMIN@ADMIN",
            };

            admin.PasswordHash = new PasswordHasher<IdentityUser>().HashPassword(admin, "admin");

            modelBuilder.Entity<IdentityUser>().HasData(admin);

            var userRoles = new List<IdentityUserRole<string>>
            {
                new IdentityUserRole<string> {RoleId = "1", UserId = "9714d24c-a7fe-4772-bf0d-320471b2f35a"}
            };
        }
    }
}
