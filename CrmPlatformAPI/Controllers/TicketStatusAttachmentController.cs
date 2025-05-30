using AutoMapper;
using CrmPlatformAPI.Models.Domain;
using CrmPlatformAPI.Models.DTO;
using CrmPlatformAPI.Repositories.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CrmPlatformAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketStatusAttachmentController : Controller
    {
        private readonly IRepositoryTicketStatusAttachment _statusAttachmentRepo;
        private readonly IMapper _mapper;

        public TicketStatusAttachmentController(
            IRepositoryTicketStatusAttachment statusAttachmentRepo,
            IMapper mapper)
        {
            _statusAttachmentRepo = statusAttachmentRepo;
            _mapper = mapper;
        }

        [HttpGet("{statusHistoryId}")]
        [Authorize(Policy = "RequireUserRole")]
        public async Task<IActionResult> GetStatusAttachments(int statusHistoryId)
        {
            var attachments = await _statusAttachmentRepo.GetByStatusHistoryIdAsync(statusHistoryId);

            var responseDtos = _mapper.Map<IEnumerable<TicketAttachmentDTO>>(attachments);
            return Ok(responseDtos);
        }
    }
}
