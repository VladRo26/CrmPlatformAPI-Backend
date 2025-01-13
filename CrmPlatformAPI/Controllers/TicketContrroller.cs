using CrmPlatformAPI.Repositories.Interface;
using Microsoft.AspNetCore.Mvc;

namespace CrmPlatformAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class TicketContrroller : Controller
    {
        private readonly IRepositoryTicket _repositoryTicket;

        public TicketContrroller(IRepositoryTicket repositoryTicket)
        {
            _repositoryTicket = repositoryTicket;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTickets()
        {
            var tickets = await _repositoryTicket.GetAllAsync();
            return Ok(tickets);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetTicketById(int id)
        {
            var ticket = await _repositoryTicket.GetByIdAsync(id);
            if (ticket == null)
            {
                return NotFound(new { message = $"Ticket with ID {id} not found." });
            }

            return Ok(ticket);
        }

        [HttpGet("ByUser/{userId:int}")]
        public async Task<IActionResult> GetTicketsByUserId(int userId)
        {
            var tickets = await _repositoryTicket.GetByUserIdAsync(userId);
            return Ok(tickets);
        }

        [HttpGet("ByTitle/{title}")]
        public async Task<IActionResult> GetTicketsByTitle(string title)
        {
            var tickets = await _repositoryTicket.GetByTitleAsync(title);
            return Ok(tickets);
        }

        [HttpGet("ByCompany/{name}")]
        public async Task<IActionResult> GetTicketsByCompany(string name)
        {
            var tickets = await _repositoryTicket.GetByCompanyAsync(name);
            return Ok(tickets);
        }

        [HttpGet("ByUserName/{username}")]
        public async Task<IActionResult> GetTicketsByUserName(string username)
        {
            var tickets = await _repositoryTicket.GetByUserNameAsync(username);
            return Ok(tickets);
        }

        [HttpGet("ByHandlerUsername/{username}")]
        public async Task<IActionResult> GetTicketsByHandlerUsername(string username)
        {
            var tickets = await _repositoryTicket.GetByHandlerUsernameAsync(username);

            if (tickets == null || !tickets.Any())
            {
                return NotFound(new { message = $"No tickets found for handler username '{username}'." });
            }

            return Ok(tickets);
        }
    }
}
