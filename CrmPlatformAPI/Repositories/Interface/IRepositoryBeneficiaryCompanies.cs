using CrmPlatformAPI.Models.Domain;

namespace CrmPlatformAPI.Repositories.Interface
{
    public interface IRepositoryBeneficiaryCompanies
    {
        Task<BeneficiaryCompanies> CreateAsync(BeneficiaryCompanies beneficiaryCompanies);

        Task<IEnumerable<BeneficiaryCompanies>> GetBeneficiaryCompaniesAsync();

    }
}
