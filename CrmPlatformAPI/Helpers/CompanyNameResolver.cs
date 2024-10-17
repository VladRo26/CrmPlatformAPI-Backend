using AutoMapper;
using CrmPlatformAPI.Data;
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
        var softwareCompany = _context.SoftwareCompanies
            .FirstOrDefault(c => c.Id == source.SoftwareCompanyId);

        return softwareCompany?.Name ?? string.Empty; 
    }
}
