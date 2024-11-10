using CrmPlatformAPI.Models.Domain;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace CrmPlatformAPI.Data
{
    public class Seed
    {
        public static async Task SeedSoftwareComp(ApplicationDbContext context)
        {
            if (await context.SoftwareCompanies.AnyAsync()) return;

            var softwareData = await System.IO.File.ReadAllTextAsync("Data/SoftwareSeedData.json");
            var softwareCompanies = JsonSerializer.Deserialize<List<SoftwareCompany>>(softwareData);

            foreach (var softwareCompany in softwareCompanies)
            {
                // Fetch and link existing users by ID
                context.SoftwareCompanies.Add(softwareCompany);
            }

            await context.SaveChangesAsync();
        }

        public static async Task SeedBeneficiaryComp(ApplicationDbContext context)
        {
            if (await context.BeneficiaryCompanies.AnyAsync()) return;

            var beneficiaryData = await System.IO.File.ReadAllTextAsync("Data/BenefSeedData.json");
            var beneficiaryCompanies = JsonSerializer.Deserialize<List<BeneficiaryCompany>>(beneficiaryData);

            foreach (var beneficiaryCompany in beneficiaryCompanies)
            {
                // Fetch and link existing users by ID
                context.BeneficiaryCompanies.Add(beneficiaryCompany);
            }

            await context.SaveChangesAsync();
        }

        public static async Task SeedContract(ApplicationDbContext context)
        {
            if (await context.Contracts.AnyAsync()) return;

            var contractData = await System.IO.File.ReadAllTextAsync("Data/ContractsSeedData.json");
            var contracts = JsonSerializer.Deserialize<List<Contract>>(contractData);

            foreach (var contract in contracts)
            {
                // Fetch and link existing users by ID
                context.Contracts.Add(contract);
            }

            await context.SaveChangesAsync();
        }   
    }
}
