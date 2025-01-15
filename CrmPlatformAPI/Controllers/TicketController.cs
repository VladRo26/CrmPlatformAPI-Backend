using AutoMapper;
using CrmPlatformAPI.Models.DTO;
using CrmPlatformAPI.Repositories.Interface;
using Microsoft.AspNetCore.Mvc;

namespace CrmPlatformAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class TicketController : Controller
    {
        private readonly IRepositoryTicket _repositoryTicket;
        private readonly IRepositoryLLM _llmRepository;
        private readonly IMapper _mapper;


        public TicketController(IRepositoryTicket repositoryTicket, IMapper mapper)
        {
            _repositoryTicket = repositoryTicket;
            _mapper = mapper;

        }

        [HttpGet]
        public async Task<IActionResult> GetAllTickets()
        {
            var tickets = await _repositoryTicket.GetAllAsync();
            var ticketDtos = _mapper.Map<IEnumerable<TicketDTO>>(tickets);
            return Ok(ticketDtos);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetTicketById(int id)
        {
            var ticket = await _repositoryTicket.GetByIdAsync(id);
            if (ticket == null)
            {
                return NotFound(new { message = $"Ticket with ID {id} not found." });
            }

            var ticketDto = _mapper.Map<TicketDTO>(ticket);
            return Ok(ticketDto);
        }

        [HttpGet("ByUser/{userId:int}")]
        public async Task<IActionResult> GetTicketsByUserId(int userId)
        {
            var tickets = await _repositoryTicket.GetByUserIdAsync(userId);
            var ticketDtos = _mapper.Map<IEnumerable<TicketDTO>>(tickets);
            return Ok(ticketDtos);
        }

        [HttpGet("ByTitle/{title}")]
        public async Task<IActionResult> GetTicketsByTitle(string title)
        {
            var tickets = await _repositoryTicket.GetByTitleAsync(title);
            var ticketDtos = _mapper.Map<IEnumerable<TicketDTO>>(tickets);
            return Ok(ticketDtos);
        }

        [HttpGet("ByCompany/{name}")]
        public async Task<IActionResult> GetTicketsByCompany(string name)
        {
            var tickets = await _repositoryTicket.GetByCompanyAsync(name);
            var ticketDtos = _mapper.Map<IEnumerable<TicketDTO>>(tickets);
            return Ok(ticketDtos);
        }

        [HttpGet("ByUserName/{username}")]
        public async Task<IActionResult> GetTicketsByUserName(string username)
        {
            var tickets = await _repositoryTicket.GetByUserNameAsync(username);
            var ticketDtos = _mapper.Map<IEnumerable<TicketDTO>>(tickets);
            return Ok(ticketDtos);
        }

        [HttpGet("ByHandlerUsername/{username}")]
        public async Task<IActionResult> GetTicketsByHandlerUsername(string username)
        {
            var tickets = await _repositoryTicket.GetByHandlerUsernameAsync(username);

            if (tickets == null || !tickets.Any())
            {
                return Ok(new List<TicketDTO>()); // Return empty list instead of 404
            }

            var ticketDtos = _mapper.Map<IEnumerable<TicketDTO>>(tickets);
            return Ok(ticketDtos);
        }
        [HttpPost("GenerateSummary/{id}")]
        public async Task<IActionResult> GenerateSummary(int id)
        {
            try
            {
                // Generate the summary
                var summary = await _repositoryTicket.GenerateSummaryForTicketAsync(id);

                // Fetch the ticket for additional details (optional)
                var ticket = await _repositoryTicket.GetByIdAsync(id);

                return Ok(new
                {
                    ticketId = id,
                    summary
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to generate summary.", error = ex.Message });
            }
        }

        [HttpPost("TranslateDescription/{id}")]
        public async Task<IActionResult> TranslateDescription(int id, [FromQuery] string sourceLanguage, [FromQuery] string targetLanguage)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(sourceLanguage) || string.IsNullOrWhiteSpace(targetLanguage))
                {
                    return BadRequest(new { message = "Source and target languages must be provided." });
                }

                var translation = await _repositoryTicket.TranslateDescriptionForTicketAsync(id, sourceLanguage, targetLanguage);

                return Ok(new
                {
                    ticketId = id,
                    sourceLanguage,
                    targetLanguage,
                    translation
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to translate description.", error = ex.Message });
            }
        }

    }

}
