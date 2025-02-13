using AutoMapper;
using CrmPlatformAPI.Extensions;
using CrmPlatformAPI.Helpers;
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
        //paginare

        [HttpGet("ByUserName")]
        public async Task<IActionResult> GetTicketsByUserName([FromQuery] TicketParams _ticketParams)
        {
            var tickets = await _repositoryTicket.GetByUserNameAsync(_ticketParams);
            Response.AddPagination(tickets);

            var ticketDtos = _mapper.Map<IEnumerable<TicketDTO>>(tickets);
            return Ok(ticketDtos);
        }


        [HttpGet("FeedbackByUserName/{username}")]
        public async Task<IActionResult> GetFeedbackTicketByUserName(string username)
        {
            var tickets = await _repositoryTicket.GetFeedbackTicketByUserNameAsync(username);
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

        [HttpPut("UpdateTicketTranslation/{id}")]
        public async Task<IActionResult> UpdateTicketTranslation(int id, [FromBody] UpdateTranslationDTO updateTranslationDto)
        {
            try
            {
                var ticket = await _repositoryTicket.GetByIdAsync(id);

                if (ticket == null)
                {
                    return NotFound(new { message = $"Ticket with ID {id} not found." });
                }

                ticket.TLanguage = updateTranslationDto.Language;
                ticket.TLanguageCode = updateTranslationDto.LanguageCode;
                ticket.TCountryCode = updateTranslationDto.CountryCode;

                await _repositoryTicket.UpdateAsync(ticket);

                return Ok(new { message = "Ticket translation details updated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to update ticket translation.", error = ex.Message });
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
        public async Task<IActionResult> GetTicketsByContractId(int contractId, [FromQuery] TicketContractsParams ticketContractsParams)
        {
            try
            {
                var tickets = await _repositoryTicket.GetByContractIdAsync(contractId, ticketContractsParams);

                // Adds pagination headers to the response (assuming you have an extension method for that)
                Response.AddPagination(tickets);

                var ticketDtos = _mapper.Map<IEnumerable<TicketDTO>>(tickets);
                return Ok(ticketDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching tickets.", error = ex.Message });
            }
        }



        [HttpPost("AddStatusHistory")]
        public async Task<IActionResult> AddTicketStatusHistory(int ticketId, [FromBody] TicketStatusHistoryDTO dto)
        {
            try
            {
                if (dto == null)
                {
                    return BadRequest(new { message = "Invalid request data." });
                }

                await _repositoryTicketStatusHistory.AddHistoryAsync(ticketId, dto);

                return Ok(new { message = "Ticket status history added successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to add ticket status history.", error = ex.Message });
            }
        }

        [HttpPut("UpdateTDescription")]
        public async Task<IActionResult> UpdateTDescription([FromBody] UpdateTicketDescriptionDTO updateDto)
        {
            try
            {
                // Validate the incoming DTO
                if (updateDto == null || updateDto.Id <= 0 || string.IsNullOrWhiteSpace(updateDto.TDescription))
                {
                    return BadRequest(new { message = "Invalid data. Ensure ID and description are provided." });
                }

                // Retrieve the ticket from the database
                var ticket = await _repositoryTicket.GetByIdAsync(updateDto.Id);

                if (ticket == null)
                {
                    return NotFound(new { message = $"Ticket with ID {updateDto.Id} not found." });
                }

                // Update the ticket description
                ticket.TDescription = updateDto.TDescription;

                // Save changes to the database
                await _repositoryTicket.UpdateAsync(ticket);

                return Ok(new { message = "Ticket description updated successfully." });
            }
            catch (Exception ex)
            {
                // Handle errors and return a 500 response if needed
                return StatusCode(500, new { message = "Failed to update ticket description.", error = ex.Message });
            }
        }

        [HttpGet("GroupedTicketsBySoftwareCompany/{username}")]
        public async Task<IActionResult> GetGroupedTicketsBySoftwareCompany(string username)
        {
            var ticketsData = await _repositoryTicket.GetTicketsGroupedBySoftwareCompanyAsync(username);

            if (!ticketsData.Any())
            {
                return Ok(new { message = $"No tickets found for user {username}.", data = new List<object>() });
            }

            return Ok(new { message = "Tickets retrieved successfully.", data = ticketsData });
        }

        [HttpGet("GroupedTicketsByBeneficiaryCompany/{username}")]
        public async Task<IActionResult> GetTicketsGroupedByBeneficiaryCompany(string username)
        {
            var groupedTickets = await _repositoryTicket.GetTicketsGroupedByBeneficiaryCompanyAsync(username);

            if (groupedTickets == null || !groupedTickets.Any())
            {
                return Ok(new List<object>()); // ✅ Returns an empty list instead of a message
            }

            return Ok(groupedTickets); // ✅ Returns only the ticket data
        }



        [HttpGet("GroupedTicketsByContract/{username}")]
        public async Task<IActionResult> GetTicketsGroupedByContract(string username)
        {
            var groupedTickets = await _repositoryTicket.GetTicketsGroupedByContractAsync(username);

            if (groupedTickets == null || !groupedTickets.Any())
            {
                return Ok(new List<object>()); // Return an empty list instead of 404
            }

            return Ok(groupedTickets);
        }



        [HttpGet("GroupedTicketsByUserStatus/{username}")]
        public async Task<IActionResult> GetGroupedTicketsByUserStatus(string username)
        {
            var ticketsData = await _repositoryTicket.GetTicketsGroupedByUserStatusAsync(username);

            if (ticketsData == null || !ticketsData.Any())
            {
                return Ok(new List<object>()); // Return an empty list instead of a wrapped object
            }

            return Ok(ticketsData);
        }



    }

}
