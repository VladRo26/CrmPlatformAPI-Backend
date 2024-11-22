using CrmPlatformAPI.Data;
using CrmPlatformAPI.Models.Domain;
using CrmPlatformAPI.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace CrmPlatformAPI.Repositories.Implementation
{
    public class RepositoryUser : IRepositoryUser
    {
        private readonly ApplicationDbContext? _context;

        public RepositoryUser(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> UpdateAsync(User user)
        {
            if (_context == null)
            {
                return false;
            }
            _context.Users.Update(user);
            return await _context.SaveChangesAsync() > 0;
        }



        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users
               .Include(u => u.SoftwareCompany)
               .Include(u => u.BeneficiaryCompany)
               .Include(u => u.Photo)
               .ToListAsync();
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            if (_context == null)
            {
                return null;
            }

            return await _context.Users
                .Include(u => u.SoftwareCompany)
                .Include(u => u.BeneficiaryCompany)
                .Include(u => u.Photo)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<IEnumerable<User>> GetByCompanyAsync(string name)
        {
            if (_context == null)
            {
                return null;
            }
            return await _context.Users
                .Include(u => u.Photo)
                .Where(u =>
                    (u.SoftwareCompany != null && u.SoftwareCompany.Name == name) ||
                    (u.BeneficiaryCompany != null && u.BeneficiaryCompany.Name == name)
                )
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetByNameAsync(string name)
        {
            if (_context == null)
            {
                return null;
            }
            return await _context.Users
                .Include(u => u.Photo)
                .Where(u => u.FirstName == name || u.LastName == name)
                .ToListAsync();
        }

        public async Task<User?> GetByUserNameAsync(string username)
        {
            if (_context == null)
            {
                return null;
            }

            // Use FirstOrDefaultAsync to get a single user
            return await _context.Users
                .Include(u => u.Photo)
                .FirstOrDefaultAsync(u => u.UserName == username);

        }

    }
}
