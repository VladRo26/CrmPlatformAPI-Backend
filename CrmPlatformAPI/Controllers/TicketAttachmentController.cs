using AutoMapper;
using CrmPlatformAPI.Data;
using CrmPlatformAPI.Models.Domain;
using CrmPlatformAPI.Models.DTO;
using CrmPlatformAPI.Repositories.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CrmPlatformAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketAttachmentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileService _fileService;
        private readonly IMapper _mapper;


        public TicketAttachmentController(ApplicationDbContext context, IFileService fileService, IMapper mapper)
        {
            _context = context;
            _fileService = fileService;
            _mapper = mapper;
        }

        [HttpPost("{ticketId}")]
        //[Authorize(Policy = "RequireUserRole")]
        public async Task<IActionResult> UploadAttachment(int ticketId, [FromForm] CreateTicketAttachmentDTO dto)
        {
            if (dto.File == null || dto.File.Length == 0)
                return BadRequest("No file provided.");

            var ticket = await _context.Tickets.FindAsync(ticketId);
            if (ticket == null)
                return NotFound("Ticket not found.");

            var result = await _fileService.UploadFileAsync(dto.File);

            var attachment = new TicketAttachment
            {
                TicketId = ticketId,
                FileName = dto.File.FileName,
                FileType = dto.File.ContentType,
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };

            _context.TicketAttachments.Add(attachment);
            await _context.SaveChangesAsync();

            var responseDto = _mapper.Map<TicketAttachmentDTO>(attachment);
            return Ok(responseDto);
        }

        [HttpGet("{ticketId}")]
        //[Authorize(Policy = "RequireUserRole")]
        public async Task<IActionResult> GetAttachments(int ticketId)
        {
            var attachments = await _context.TicketAttachments
                .Where(a => a.TicketId == ticketId)
                .ToListAsync();

            var responseDtos = _mapper.Map<IEnumerable<TicketAttachmentDTO>>(attachments);
            return Ok(responseDtos);
        }

        [HttpDelete("delete/{attachmentId}")]
        //[Authorize(Policy = "RequireUserRole")]
        public async Task<IActionResult> DeleteAttachment(int attachmentId)
        {
            var attachment = await _context.TicketAttachments.FindAsync(attachmentId);
            if (attachment == null)
                return NotFound();

            await _fileService.DeleteFileAsync(attachment.PublicId);

            _context.TicketAttachments.Remove(attachment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }

}
