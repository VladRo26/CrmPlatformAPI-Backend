using Azure;
using CrmPlatformAPI.Data;
using CrmPlatformAPI.Helpers;
using CrmPlatformAPI.Models.Domain;
using CrmPlatformAPI.Models.DTO;
using CrmPlatformAPI.Repositories.Interface;
using Microsoft.AspNetCore.Mvc;
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
            return await _context.BeneficiaryCompanies
               .Include(bc => bc.CompanyPhoto) // Include the CompanyPhoto relationship
               .ToListAsync();
        }


        public async Task<BeneficiaryCompany?> GetCompanyByUsernameAsync(string username)
        {
            if (_context == null)
            {
                return null;
            }

            return await _context.BeneficiaryCompanies
                .Include(bc => bc.Users) // Include the Users relationship
            .Include(bc => bc.CompanyPhoto) // Include the CompanyPhoto relationship
                .FirstOrDefaultAsync(bc => bc.Users.Any(user => user.UserName == username));
        }

        public async Task<BeneficiaryCompany?> GetBeneficiaryCompanyByUserIdAsync(int userId)
        {
            if (_context == null)
            {
                return null;
            }

            return await _context.BeneficiaryCompanies
                .Include(bc => bc.Users) // Include related Users
                .Include(bc => bc.CompanyPhoto) // Include the CompanyPhoto relationship
                .FirstOrDefaultAsync(bc => bc.Users.Any(user => user.Id == userId));
        }

        public async Task<PagedList<BeneficiaryCompany>> GetCompaniesAsync(CompanyParams companyParams)
        {
            if (_context == null)
                return null;

            var query = _context.BeneficiaryCompanies
                                .Include(bc => bc.CompanyPhoto)
                                .AsQueryable();

            if (!string.IsNullOrEmpty(companyParams.CompanyName))
            {
                query = query.Where(c => c.Name.Contains(companyParams.CompanyName));
            }

            if (!string.IsNullOrEmpty(companyParams.OrderBy))
            {
                query = query.OrderBy(c => c.Name);
            }

            return await PagedList<BeneficiaryCompany>.CreateAsync(query, companyParams.PageNumber, companyParams.PageSize);
        }

        public async Task<BeneficiaryCompany?> GetByIdAsync(int id)
        {
            if (_context == null)
                return null;

            return await _context.BeneficiaryCompanies
                .Include(bc => bc.CompanyPhoto)
                .FirstOrDefaultAsync(bc => bc.Id == id);
        }

        public async Task<BeneficiaryCompany?> UpdateAsync(BeneficiaryCompany updatedCompany)
        {
            if (_context == null)
                return null;

            // Retrieve the existing company record including the CompanyPhoto
            var existingCompany = await _context.BeneficiaryCompanies
                .Include(bc => bc.CompanyPhoto)
                .FirstOrDefaultAsync(bc => bc.Id == updatedCompany.Id);

            if (existingCompany == null)
                return null;

            // Manually update only the properties you want to change.
            existingCompany.Name = updatedCompany.Name;
            existingCompany.ShortDescription = updatedCompany.ShortDescription;
            existingCompany.ActivityDomain = updatedCompany.ActivityDomain;
            existingCompany.Address = updatedCompany.Address;
            existingCompany.EstablishmentDate = updatedCompany.EstablishmentDate;

            // Update the CompanyPhoto reference. This will update if a new photo is provided,
            // or leave it unchanged if no new photo is provided.
            existingCompany.CompanyPhoto = updatedCompany.CompanyPhoto;

            await _context.SaveChangesAsync();
            return existingCompany;
        }

        public async Task<BeneficiaryCompany?> GetBeneficiaryCompanyByNameAsync(string companyName)
        {
            if (_context == null)
                return null;

            return await _context.BeneficiaryCompanies
                .Include(bc => bc.CompanyPhoto) // Include photo if needed
                .FirstOrDefaultAsync(bc => bc.Name == companyName);
        }





    }
}
