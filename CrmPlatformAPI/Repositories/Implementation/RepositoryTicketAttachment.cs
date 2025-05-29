using CrmPlatformAPI.Data;
using CrmPlatformAPI.Models.Domain;
using CrmPlatformAPI.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace CrmPlatformAPI.Repositories.Implementation
{

    public class RepositoryTicketAttachment : IRepositoryTicketAttachment
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileService _fileService;

        public RepositoryTicketAttachment(ApplicationDbContext context, IFileService fileService)
        {
            _context = context;
            _fileService = fileService;
        }

        public async Task AddAttachmentsAsync(int ticketId, IFormFileCollection attachments)
        {
            foreach (var file in attachments)
            {
                var result = await _fileService.UploadFileAsync(file);

                var attachment = new TicketAttachment
                {
                    TicketId = ticketId,
                    FileName = file.FileName,
                    FileType = file.ContentType,
                    Url = result.SecureUrl.AbsoluteUri,
                    PublicId = result.PublicId,
                    UploadedAt = DateTime.UtcNow
                };

                _context.TicketAttachments.Add(attachment);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<TicketAttachment>> GetAttachmentsByTicketIdAsync(int ticketId)
        {
            return await _context.TicketAttachments
                                 .Where(a => a.TicketId == ticketId)
                                 .ToListAsync();
        }

    }

}
