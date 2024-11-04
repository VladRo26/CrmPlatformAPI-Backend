using AutoMapper;
using CrmPlatformAPI.Data;
using CrmPlatformAPI.Helpers.Enums;
using CrmPlatformAPI.Models.Domain;
using CrmPlatformAPI.Models.DTO;

public class CompanyNameResolver : IValueResolver<User, RegisterDTO, string>
{
    private readonly ApplicationDbContext _context;

    public CompanyNameResolver(ApplicationDbContext context)
    {
        _context = context;
    }

    public string Resolve(User source, RegisterDTO destination, string destMember, ResolutionContext context)
    {
        if (source.UserType == UserType.SoftwareCompanyUser)
        {
            // Get the name of the SoftwareCompany if the user is a SoftwareCompanyUser
            var softwareCompany = _context.SoftwareCompanies
                .FirstOrDefault(c => c.Id == source.SoftwareCompanyId);

            return softwareCompany?.Name ?? string.Empty;
        }
        else if (source.UserType == UserType.BeneficiaryCompanyUser)
        {
            // Get the name of the BeneficiaryCompany if the user is a BeneficiaryCompanyUser
            var beneficiaryCompany = _context.BeneficiaryCompanies
                .FirstOrDefault(c => c.Id == source.BeneficiaryCompanyId);

            return beneficiaryCompany?.Name ?? string.Empty;
        }

        // Return empty string if neither is set
        return string.Empty;
    }
}
