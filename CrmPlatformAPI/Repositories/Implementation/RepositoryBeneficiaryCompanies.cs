using CrmPlatformAPI.Data;
using CrmPlatformAPI.Models.Domain;
using CrmPlatformAPI.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace CrmPlatformAPI.Repositories.Implementation
{
    public class RepositoryBeneficiaryCompanies : IRepositoryBeneficiaryCompanies
    {
        private readonly ApplicationDbContext? _context;

        public RepositoryBeneficiaryCompanies(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<BeneficiaryCompanies?> CreateAsync(BeneficiaryCompanies beneficiaryCompanies )
        {

            if (_context == null)
            {
                return null;
            }
            await _context.BeneficiaryCompanies.AddAsync(beneficiaryCompanies);
            await _context.SaveChangesAsync();

            return beneficiaryCompanies;

        }

        public async Task<IEnumerable<BeneficiaryCompanies>?> GetBeneficiaryCompaniesAsync()
        {
            if (_context == null)
            {
                return null;
            }
            return await _context.BeneficiaryCompanies.ToListAsync();
        }
    }
}
