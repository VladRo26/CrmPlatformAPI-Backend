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

            // Validate and parse status
            if (!Enum.TryParse<TicketStatus>(dto.Status, true, out var parsedStatus))
            {
                throw new Exception("Invalid status.");
            }

            // Determine the unseen user
            int unseenUserId = (int)(ticket.CreatorId == user.Id ? ticket.HandlerId ?? 0 : ticket.CreatorId);

            // Create the TicketStatusHistory record
            var history = new TicketStatusHistory
            {
                TicketId = ticketId,
                Status = parsedStatus,
                Message = dto.Message,
                UpdatedAt = dto.UpdatedAt != default ? dto.UpdatedAt : DateTime.UtcNow,
                UpdatedByUserId = user.Id,
                TicketUserRole = ticketUserRole,
                Seen = false // The other user hasn't seen this update yet
            };

            try
            {
                // Add the status history entry
                await _context.TicketStatusHistories.AddAsync(history);

                // Update the ticket status to match the latest history status
                ticket.Status = parsedStatus;
                _context.Tickets.Update(ticket);

                // Save changes
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the ticket status history and updating the ticket status.", ex);
            }
        }


        public async Task<IEnumerable<TicketStatusHistory>> GetLastTicketStatusHistoryByUserAsync(string username, int count)
        {
            if (_context == null)
            {
                return Enumerable.Empty<TicketStatusHistory>();
            }

            // Include Ticket with its Creator and Handler for filtering
            return await _context.TicketStatusHistories
                .Include(h => h.Ticket)
                    .ThenInclude(t => t.Creator)
                .Include(h => h.Ticket)
                    .ThenInclude(t => t.Handler)
                .Where(h => h.Ticket.Creator.UserName == username
                         || (h.Ticket.Handler != null && h.Ticket.Handler.UserName == username))
                .OrderByDescending(h => h.UpdatedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task MarkStatusAsSeenAsync(string message, DateTime updatedAt, string updatedByUsername)
        {
            var history = await _context.TicketStatusHistories
                .FirstOrDefaultAsync(h => h.Message == message
                    && h.UpdatedAt == updatedAt
                    && h.UpdatedByUser.UserName == updatedByUsername);

            if (history != null)
            {
                history.Seen = true;
                await _context.SaveChangesAsync();
            }
        }



    }
}
