using CrmPlatformAPI.Data;
using CrmPlatformAPI.Helpers;
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
            return await _context.SoftwareCompanies
              .Include(sc => sc.CompanyPhoto) // Include the CompanyPhoto relationship
              .ToListAsync();
        }



        Task<bool> IRepositorySoftwareCompany.ExistsAsync(string name)
        {
            return _context.SoftwareCompanies.AnyAsync(c => c.Name == name);
        }

        Task<SoftwareCompany?> IRepositorySoftwareCompany.GetSoftwareCompanyByNameAsync(string name)
        {

            return _context.SoftwareCompanies.FirstOrDefaultAsync(c => c.Name == name);
        }

        public async Task<SoftwareCompany?> GetSoftwareCompanyByUserIdAsync(int userId)
        {
            if (_context == null)
            {
                return null;
            }

            return await _context.SoftwareCompanies
                .Include(sc => sc.Users) // Include related Users
                .Include(sc => sc.CompanyPhoto) // Include the CompanyPhoto relationship
                .FirstOrDefaultAsync(sc => sc.Users.Any(user => user.Id == userId));
        }

        public async Task<PagedList<SoftwareCompany>> GetCompaniesAsync(CompanyParams companyParams)
        {
            if (_context == null)
                return null;

            var query = _context.SoftwareCompanies
                                .Include(sc => sc.CompanyPhoto)
                                .AsQueryable();

            if (!string.IsNullOrEmpty(companyParams.CompanyName))
            {
                query = query.Where(c => c.Name.Contains(companyParams.CompanyName));
            }

            if (!string.IsNullOrEmpty(companyParams.OrderBy))
            {
                query = query.OrderBy(c => c.Name);
            }

            return await PagedList<SoftwareCompany>.CreateAsync(query, companyParams.PageNumber, companyParams.PageSize);
        }




    }
}
