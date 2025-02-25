using CrmPlatformAPI.Models.Domain;

namespace CrmPlatformAPI.Repositories.Interface
{
    public interface IRepositoryContract
    {

        Task<Contract> CreateAsync(Contract contract,string name1,string name2);

        Task<IEnumerable<Contract>> GetContractsAsync();

        Task<IEnumerable<Contract>> GetContractsByNameAsync(string? softComp, string? benefComp);

        Task<IEnumerable<Models.Domain.Contract>> GetContractsByBeneficiaryCompanyNameAsync(string beneficiaryCompanyName);

        Task<IEnumerable<Models.Domain.Contract>> GetContractsBySoftwareCompanyNameAsync(string softwareCompanyName);

        Task<Models.Domain.Contract?> GetContractByTicketIdAsync(int ticketId);

        Task<int> CountContractsAsync();


        Task<Contract?> UpdateContractStatusAsync(int contractId, float newStatus);
        Task<Contract?> UpdateContractAsync(Contract updatedContract);

        Task<Models.Domain.Contract?> GetContractByIdAsync(int id);




    }
}
