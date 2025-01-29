using AutoMapper;
using CrmPlatformAPI.Data;
using CrmPlatformAPI.Models.Domain;
using CrmPlatformAPI.Models.DTO;
using CrmPlatformAPI.Repositories.Implementation;
using CrmPlatformAPI.Repositories.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.EntityFrameworkCore;

namespace CrmPlatformAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IRepositorySentimentAnalysis _sentimentAnalysisRepository;
        private readonly IRepositoryFeedbackSentiment _feedbackSentimentRepository;
        private readonly IRepositoryLLM _repositoryLLM;
        private readonly IRepositoryUser _userRepository;
        private readonly IRepositoryTicket _ticketRepository;
        private readonly IMapper _mapper;


        public FeedbackController(ApplicationDbContext context, IMapper mapper,
            IRepositorySentimentAnalysis repositorySentimentAnalysis,
            IRepositoryFeedbackSentiment repositoryFeedbackSentiment, IRepositoryLLM repositoryLLM,IRepositoryTicket repositoryTicket, IRepositoryUser repositoryUser)
        {
            _context = context;
            _mapper = mapper;
            _sentimentAnalysisRepository = repositorySentimentAnalysis;
            _feedbackSentimentRepository = repositoryFeedbackSentiment;
            _repositoryLLM = repositoryLLM;
            _userRepository = repositoryUser;
            _ticketRepository = repositoryTicket;

        }

        [HttpGet("to-user/{toUserId}")]
        public async Task<ActionResult<IEnumerable<FeedbackDTO>>> GetFeedbackByToUserId(int toUserId)
        {
            if (_context == null)
            {
                return Ok(new List<FeedbackDTO>()); // Return an empty list if context is null
            }

            var feedbacks = await _context.Feedbacks
                .Include(f => f.FromUser) // Include FromUser navigation property
                .Include(f => f.ToUser)   // Include ToUser navigation property
                .Include(f => f.Ticket)   // Include Ticket navigation property
                .Where(f => f.ToUserId == toUserId)
                .ToListAsync();

            // Use AutoMapper to map domain models to DTOs
            var feedbackDTOs = _mapper.Map<IEnumerable<FeedbackDTO>>(feedbacks);

            return Ok(feedbackDTOs);
        }

        [HttpPost]
        public async Task<ActionResult<FeedbackDTO>> CreateFeedback(string username, int ticketId, string content, int rating)
        {
            // Get the FromUserId based on the username
            var fromUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);
            if (fromUser == null) return BadRequest("Invalid username.");

            // Get the ticket and ToUserId (HandlerId)
            var ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.Id == ticketId);
            if (ticket == null) return BadRequest("Invalid ticket ID.");
            if (ticket.HandlerId == null) return BadRequest("Ticket does not have a handler.");

            var toUserId = ticket.HandlerId.Value;

            // Create the feedback
            var feedback = new Feedback
            {
                FromUserId = fromUser.Id,
                ToUserId = toUserId,
                TicketId = ticketId,
                Content = content,
                Rating = rating,
                CreatedAt = DateTime.UtcNow
            };

            _context.Feedbacks.Add(feedback);
            await _context.SaveChangesAsync();

            // Call sentiment analysis and save the results
            var sentimentResponse = await _sentimentAnalysisRepository.AnalyzeSentimentAsync(content);
            var sentiment = new FeedBackSentiment
            {
                FeedbackId = feedback.Id,
                Positive = sentimentResponse.Scores[0],
                Neutral = sentimentResponse.Scores[1],
                Negative = sentimentResponse.Scores[2]
            };

            await _feedbackSentimentRepository.AddSentimentAsync(sentiment);

            var feedbackDto = _mapper.Map<FeedbackDTO>(feedback);
            return CreatedAtAction(nameof(GetFeedbackByToUserId), new { toUserId = feedback.ToUserId }, feedbackDto);
        }

        [HttpGet("sentiment/{feedbackId}")]
        public async Task<ActionResult<FeedBackSentimentDTO>> GetSentimentByFeedbackId(int feedbackId)
        {
            // Retrieve sentiment from the repository
            var sentiment = await _feedbackSentimentRepository.GetSentimentByFeedbackIdAsync(feedbackId);

            // If not found, return 404
            if (sentiment == null)
            {
                return NotFound($"No sentiment analysis found for Feedback ID {feedbackId}.");
            }

            // Map to DTO and return
            var sentimentDto = _mapper.Map<FeedBackSentimentDTO>(sentiment);
            return Ok(sentimentDto);
        }

        [HttpPost("generate-feedback")]
        public async Task<ActionResult<string>> GenerateFeedbackForUser(
        string username, int ticketId, int rating, [FromBody] string userExperience)
        {
            // Validate the rating (must be between 1 and 5)
            if (rating < 1 || rating > 5)
            {
                return BadRequest("Rating must be between 1 and 5.");
            }

            // Retrieve the user from the repository
            var user = await _userRepository.GetByUserNameAsync(username);
            if (user == null) return BadRequest("Invalid username.");

            // Retrieve the ticket from the repository
            var ticket = await _ticketRepository.GetByIdAsync(ticketId);
            if (ticket == null) return BadRequest("Invalid ticket ID.");

            // Ensure the ticket has a description
            if (string.IsNullOrWhiteSpace(ticket.Description))
            {
                return BadRequest("Ticket description is required to generate feedback.");
            }

            // Construct the LLM prompt
            var prompt = $@"
        Generate professional and concise feedback based on the following details:
        - **User Experience:** {userExperience}
        - **Ticket Description:** {ticket.Description}
        - **Rating (1-5):** {rating}
        - **Username:** {user.UserName}

        Ensure:
        - **Positive and appreciative feedback** for high ratings (4-5).
        - **Constructive feedback with suggestions** for a neutral rating (3).
        - **Polite but critical feedback** for low ratings (1-2).
        - **Clear and concise wording**, avoiding unnecessary details.

        **Return only the generated feedback as text. No formatting needed.**
    ";

            try
            {
                // Call the LLM service and get the response
                var llmResponse = await _repositoryLLM.GenerateResponseAsync(prompt);

                // Extract the generated feedback text
                string generatedFeedback = llmResponse.Response ?? "No feedback generated.";

                return Ok(generatedFeedback);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error generating feedback: {ex.Message}");
            }
        }
    }
}
