using CrmPlatformAPI.Data;
using CrmPlatformAPI.Helpers;
using CrmPlatformAPI.Helpers.Enums;
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

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;

        }

        public async Task<PagedList<User>> GetAllAsync(UserParams userParams)
        {
            var query = _context.Users
               .Include(u => u.SoftwareCompany)
               .Include(u => u.BeneficiaryCompany)
               .Include(u => u.Photo)
               .AsQueryable();

            query = query.Where(u => u.UserName != userParams.CurrentUserName);


            if (!string.IsNullOrEmpty(userParams.CompanyName))
            {
                query = query.Where(u =>
                    (u.SoftwareCompany != null && EF.Functions.Like(u.SoftwareCompany.Name, $"%{userParams.CompanyName}%")) ||
                    (u.BeneficiaryCompany != null && EF.Functions.Like(u.BeneficiaryCompany.Name, $"%{userParams.CompanyName}%"))
                );
            }

            if (!string.IsNullOrEmpty(userParams.UserType) &&
                Enum.TryParse(userParams.UserType, true, out UserType userTypeEnum))
            {
                query = query.Where(u => u.UserType == userTypeEnum);
            }

            if (userParams.Rating > 0)
            {
                query = query.Where(u => u.Rating >= userParams.Rating);
            }

            // Search by FirstName or LastName (case-insensitive)
            if (!string.IsNullOrEmpty(userParams.Name))
            {
                query = query.Where(u =>
                   u.FirstName == userParams.Name || // Match exact first name
                   u.LastName == userParams.Name ||  // Match exact last name
                   (u.FirstName + " " + u.LastName) == userParams.Name); // Match full name
            }

            query = userParams.OrderBy switch
            {
                "rating" => query.OrderByDescending(u => u.Rating),
                "firstname" => query.OrderBy(u => u.FirstName),
                "hiredate" => query.OrderByDescending(u => u.HireDate),
                _ => query.OrderBy(u => u.LastName)
            };

            return await PagedList<User>.CreateAsync(query, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users
               .Include(u => u.SoftwareCompany)
               .Include(u => u.BeneficiaryCompany)
               .Include(u => u.Photo)
               .ToListAsync();
        }

        public async Task<User?> GetByIdAsync(int? id)
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

        public async Task<string?> GetPhotoUrlByUsernameAsync(string username)
        {
            if (_context == null)
            {
                return null;
            }

            var user = await _context.Users
                .Include(u => u.Photo)
                .FirstOrDefaultAsync(u => u.UserName == username);

            return user?.Photo?.Url;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) return false;

            _context.Users.Remove(user);
            return await _context.SaveChangesAsync() > 0;
        }



    }
}
