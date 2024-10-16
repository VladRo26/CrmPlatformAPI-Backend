using CrmPlatformAPI.Models.Domain;

namespace CrmPlatformAPI.Repositories.Interface
{
    public interface IRepositoryBeneficiaryCompany
    {
        Task<BeneficiaryCompany> CreateAsync(BeneficiaryCompany beneficiaryCompanies);

        Task<IEnumerable<BeneficiaryCompany>> GetBeneficiaryCompaniesAsync();

    }
}
