using CrmPlatformAPI.Data;
using CrmPlatformAPI.Models.Domain;
using CrmPlatformAPI.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace CrmPlatformAPI.Repositories.Implementation
{
    public class RepositorySoftwareCompany : IRepositorySoftwareCompany
    {
        private readonly ApplicationDbContext? _context;

        public RepositorySoftwareCompany(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<SoftwareCompany?> CreateAsync(SoftwareCompany softwareCompany)
        {
            if (_context == null)
            {
                return null;
            }
            await _context.SoftwareCompanies.AddAsync(softwareCompany);
            await _context.SaveChangesAsync();
            return softwareCompany;
        }

        public async Task<IEnumerable<SoftwareCompany>> GetSoftwareCompaniesAsync()
        {
            return await _context.SoftwareCompanies.ToListAsync();
        }



        Task<bool> IRepositorySoftwareCompany.ExistsAsync(string name)
        {
            return _context.SoftwareCompanies.AnyAsync(c => c.Name == name);
        }

        Task<SoftwareCompany?> IRepositorySoftwareCompany.GetSoftwareCompanyByIdAsync(string name)
        {

            return _context.SoftwareCompanies.FirstOrDefaultAsync(c => c.Name == name);
        }
    }
}
