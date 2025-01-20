using CrmPlatformAPI.Data;
using CrmPlatformAPI.Models.Domain;
using CrmPlatformAPI.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Contracts;

namespace CrmPlatformAPI.Repositories.Implementation
{

    
    public class RepositoryContract : IRepositoryContract
    {
        private readonly ApplicationDbContext? _context;

        public RepositoryContract(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<IEnumerable<Models.Domain.Contract>> GetContractsByNameAsync(string? softComp, string? benefComp)
        {
            if (_context == null)
            {
                return null;
            }

            var query = _context.Contracts
                .Include(c => c.BeneficiaryCompany)
                    .ThenInclude(bc => bc.CompanyPhoto)  // Include BeneficiaryCompany photo
                .Include(c => c.SoftwareCompany)
                    .ThenInclude(sc => sc.CompanyPhoto) // Include SoftwareCompany photo
                .AsQueryable();

            if (!string.IsNullOrEmpty(softComp))
            {
                query = query.Where(c => c.SoftwareCompany.Name == softComp);
            }

            if (!string.IsNullOrEmpty(benefComp))
            {
                query = query.Where(c => c.BeneficiaryCompany.Name == benefComp);
            }

            return await query.ToListAsync();
        }



        public async Task<IEnumerable<Models.Domain.Contract>> GetContractsAsync()
        {
            if (_context == null)
            {
                return null;
            }


            return await _context.Contracts
                .Include(c => c.BeneficiaryCompany)
                    .ThenInclude(bc => bc.CompanyPhoto)  // Include BeneficiaryCompany photo
                .Include(c => c.SoftwareCompany)
                    .ThenInclude(sc => sc.CompanyPhoto) // Include SoftwareCompany photo
                .ToListAsync();
        }


        public async Task<Models.Domain.Contract?> CreateAsync(Models.Domain.Contract contract, string? beneficiaryCompanyName, string? softwareCompanyName)
        {
            // Look up BeneficiaryCompanyId based on BeneficiaryCompanyName
            var beneficiaryCompany = await _context.BeneficiaryCompanies
                .FirstOrDefaultAsync(bc => bc.Name == beneficiaryCompanyName);
            if (beneficiaryCompany == null)
            {
                throw new ArgumentException($"Beneficiary company '{beneficiaryCompanyName}' not found.");
            }

            var softwareCompany = await _context.SoftwareCompanies
                .FirstOrDefaultAsync(sc => sc.Name == softwareCompanyName);
            if (softwareCompany == null)
            {
                throw new ArgumentException($"Software company '{softwareCompanyName}' not found.");
            }

   
            contract.BeneficiaryCompanyId = beneficiaryCompany.Id;
            contract.SoftwareCompanyId = softwareCompany.Id;


            await _context.Contracts.AddAsync(contract);
            await _context.SaveChangesAsync();

            return contract;
        }

        public async Task<IEnumerable<Models.Domain.Contract>> GetContractsByBeneficiaryCompanyNameAsync(string beneficiaryCompanyName)
        {
            if (_context == null)
            {
                return null;
            }

            return await _context.Contracts
                .Include(c => c.BeneficiaryCompany)
                    .ThenInclude(bc => bc.CompanyPhoto)  // Include BeneficiaryCompany photo
                .Include(c => c.SoftwareCompany)
                    .ThenInclude(sc => sc.CompanyPhoto) // Include SoftwareCompany photo
                .Where(c => c.BeneficiaryCompany.Name == beneficiaryCompanyName)
                .ToListAsync();
        }

    }
}
