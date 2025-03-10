using CrmPlatformAPI.Data;
using CrmPlatformAPI.Models.Domain;
using CrmPlatformAPI.Repositories.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CrmPlatformAPI.Repositories.Implementation
{
    public class RepositoryHomeImage : IRepositoryHomeImage
    {
        private readonly ApplicationDbContext? _context;

        public RepositoryHomeImage(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<HomeImage>> GetImagesAsync()
        {
            if (_context == null)
            {
                return null;
            }

            return await _context.HomeImages.ToListAsync();
        }

        public async Task<HomeImage> CreateAsync(HomeImage homeImage)
        {
            if (_context == null)
            {
                throw new Exception("Database context is not initialized.");
            }

            await _context.HomeImages.AddAsync(homeImage);
            await _context.SaveChangesAsync();
            return homeImage;
        }

    }
}
