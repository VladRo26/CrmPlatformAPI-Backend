using CrmPlatformAPI.Models.Domain;

namespace CrmPlatformAPI.Repositories.Interface
{
    public interface IRepositoryTicketStatusHistory
    {
        Task<IEnumerable<TicketStatusHistory>> GetTicketStatusHistoryAsync();
        
    }
}
