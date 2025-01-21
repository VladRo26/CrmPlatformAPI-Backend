using AutoMapper;
using CrmPlatformAPI.Helpers.Enums;
using CrmPlatformAPI.Models.Domain;
using CrmPlatformAPI.Models.DTO;
using CrmPlatformAPI.Repositories.Implementation;
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
        private readonly IRepositoryTicketStatusHistory _repositoryTicketStatusHistory;
        private readonly IMapper _mapper;


        public TicketController(IRepositoryTicket repositoryTicket, IMapper mapper, IRepositoryTicketStatusHistory repositoryTicketStatusHistory)
        {
            _repositoryTicket = repositoryTicket;
            _mapper = mapper;
            _repositoryTicketStatusHistory = repositoryTicketStatusHistory;

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

        [HttpPost("CreateTicket")]
        public async Task<IActionResult> CreateTicket([FromBody] CreateTicketDTO createTicketDto)
        {
            try
            {
                if (createTicketDto == null)
                {
                    return BadRequest(new { message = "Ticket data must be provided." });
                }

                // Map the DTO to the domain model
                var ticket = _mapper.Map<Ticket>(createTicketDto);

                // Ensure HandlerId is null
                ticket.HandlerId = null;

                // Set additional fields if needed (e.g., timestamps)
                ticket.CreatedAt = DateTime.Now;

                // Add the ticket to the repository
                await _repositoryTicket.AddAsync(ticket);

                // Return the created ticket
                var ticketDto = _mapper.Map<TicketDTO>(ticket);
                return CreatedAtAction(nameof(GetTicketById), new { id = ticketDto.Id }, ticketDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to create ticket.", error = ex.Message });
            }
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

        [HttpGet("GetHistoryByTicketId/{id}")]
        public async Task<IActionResult> GetHistoryByTicketId(int id)
        {
            try
            {
                var history = await _repositoryTicketStatusHistory.GetHistoryByTicketIdAsync(id);
                if (!history.Any())
                {
                    return NotFound(new { message = $"No history found for Ticket with ID {id}." });
                }

                var historyDtos = _mapper.Map<IEnumerable<TicketStatusHistoryDTO>>(history);
                return Ok(historyDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to retrieve ticket history.", error = ex.Message });
            }
        }

        [HttpGet("TicketPerformance/{username}")]
        public async Task<IActionResult> TicketPerformance(string username)
        {
            try
            {
                // Fetch tickets where the user is the handler
                var tickets = await _repositoryTicket.GetByHandlerUsernameAsync(username);

                if (tickets == null || !tickets.Any())
                {
                    return Ok(new
                    {
                        username,
                        totalTickets = 0,
                        resolvedTickets = 0,
                        unresolvedTickets = 0,
                        ticketsByPriority = new Dictionary<string, int>()
                    });
                }

                // Calculate performance metrics
                var totalTickets = tickets.Count();
                var resolvedTickets = tickets.Count(t => t.Status == TicketStatus.Resolved);
                var unresolvedTickets = tickets.Count(t => t.Status != TicketStatus.Resolved);

                // Group tickets by priority
                var ticketsByPriority = tickets
                    .GroupBy(t => t.Priority.ToString())
                    .ToDictionary(g => g.Key, g => g.Count());

                return Ok(new
                {
                    username,
                    totalTickets,
                    resolvedTickets,
                    unresolvedTickets,
                    ticketsByPriority
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to calculate ticket performance.", error = ex.Message });
            }
        }

        [HttpPut("TakeOver/{ticketId}")]
        public async Task<IActionResult> TakeOverTicket(int ticketId, [FromQuery] int handlerId)
        {
            try
            {
                var result = await _repositoryTicket.TakeOverTicketAsync(ticketId, handlerId);
                if (result)
                {
                    return Ok(new { message = $"Ticket {ticketId} successfully taken over by handler {handlerId}." });
                }

                return BadRequest(new { message = "Failed to take over the ticket." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while taking over the ticket.", error = ex.Message });
            }
        }

        [HttpGet("ByContract/{contractId:int}")]
        public async Task<IActionResult> GetTicketsByContractId(int contractId)
        {
            try
            {
                var tickets = await _repositoryTicket.GetByContractIdAsync(contractId);

                // Return an empty list if no tickets are found
                if (tickets == null || !tickets.Any())
                {
                    return Ok(new List<TicketDTO>()); // Return an empty list
                }

                var ticketDtos = _mapper.Map<IEnumerable<TicketDTO>>(tickets);
                return Ok(ticketDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching tickets.", error = ex.Message });
            }
        }
    }

}
