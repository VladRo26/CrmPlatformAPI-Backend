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
        public DbSet<SoftwareCompany> SoftwareCompanies { get; set; }

        public DbSet<HomeImage> HomeImages { get; set; }

        public DbSet<Contract> Contracts { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>()
                .HasKey(u => u.Id);

            builder.Entity<User>()
                .HasMany(ur => ur.UserRoles)
                .WithOne(u => u.User)
                .HasForeignKey(ur => ur.UserId);

            builder.Entity<SoftwareCompany>()
                   .HasMany(sc => sc.Users)
                   .WithOne(u => u.SoftwareCompany)
                   .HasForeignKey(u => u.SoftwareCompanyId)
                   .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<BeneficiaryCompany>()
               .HasMany(bc => bc.Users)
               .WithOne(u => u.BeneficiaryCompany)
               .HasForeignKey(u => u.BeneficiaryCompanyId)
               .OnDelete(DeleteBehavior.SetNull);


            builder.Entity<Role>()
                .HasMany(ur => ur.UserRoles)
                .WithOne(r => r.Role)
                .HasForeignKey(ur => ur.RoleId)
                .IsRequired();

            builder.Entity<User>()
                .HasOne(u => u.Photo)
                .WithOne(p => p.User)
                .HasForeignKey<Photo>(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Contract>()
              .HasKey(c => c.Id);

            builder.Entity<Contract>()
                .HasOne(c => c.BeneficiaryCompany)
                .WithMany(b => b.Contracts)
                .HasForeignKey(c => c.BeneficiaryCompanyId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Contract>()
                .HasOne(c => c.SoftwareCompany)
                .WithMany(s => s.Contracts)
                .HasForeignKey(c => c.SoftwareCompanyId)
                .OnDelete(DeleteBehavior.Cascade);
        }


    }
}
