using CrmPlatformAPI.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace CrmPlatformAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<BeneficiaryCompanies> BeneficiaryCompanies { get; set; }
    }
}
