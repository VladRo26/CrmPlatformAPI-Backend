using CrmPlatformAPI.Data;
using CrmPlatformAPI.Models.Domain;
using CrmPlatformAPI.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace CrmPlatformAPI.Repositories.Implementation
{
    public class RepositoryTicketStatusHistory : IRepositoryTicketStatusHistory
    {
        private readonly ApplicationDbContext _context;

        public RepositoryTicketStatusHistory(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TicketStatusHistory>> GetHistoryByTicketIdAsync(int ticketId)
        {
            if (_context == null)
            {
                return Enumerable.Empty<TicketStatusHistory>();
            }

            return await _context.TicketStatusHistories
                .Include(h => h.Ticket)
                .Include(h => h.UpdatedByUser)
                .Where(h => h.TicketId == ticketId)
                .OrderByDescending(h => h.UpdatedAt) // Orders the results by UpdatedAt in ascending order
                .ToListAsync();
        }

    }
}
