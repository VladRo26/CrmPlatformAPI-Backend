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
            // Check if there are existing records
            if (await context.SoftwareCompanies.AnyAsync()) return;

            // Read the JSON file containing software company data
            var softwareData = await System.IO.File.ReadAllTextAsync("Data/SoftwareSeedData.json");

            // Parse the JSON into a JsonDocument
            using var document = JsonDocument.Parse(softwareData);
            var softwareCompanies = document.RootElement.EnumerateArray();

            foreach (var company in softwareCompanies)
            {
                // Access JSON properties
                var name = company.GetProperty("Name").GetString();
                var shortDescription = company.GetProperty("ShortDescription").GetString();
                var establishmentDate = company.GetProperty("EstablishmentDate").GetString();
                var photoUrl = company.GetProperty("PhotoUrl").GetString();

                // Create a new SoftwareCompany
                var softwareCompany = new SoftwareCompany
                {
                    Name = name,
                    ShortDescription = shortDescription,
                    EstablishmentDate = DateOnly.Parse(establishmentDate)
                };

                context.SoftwareCompanies.Add(softwareCompany);
                await context.SaveChangesAsync(); 

                // Create an associated CompanyPhoto
                var companyPhoto = new CompanyPhoto
                {
                    Url = photoUrl,
                    PublicId = photoUrl,
                    SoftwareCompanyId = softwareCompany.Id 
                };

                // Add the photo to the context
                context.CompanyPhotos.Add(companyPhoto);
            }

            // Save all changes to the database
            await context.SaveChangesAsync();
        }

        public static async Task SeedBeneficiaryComp(ApplicationDbContext context)
        {
            // Check if there are existing records
            if (await context.BeneficiaryCompanies.AnyAsync()) return;

            // Read the JSON file containing beneficiary company data
            var beneficiaryData = await System.IO.File.ReadAllTextAsync("Data/BenefSeedData.json");

            // Parse the JSON into a JsonDocument
            using var document = JsonDocument.Parse(beneficiaryData);
            var beneficiaryCompanies = document.RootElement.EnumerateArray();

            foreach (var company in beneficiaryCompanies)
            {
                // Access JSON properties
                var name = company.GetProperty("Name").GetString();
                var shortDescription = company.GetProperty("ShortDescription").GetString();
                var activityDomain = company.GetProperty("ActivityDomain").GetString();
                var address = company.GetProperty("Address").GetString();
                var establishmentDate = company.GetProperty("EstablishmentDate").GetString();
                var photoUrl = company.GetProperty("PhotoUrl").GetString();

                // Create a new BeneficiaryCompany
                var beneficiaryCompany = new BeneficiaryCompany
                {
                    Name = name,
                    ShortDescription = shortDescription,
                    ActivityDomain = activityDomain,
                    Address = address,
                    EstablishmentDate = DateOnly.Parse(establishmentDate)
                };

                context.BeneficiaryCompanies.Add(beneficiaryCompany);
                await context.SaveChangesAsync();

                // Create an associated CompanyPhoto
                var companyPhoto = new CompanyPhoto
                {
                    Url = photoUrl,
                    PublicId = photoUrl,
                    BeneficiaryCompanyId = beneficiaryCompany.Id
                };

                context.CompanyPhotos.Add(companyPhoto);
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
