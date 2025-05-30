using CrmPlatformAPI.Data;
using CrmPlatformAPI.Models.Domain;
using CrmPlatformAPI.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace CrmPlatformAPI.Repositories.Implementation
{
    public class RepositoryTicketStatusAttachment : IRepositoryTicketStatusAttachment
    {
        private readonly ApplicationDbContext _context;

        public RepositoryTicketStatusAttachment(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TicketStatusAttachment>> GetByStatusHistoryIdAsync(int statusHistoryId)
        {
            return await _context.TicketStatusAttachments
                .Where(a => a.TicketStatusHistoryId == statusHistoryId)
                .ToListAsync();
        }
    }

}


