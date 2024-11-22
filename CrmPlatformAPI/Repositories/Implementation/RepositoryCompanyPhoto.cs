using Microsoft.EntityFrameworkCore;
using CrmPlatformAPI.Data;
using CrmPlatformAPI.Models.Domain;
using CrmPlatformAPI.Repositories.Interface;

public class RepositoryCompanyPhoto : IRepositoryCompanyPhoto
{
    private readonly ApplicationDbContext _context;

    public RepositoryCompanyPhoto(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<string?> GetComapanyPhotoUrlAsync(int userId)
    {
        var user = await _context.Users
            .Include(u => u.BeneficiaryCompany)
                .ThenInclude(b => b.CompanyPhoto)
            .Include(u => u.SoftwareCompany)
                .ThenInclude(s => s.CompanyPhoto)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            return null; 
        }

        if (user.BeneficiaryCompany?.CompanyPhoto != null)
        {
            return user.BeneficiaryCompany.CompanyPhoto.Url;
        }

        if (user.SoftwareCompany?.CompanyPhoto != null)
        {
            return user.SoftwareCompany.CompanyPhoto.Url;
        }

        return null; // No photo available
    }

}
