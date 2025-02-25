using CrmPlatformAPI.Helpers;
using CrmPlatformAPI.Models.Domain;

namespace CrmPlatformAPI.Repositories.Interface
{
    public interface IRepositoryBeneficiaryCompany
    {
        Task<BeneficiaryCompany> CreateAsync(BeneficiaryCompany beneficiaryCompanies);

        Task<IEnumerable<BeneficiaryCompany>> GetBeneficiaryCompaniesAsync();

        Task<BeneficiaryCompany?> GetBeneficiaryCompanyByNameAsync(string name);

        Task<BeneficiaryCompany?> GetCompanyByUsernameAsync(string username);

        Task<BeneficiaryCompany?> GetBeneficiaryCompanyByUserIdAsync(int userId);

        Task<PagedList<BeneficiaryCompany>> GetCompaniesAsync(CompanyParams companyParams);

        Task<BeneficiaryCompany?> GetByIdAsync(int id);

        Task<BeneficiaryCompany?> UpdateAsync(BeneficiaryCompany updatedCompany);





    }
}
