using CrmPlatformAPI.Models.Domain;

namespace CrmPlatformAPI.Repositories.Interface
{
    public interface IRepositorySoftwareCompany
    {
        Task<SoftwareCompany> CreateAsync(SoftwareCompany softwareCompany);

        Task<IEnumerable<SoftwareCompany>> GetSoftwareCompaniesAsync();

        Task<SoftwareCompany?> GetSoftwareCompanyByIdAsync(string name);

        Task<bool> ExistsAsync(string name);
    }

}
