using CrmPlatformAPI.Models.Domain;

namespace CrmPlatformAPI.Repositories.Interface
{
    public interface IRepositoryContract
    {

        Task<Contract> CreateAsync(Contract contract,string name1,string name2);

        Task<IEnumerable<Contract>> GetContractsAsync();

        Task<IEnumerable<Contract>> GetContractsByNameAsync(string? softComp, string? benefComp);

        Task<IEnumerable<Models.Domain.Contract>> GetContractsByBeneficiaryCompanyNameAsync(string beneficiaryCompanyName);


    }
}
