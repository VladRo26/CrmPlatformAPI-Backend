using CrmPlatformAPI.Data;
using CrmPlatformAPI.Models.Domain;
using CrmPlatformAPI.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace CrmPlatformAPI.Repositories.Implementation
{
    public class RepositoryBeneficiaryCompany : IRepositoryBeneficiaryCompany
    {
        private readonly ApplicationDbContext? _context;

        public RepositoryBeneficiaryCompany(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<BeneficiaryCompany?> CreateAsync(BeneficiaryCompany beneficiaryCompanies)
        {

            if (_context == null)
            {
                return null;
            }
            await _context.BeneficiaryCompanies.AddAsync(beneficiaryCompanies);
            await _context.SaveChangesAsync();

            return beneficiaryCompanies;

        }

        public async Task<IEnumerable<BeneficiaryCompany>?> GetBeneficiaryCompaniesAsync()
        {
            if (_context == null)
            {
                return null;
            }
            return await _context.BeneficiaryCompanies.ToListAsync();
        }

        Task<BeneficiaryCompany?> IRepositoryBeneficiaryCompany.GetBeneficiaryCompanyByNameAsync(string name)
        {
            return _context.BeneficiaryCompanies.FirstOrDefaultAsync(c => c.Name == name);
        }
    }
}
