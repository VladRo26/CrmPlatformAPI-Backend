using CrmPlatformAPI.Data;
using CrmPlatformAPI.Helpers;
using CrmPlatformAPI.Helpers.Enums;
using CrmPlatformAPI.Models.Domain;
using CrmPlatformAPI.Models.DTO;
using CrmPlatformAPI.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace CrmPlatformAPI.Repositories.Implementation
{
    public class RepositoryTicketStatusHistory : IRepositoryTicketStatusHistory
    {
        private readonly ApplicationDbContext _context;

        private readonly IEmailService _emailService;
        private readonly FrontendSettings _frontendSettings;
        private readonly IFileService _fileService;


        public RepositoryTicketStatusHistory(ApplicationDbContext context, IEmailService emailService,
            IOptions<FrontendSettings> frontendSettings, IFileService fileService)
        {
            _context = context;
            _emailService = emailService;
            _frontendSettings = frontendSettings.Value;
            _fileService = fileService;
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

        public async Task AddHistoryAsync(int ticketId, TicketStatusHistoryDTO dto, IFormFileCollection? attachments = null)
        {
            if (_context == null)
                throw new Exception("Database context is not initialized.");

            // Fetch the ticket and user
            var ticket = await _context.Tickets
                .Include(t => t.Creator)
                .Include(t => t.Handler)
                .FirstOrDefaultAsync(t => t.Id == ticketId);

            if (ticket == null)
                throw new Exception($"Ticket with ID {ticketId} not found.");

            var updatedByUser = (ticket.Creator?.UserName == dto.UpdatedByUsername) ? ticket.Creator
                               : (ticket.Handler?.UserName == dto.UpdatedByUsername) ? ticket.Handler
                               : null;

            if (updatedByUser == null)
                throw new Exception($"User '{dto.UpdatedByUsername}' is not associated with the ticket as Creator or Handler.");

            if (!Enum.TryParse<TicketUserRole>(dto.TicketUserRole, true, out var ticketUserRole))
                throw new Exception("Invalid role for TicketUserRole.");

            if (!Enum.TryParse<TicketStatus>(dto.Status, true, out var parsedStatus))
                throw new Exception("Invalid status.");

            var recipient = (updatedByUser.Id == ticket.CreatorId) ? ticket.Handler : ticket.Creator;
            if (recipient == null)
                throw new Exception("No other user found on the ticket to notify.");

            var history = new TicketStatusHistory
            {
                TicketId = ticketId,
                Status = parsedStatus,
                Message = dto.Message,
                UpdatedAt = dto.UpdatedAt != default ? dto.UpdatedAt : DateTime.UtcNow,
                UpdatedByUserId = updatedByUser.Id,
                TicketUserRole = ticketUserRole,
                Seen = false
            };

            try
            {
                await _context.TicketStatusHistories.AddAsync(history);
                await _context.SaveChangesAsync(); // Required for history.Id

                // 🔗 Collect attachment links
                List<string> attachmentLinks = new();

                if (attachments != null && attachments.Count > 0)
                {
                    foreach (var file in attachments)
                    {
                        var result = await _fileService.UploadFileAsync(file);

                        var attachment = new TicketStatusAttachment
                        {
                            TicketStatusHistoryId = history.Id,
                            FileName = file.FileName,
                            FileType = file.ContentType,
                            Url = result.SecureUrl.AbsoluteUri,
                            PublicId = result.PublicId,
                            UploadedAt = DateTime.UtcNow
                        };

                        _context.TicketStatusAttachments.Add(attachment);

                        // 📎 Generate HTML link for email
                        string link = $@"<a href=""{attachment.Url}"">{attachment.FileName}</a>";
                        attachmentLinks.Add(link);
                    }

                    await _context.SaveChangesAsync();
                }

                // ✅ Update ticket status
                ticket.Status = parsedStatus;
                _context.Tickets.Update(ticket);
                await _context.SaveChangesAsync();

                // 📨 Email
                string subject = $"[Ticket #{ticketId}] Status Updated to {parsedStatus}";

                string attachmentSection = attachmentLinks.Any()
                    ? $"<b>Attachments:</b><br>{string.Join("<br>", attachmentLinks)}<br><br>"
                    : "";

                string message = $@"
            Hello {recipient.FirstName},<br><br>
            The status of the ticket <b>{ticket.Title}</b> has been updated.<br>
            <b>New Status:</b> {parsedStatus} <br>
            <b>Updated By:</b> {updatedByUser.FirstName} {updatedByUser.LastName} <br>
            <b>Message:</b> {dto.Message} <br><br>
            {attachmentSection}
            You can view the ticket <a href=""{_frontendSettings.BaseUrl}/tickets/{ticket.Id}"">here</a>.<br><br>
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

        public async Task AddAttachmentsToStatusAsync(int ticketStatusHistoryId, IFormFileCollection attachments)
        {
            foreach (var file in attachments)
            {
                var result = await _fileService.UploadFileAsync(file);

                var attachment = new TicketStatusAttachment
                {
                    TicketStatusHistoryId = ticketStatusHistoryId,
                    FileName = file.FileName,
                    FileType = file.ContentType,
                    Url = result.SecureUrl.AbsoluteUri,
                    PublicId = result.PublicId,
                    UploadedAt = DateTime.UtcNow
                };

                _context.TicketStatusAttachments.Add(attachment);
            }

            await _context.SaveChangesAsync();
        }


    }
}
