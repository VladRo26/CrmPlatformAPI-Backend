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

        public async Task<IEnumerable<Models.Domain.Contract>> GetContractsBySoftwareCompanyNameAsync(string softwareCompanyName)
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
                .Where(c => c.SoftwareCompany.Name == softwareCompanyName)
                .ToListAsync();
        }

        public async Task<Models.Domain.Contract?> GetContractByTicketIdAsync(int ticketId)
        {
            if (_context == null)
            {
                return null;
            }

            return await _context.Contracts
                .Include(c => c.BeneficiaryCompany)
                    .ThenInclude(bc => bc.CompanyPhoto)
                .Include(c => c.SoftwareCompany)
                    .ThenInclude(sc => sc.CompanyPhoto)
                .FirstOrDefaultAsync(c => c.Id ==
                    (from t in _context.Tickets
                     where t.Id == ticketId
                     select t.ContractId).FirstOrDefault());
        }

        public async Task<int> CountContractsAsync()
        {
            if (_context == null)
            {
                throw new Exception("Database context is not initialized.");
            }

            // Count all contracts in the Contracts DbSet.
            return await _context.Contracts.CountAsync();
        }

        public async Task<Models.Domain.Contract?> UpdateContractStatusAsync(int contractId, float newStatus)
        {
            if (_context == null)
            {
                return null;
            }

            // Find the contract by its id
            var contract = await _context.Contracts.FindAsync(contractId);
            if (contract == null)
            {
                return null;
            }

            // Update only the status property (as float)
            contract.Status = newStatus;
            await _context.SaveChangesAsync();

            return contract;
        }

        public async Task<Models.Domain.Contract?> UpdateContractAsync(Models.Domain.Contract updatedContract)
        {
            if (_context == null)
            {
                return null;
            }

            // Retrieve the existing contract record
            var existingContract = await _context.Contracts.FindAsync(updatedContract.Id);
            if (existingContract == null)
            {
                return null;
            }

            // Preserve the foreign key values
            updatedContract.BeneficiaryCompanyId = existingContract.BeneficiaryCompanyId;
            updatedContract.SoftwareCompanyId = existingContract.SoftwareCompanyId;

            // Optionally preserve navigation properties if needed:
            // updatedContract.BeneficiaryCompany = existingContract.BeneficiaryCompany;
            // updatedContract.SoftwareCompany = existingContract.SoftwareCompany;

            // Update all values
            _context.Entry(existingContract).CurrentValues.SetValues(updatedContract);
            await _context.SaveChangesAsync();

            return existingContract;
        }


        public async Task<Models.Domain.Contract?> GetContractByIdAsync(int id)
        {
            if (_context == null)
            {
                return null;
            }

            return await _context.Contracts
                .Include(c => c.BeneficiaryCompany)
                    .ThenInclude(bc => bc.CompanyPhoto)
                .Include(c => c.SoftwareCompany)
                    .ThenInclude(sc => sc.CompanyPhoto)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

    }
}
