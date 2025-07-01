using AutoMapper;
using CrmPlatformAPI.Data;
using CrmPlatformAPI.Models.Domain;
using CrmPlatformAPI.Models.DTO;
using CrmPlatformAPI.Repositories.Implementation;
using CrmPlatformAPI.Repositories.Interface;
using Microsoft.AspNetCore.Authorization;
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
            IRepositoryFeedbackSentiment repositoryFeedbackSentiment, IRepositoryLLM repositoryLLM, IRepositoryTicket repositoryTicket, IRepositoryUser repositoryUser)
        {
            _context = context;
            _mapper = mapper;
            _sentimentAnalysisRepository = repositorySentimentAnalysis;
            _feedbackSentimentRepository = repositoryFeedbackSentiment;
            _repositoryLLM = repositoryLLM;
            _userRepository = repositoryUser;
            _ticketRepository = repositoryTicket;

        }

        [Authorize(Policy = "RequireUserRole")]
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

        [Authorize(Policy = "RequireUserRole")]
        [HttpPost]
        public async Task<ActionResult<FeedbackDTO>> CreateFeedback(
      string username, int ticketId, string content, int rating)
        {
            // Get the FromUserId based on the username
            var fromUser = await _userRepository.GetByUserNameAsync(username);
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

            // Calculate new average rating for the handler
            var feedbacks = await _context.Feedbacks
                .Where(f => f.ToUserId == toUserId)
                .ToListAsync();

            var newAverageRating = (int)Math.Round(feedbacks.Average(f => f.Rating));

            // Update the handler's rating using RepositoryUser
            var handler = await _userRepository.GetByIdAsync(toUserId);
            if (handler != null)
            {
                handler.Rating = newAverageRating;
                await _userRepository.UpdateAsync(handler);
            }

            // Call sentiment analysis and save the results
            // Translate content to English before sentiment analysis
            var translatedContent = await _repositoryLLM.TranslateTextAsync(content, ticket.Language, "English");
            var sentimentResponse = await _sentimentAnalysisRepository.AnalyzeSentimentAsync(translatedContent);

            var positiveScore = sentimentResponse.Scores[sentimentResponse.Labels.FindIndex(l => l.Equals("positive", StringComparison.OrdinalIgnoreCase))];
            var neutralScore = sentimentResponse.Scores[sentimentResponse.Labels.FindIndex(l => l.Equals("neutral", StringComparison.OrdinalIgnoreCase))];
            var negativeScore = sentimentResponse.Scores[sentimentResponse.Labels.FindIndex(l => l.Equals("negative", StringComparison.OrdinalIgnoreCase))];

            var sentiment = new FeedBackSentiment
            {
                FeedbackId = feedback.Id,
                Positive = positiveScore,
                Neutral = neutralScore,
                Negative = negativeScore
            };


            await _feedbackSentimentRepository.AddSentimentAsync(sentiment);

            var feedbackDto = _mapper.Map<FeedbackDTO>(feedback);
            return CreatedAtAction(nameof(GetFeedbackByToUserId), new { toUserId = feedback.ToUserId }, feedbackDto);
        }

        [Authorize(Policy = "RequireUserRole")]
        [HttpPost("software-to-beneficiary")]
        public async Task<ActionResult<FeedbackDTO>> CreateFeedbackForBeneficiary(
    string username, int ticketId, string content, int rating)
        {
            var softwareUser = await _userRepository.GetByUserNameAsync(username);
            if (softwareUser == null) return BadRequest("Invalid username.");

            var ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.Id == ticketId);
            if (ticket == null) return BadRequest("Invalid ticket ID.");

            if (ticket.HandlerId != softwareUser.Id)
                return BadRequest("You are not the handler of this ticket.");

            var beneficiaryUser = await _userRepository.GetByIdAsync(ticket.CreatorId);
            if (beneficiaryUser == null) return BadRequest("Invalid ticket creator.");

            // ✅ Check if feedback already exists for this ticket and user
            bool feedbackExists = await _context.Feedbacks
                .AnyAsync(f => f.FromUserId == softwareUser.Id && f.TicketId == ticketId);

            if (feedbackExists)
                return BadRequest("You have already provided feedback for this ticket.");

            // Create the feedback
            var feedback = new Feedback
            {
                FromUserId = softwareUser.Id,  // Software user is the sender
                ToUserId = beneficiaryUser.Id, // Beneficiary user (creator) is the receiver
                TicketId = ticketId,
                Content = content,
                Rating = rating,
                CreatedAt = DateTime.UtcNow
            };

            _context.Feedbacks.Add(feedback);
            await _context.SaveChangesAsync();

            // Calculate new average rating for the Beneficiary User
            var feedbacks = await _context.Feedbacks
                .Where(f => f.ToUserId == beneficiaryUser.Id)
                .ToListAsync();

            var newAverageRating = (int)Math.Round(feedbacks.Average(f => f.Rating));

            // Update the Beneficiary User's rating
            beneficiaryUser.Rating = newAverageRating;
            await _userRepository.UpdateAsync(beneficiaryUser);

            // Perform sentiment analysis
            // Translate content to English before sentiment analysis
            var translatedContent = await _repositoryLLM.TranslateTextAsync(content, ticket.Language, "English");
            var sentimentResponse = await _sentimentAnalysisRepository.AnalyzeSentimentAsync(translatedContent);

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


        [Authorize(Policy = "RequireDefaultRole")]
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

        [Authorize(Policy = "RequireUserRole")]
        [HttpPost("generate-feedback")]
        public async Task<IActionResult> GenerateFeedbackForUser([FromBody] GenerateFeedbackRequestDTO request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.UserExperience))
            {
                return BadRequest("The userExperience field is required.");
            }

            Console.WriteLine($"Received userExperience: {request.UserExperience}");

            if (request.Rating < 1 || request.Rating > 5)
            {
                return BadRequest("Rating must be between 1 and 5.");
            }

            var user = await _userRepository.GetByUserNameAsync(request.Username);
            if (user == null) return BadRequest("Invalid username.");

            var ticket = await _ticketRepository.GetByIdAsync(request.TicketId);
            if (ticket == null) return BadRequest("Invalid ticket ID.");

            if (string.IsNullOrWhiteSpace(ticket.Description))
            {
                return BadRequest("Ticket description is required to generate feedback.");
            }
            var prompt = $@"
            Write a professional and concise feedback message from me (the **beneficiary** of a support ticket) to the developer with the username: {user.UserName}.

            Details:
            - I am the person who created the ticket and needed help. I did not fix the problem myself.
            - My experience as the beneficiary: {request.UserExperience}
            - The issue described in the ticket: {ticket.Description}
            - My rating for this support (1-5): {request.Rating}

            Instructions:
            - Write the feedback **as if I am personally addressing the developer** who resolved the ticket.
            - Mention briefly the problem that was solved, based on the ticket description.
            - Make it clear that I am the requester who benefited from the support.
            - The rating reflects my satisfaction with how the issue was handled.
            - Generate the feedback message in the following language: **{ticket.Language}**

            Tone:
            - For ratings 4-5: express appreciation and positive feedback.
            - For rating 3: provide neutral or constructive feedback, possibly with suggestions.
            - For ratings 1-2: be polite but express dissatisfaction and explain what could be improved.

            Format:
            - Keep the feedback short, clear, and respectful.
            - Do not include any headings, formatting, or instructions — return only the plain text message.

        ";



            try
            {
                var llmResponse = await _repositoryLLM.GenerateResponseAsync(prompt);
                string generatedFeedback = llmResponse.Response ?? "No feedback generated.";
                return Ok(generatedFeedback);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error generating feedback: {ex.Message}");
            }
        }

        [Authorize(Policy = "RequireDefaultRole")]
        [HttpGet("sentiment/average/{username}")]
        public async Task<ActionResult<AverageFeedbackSentimentDTO>> GetAverageSentimentByUsername(string username)
        {
            var sentimentData = await _feedbackSentimentRepository.GetAverageSentimentByUsernameAsync(username);
            return Ok(sentimentData); 
        }


        [Authorize(Policy = "RequireUserRole")]
        [HttpGet("check-eligibility/{username}/{ticketId}")]
        public async Task<ActionResult<bool>> CheckFeedbackEligibility(string username, int ticketId)
        {
            // Get the user by username
            var user = await _userRepository.GetByUserNameAsync(username);
            if (user == null) return BadRequest("Invalid username.");

            // Get the ticket by ID
            var ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.Id == ticketId);
            if (ticket == null) return BadRequest("Invalid ticket ID.");

            // Check if the user is eligible to give feedback
            bool isEligible = false;

            if (user.Id == ticket.CreatorId)
            {
                isEligible = !await _context.Feedbacks.AnyAsync(f => f.FromUserId == user.Id && f.TicketId == ticketId);
            }
            else if (user.Id == ticket.HandlerId)
            {
                isEligible = !await _context.Feedbacks.AnyAsync(f => f.FromUserId == user.Id && f.TicketId == ticketId);
            }

            return Ok(isEligible);
        }

        [Authorize(Policy = "RequireUserRole")]
        [HttpGet("has-feedback/{userId}")]
        public async Task<bool> HasFeedbackFromUserAsync(int ticketId, string username)
        {
            // Retrieve the user by username.
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);
            if (user == null)
            {
                // Optionally, you could throw an exception if the user is not found.
                return false;
            }

            // Check if any feedback exists for this ticket from this user.
            return await _context.Feedbacks
                .AnyAsync(f => f.TicketId == ticketId && f.FromUserId == user.Id);
        }

    }


}
