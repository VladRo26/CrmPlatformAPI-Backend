using CrmPlatformAPI.Models.Domain;

namespace CrmPlatformAPI.Repositories.Interface
{
    public interface IRepositoryTicket
    {
        Task<IEnumerable<Ticket>> GetAllAsync();

        Task<Ticket> GetByIdAsync(int id);

        Task<IEnumerable<Ticket>> GetByUserIdAsync(int userId);

        Task<IEnumerable<Ticket>> GetByStatusAsync(string status);

        Task<IEnumerable<Ticket>> GetByCompanyAsync(string name);

        Task<IEnumerable<Ticket>> GetByUserNameAsync(string username);

        Task<IEnumerable<Ticket>> GetByTitleAsync(string name);

        Task<IEnumerable<Ticket>> GetByPriorityAsync(string description);

        Task<IEnumerable<Ticket>> GetByHandlerUsernameAsync(string handlerUsername);


        Task<string> GenerateSummaryForTicketAsync(int ticketId, string model, int maxTokens);


    }
}
