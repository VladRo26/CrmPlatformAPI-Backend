using CrmPlatformAPI.Helpers;
using CrmPlatformAPI.Models.Domain;
using System.Threading.Tasks;

namespace CrmPlatformAPI.Repositories.Interface
{
    public interface IRepositoryTicket
    {
        Task<IEnumerable<Ticket>> GetAllAsync();

        Task<Ticket> GetByIdAsync(int id);

        Task<IEnumerable<Ticket>> GetByUserIdAsync(int userId);

        Task<IEnumerable<Ticket>> GetByStatusAsync(string status);

        Task<IEnumerable<Ticket>> GetByCompanyAsync(string name);


        Task<IEnumerable<Ticket>> GetByTitleAsync(string name);

        Task<IEnumerable<Ticket>> GetByPriorityAsync(string description);

        Task<IEnumerable<Ticket>> GetByHandlerUsernameAsync(string handlerUsername);


        Task<string> GenerateSummaryForTicketAsync(int ticketId);

        Task<string> TranslateDescriptionForTicketAsync(int ticketId, string sourceLanguage, string targetLanguage);

        Task AddAsync(Ticket ticket);
        Task<bool> TakeOverTicketAsync(int ticketId, int handlerId);

        Task<bool> UpdateAsync(Ticket ticket);

        Task<IEnumerable<Ticket>> GetByContractIdAsync(int contractId);

        Task<IEnumerable<Ticket>> GetFeedbackTicketByUserNameAsync(string username);
        Task<PagedList<Ticket>> GetByUserNameAsync(TicketParams ticketParams);

        Task<IEnumerable<object>> GetTicketsGroupedBySoftwareCompanyAsync(string username);

        Task<IEnumerable<object>> GetTicketsGroupedByBeneficiaryCompanyAsync(string username);

        Task<IEnumerable<object>> GetTicketsGroupedByContractAsync(string username);

        Task<IEnumerable<object>> GetTicketsGroupedByUserStatusAsync(string username);

    }
}
