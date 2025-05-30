using CrmPlatformAPI.Models.Domain;

namespace CrmPlatformAPI.Repositories.Interface
{
    public interface IRepositoryTicketStatusAttachment
    {
        Task<IEnumerable<TicketStatusAttachment>> GetByStatusHistoryIdAsync(int statusHistoryId);

    }
}
