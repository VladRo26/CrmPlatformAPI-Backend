using CrmPlatformAPI.Helpers;
using CrmPlatformAPI.Models.Domain;

namespace CrmPlatformAPI.Repositories.Interface
{
    public interface IRepositorySoftwareCompany
    {
        Task<SoftwareCompany> CreateAsync(SoftwareCompany softwareCompany);

        Task<IEnumerable<SoftwareCompany>> GetSoftwareCompaniesAsync();

        Task<SoftwareCompany?> GetSoftwareCompanyByNameAsync(string name);

        Task<SoftwareCompany?> GetSoftwareCompanyByUserIdAsync(int userId);

        Task<PagedList<SoftwareCompany>> GetCompaniesAsync(CompanyParams companyParams);

        Task<bool> ExistsAsync(string name);
    }

}
