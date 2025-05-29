using CrmPlatformAPI.Models.Domain;

namespace CrmPlatformAPI.Repositories.Interface
{
    public interface IRepositoryTicketAttachment
    {
        Task AddAttachmentsAsync(int ticketId, IFormFileCollection attachments);

        Task<IEnumerable<TicketAttachment>> GetAttachmentsByTicketIdAsync(int ticketId);
    }
}
