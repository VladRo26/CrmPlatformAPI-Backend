using CrmPlatformAPI.Data;
using CrmPlatformAPI.Helpers.Enums;
using CrmPlatformAPI.Models.Domain;
using CrmPlatformAPI.Models.DTO;
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

        public async Task AddHistoryAsync(int ticketId, TicketStatusHistoryDTO dto)
        {
            if (_context == null)
            {
                throw new Exception("Database context is not initialized.");
            }

            // Fetch the ticket and user
            var ticket = await _context.Tickets
                .Include(t => t.Creator)
                .Include(t => t.Handler)
                .FirstOrDefaultAsync(t => t.Id == ticketId);

            if (ticket == null)
            {
                throw new Exception($"Ticket with ID {ticketId} not found.");
            }

            var user = ticket.Creator?.UserName == dto.UpdatedByUsername ? ticket.Creator
                       : ticket.Handler?.UserName == dto.UpdatedByUsername ? ticket.Handler
                       : null;

            if (user == null)
            {
                throw new Exception($"User '{dto.UpdatedByUsername}' is not associated with the ticket as Creator or Handler.");
            }

            // Validate and map the role
            if (!Enum.TryParse<TicketUserRole>(dto.TicketUserRole, true, out var ticketUserRole))
            {
                throw new Exception("Invalid role for TicketUserRole.");
            }

            // Create the TicketStatusHistory record
            var history = new TicketStatusHistory
            {
                TicketId = ticketId,
                Status = Enum.TryParse<TicketStatus>(dto.Status, true, out var parsedStatus) ? parsedStatus : throw new Exception("Invalid status."),
                Message = dto.Message,
                UpdatedAt = dto.UpdatedAt != default ? dto.UpdatedAt : DateTime.UtcNow,
                UpdatedByUserId = user.Id,
                TicketUserRole = ticketUserRole
            };

            try
            {
                await _context.TicketStatusHistories.AddAsync(history);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the ticket status history.", ex);
            }
        }

    }
}
