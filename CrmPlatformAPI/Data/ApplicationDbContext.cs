using CrmPlatformAPI.Models.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace CrmPlatformAPI.Data
{
    public class ApplicationDbContext(DbContextOptions options) : IdentityDbContext<User,Role,int,IdentityUserClaim<int>,
        UserRole,IdentityUserLogin<int>,IdentityRoleClaim<int>,IdentityUserToken<int>>(options)
    {
        public DbSet<BeneficiaryCompany> BeneficiaryCompanies { get; set; }
        public DbSet<SoftwareCompany> SoftwareCompanies { get; set; }

        public DbSet<HomeImage> HomeImages { get; set; }

        public DbSet<Contract> Contracts { get; set; }

        public DbSet<CompanyPhoto> CompanyPhotos { get; set; }

        public DbSet<Ticket> Tickets { get; set; }

        public DbSet<TicketStatusHistory> TicketStatusHistories { get; set; }

        public DbSet<Feedback> Feedbacks { get; set; }

        public DbSet<FeedBackSentiment> FeedbackSentiments { get; set; }



        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>()
                .HasKey(u => u.Id);

            builder.Entity<User>()
                .HasMany(ur => ur.UserRoles)
                .WithOne(u => u.User)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Keeps roles even if user is deleted


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
                .OnDelete(DeleteBehavior.Cascade);


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

            builder.Entity<BeneficiaryCompany>()
                .HasOne(b => b.CompanyPhoto)
                .WithOne(p => p.BeneficiaryCompany)
                .HasForeignKey<CompanyPhoto>(p => p.BeneficiaryCompanyId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<SoftwareCompany>()
                .HasOne(s => s.CompanyPhoto)
                .WithOne(p => p.SoftwareCompany)
                .HasForeignKey<CompanyPhoto>(p => p.SoftwareCompanyId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Ticket>()
                .HasOne(t => t.Contract)
                .WithMany(c => c.Tickets)
                .HasForeignKey(t => t.ContractId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Ticket>()
                .HasOne(t => t.Creator)
                .WithMany()
                .HasForeignKey(t => t.CreatorId)
                .OnDelete(DeleteBehavior.Restrict); // Prevents cascade issues


            builder.Entity<Ticket>()
               .HasOne(t => t.Handler)
               .WithMany()
               .HasForeignKey(t => t.HandlerId)
               .OnDelete(DeleteBehavior.SetNull); // This can remain as SetNull


            builder.Entity<TicketStatusHistory>()
                .HasOne(sh => sh.Ticket)
                .WithMany(t => t.StatusHistory)
                .HasForeignKey(sh => sh.TicketId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<TicketStatusHistory>()
                .HasOne(h => h.UpdatedByUser)
                .WithMany()
                .HasForeignKey(h => h.UpdatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);


            builder.Entity<Ticket>()
                  .Property(t => t.Status)
                  .HasConversion<string>();

            builder.Entity<TicketStatusHistory>()
                .Property(h => h.TicketUserRole)
                .HasConversion<string>();

            builder.Entity<TicketStatusHistory>()
                .Property(h => h.Status)
                .HasConversion<string>();

            builder.Entity<Feedback>()
                 .HasOne(f => f.FromUser)
                 .WithMany()
                 .HasForeignKey(f => f.FromUserId)
                 .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Feedback>()
                .HasOne(f => f.ToUser)
                .WithMany()
                .HasForeignKey(f => f.ToUserId)
                .OnDelete(DeleteBehavior.Restrict);



            builder.Entity<Feedback>()
                .HasOne(f => f.Ticket)
                .WithMany() 
                .HasForeignKey(f => f.TicketId)
                .OnDelete(DeleteBehavior.SetNull); 


        }
    }
}
