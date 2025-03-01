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

        private readonly IEmailService _emailService;

        public RepositoryTicketStatusHistory(ApplicationDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
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

            // Find the user who updated the ticket
            var updatedByUser = (ticket.Creator?.UserName == dto.UpdatedByUsername) ? ticket.Creator
                               : (ticket.Handler?.UserName == dto.UpdatedByUsername) ? ticket.Handler
                               : null;

            if (updatedByUser == null)
            {
                throw new Exception($"User '{dto.UpdatedByUsername}' is not associated with the ticket as Creator or Handler.");
            }

            // Validate and convert TicketUserRole
            if (!Enum.TryParse<TicketUserRole>(dto.TicketUserRole, true, out var ticketUserRole))
            {
                throw new Exception("Invalid role for TicketUserRole.");
            }

            // Validate and convert Status
            if (!Enum.TryParse<TicketStatus>(dto.Status, true, out var parsedStatus))
            {
                throw new Exception("Invalid status.");
            }

            // Determine the other user (recipient of the email)
            var recipient = (updatedByUser.Id == ticket.CreatorId) ? ticket.Handler : ticket.Creator;

            if (recipient == null)
            {
                throw new Exception("No other user found on the ticket to notify.");
            }

            // Create TicketStatusHistory record
            var history = new TicketStatusHistory
            {
                TicketId = ticketId,
                Status = parsedStatus,
                Message = dto.Message,
                UpdatedAt = dto.UpdatedAt != default ? dto.UpdatedAt : DateTime.UtcNow,
                UpdatedByUserId = updatedByUser.Id,
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

                // 📧 Send Email Notification to the Other User
                string subject = $"[Ticket #{ticketId}] Status Updated to {parsedStatus}";
                string message = $@"
            Hello {recipient.FirstName},<br><br>
            The status of the ticket <b>{ticket.Title}</b> has been updated.<br>
            <b>New Status:</b> {parsedStatus} <br>
            <b>Updated By:</b> {updatedByUser.FirstName} {updatedByUser.LastName} <br>
            <b>Message:</b> {dto.Message} <br><br>
            Please check the ticket for more details.<br><br>
            Regards,<br>
            CRM Support Team
        ";

                await _emailService.SendEmailAsync(recipient.Email, subject, message);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the ticket status and sending the notification email.", ex);
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
