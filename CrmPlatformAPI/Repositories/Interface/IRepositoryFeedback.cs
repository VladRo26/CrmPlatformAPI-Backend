using CrmPlatformAPI.Models.Domain;

namespace CrmPlatformAPI.Repositories.Interface
{
    public interface IRepositoryFeedback
    {
        Task<IEnumerable<Feedback>> GetByFromUserIdAsync(int fromUserId);
        Task<IEnumerable<Feedback>> GetByToUserIdAsync(int toUserId);
        Task<IEnumerable<Feedback>> GetByTicketIdAsync(int ticketId);



    }
}
