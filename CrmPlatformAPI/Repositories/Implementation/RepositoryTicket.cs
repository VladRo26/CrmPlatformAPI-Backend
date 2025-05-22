using CrmPlatformAPI.Data;
using CrmPlatformAPI.Helpers;
using CrmPlatformAPI.Helpers.Enums;
using CrmPlatformAPI.Models.Domain;
using CrmPlatformAPI.Models.DTO;
using CrmPlatformAPI.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace CrmPlatformAPI.Repositories.Implementation
{
    public class RepositoryTicket : IRepositoryTicket
    {
        private readonly ApplicationDbContext _context;
        private readonly IRepositoryLLM _llmRepository;
        private readonly IEmailService _emailService;


        public RepositoryTicket(ApplicationDbContext context, IRepositoryLLM llmRepository, IEmailService emailService)
        {
            _context = context;
            _llmRepository = llmRepository;
            _emailService = emailService;

        }
        public async Task<IEnumerable<Ticket>> GetAllAsync()
        {
            if (_context == null)
            {
                return null;
            }

            return await _context.Tickets
              .Include(t => t.Contract)
                  .ThenInclude(c => c.BeneficiaryCompany)
              .Include(t => t.Contract)
                  .ThenInclude(c => c.SoftwareCompany)
              .ToListAsync();

        }

        public async Task<IEnumerable<Ticket>> GetByCompanyAsync(string name)
        {
            if (_context == null)
            {
                return null;
            }


            return await _context.Tickets
               .Include(t => t.Contract)
                   .ThenInclude(c => c.BeneficiaryCompany)
               .Include(t => t.Contract)
                   .ThenInclude(c => c.SoftwareCompany)
               .Where(t => t.Contract.BeneficiaryCompany.Name.Contains(name) ||
                           t.Contract.SoftwareCompany.Name.Contains(name))
               .ToListAsync();

        }

        public async Task<Ticket?> GetByIdAsync(int id)
        {
            if (_context == null)
            {
                return null;
            }

            return await _context.Tickets
                .Include(t => t.Contract)
                    .ThenInclude(c => c.BeneficiaryCompany)
                .Include(t => t.Contract)
                    .ThenInclude(c => c.SoftwareCompany)
                .FirstOrDefaultAsync(t => t.Id == id);

        }

        public async Task<IEnumerable<Ticket>> GetByTitleAsync(string name)
        {
            if (_context == null)
            {
                return null;
            }
            return await _context.Tickets
                .Include(t => t.Contract)
                    .ThenInclude(c => c.BeneficiaryCompany)
                .Include(t => t.Contract)
                    .ThenInclude(c => c.SoftwareCompany)
                .Include(t => t.Creator)
                .Include(t => t.Handler)
                .Where(t => t.Title != null && t.Title.Contains(name))
                .ToListAsync();

        }

        public async Task<IEnumerable<Ticket>> GetByStatusAsync(string status)
        {
            if (_context == null)
            {
                return null;
            }

            if (!Enum.TryParse<TicketStatus>(status, true, out var parsedStatus))
            {
                return Enumerable.Empty<Ticket>();
            }

            return await _context.Tickets
                .Include(t => t.Contract)
                    .ThenInclude(c => c.BeneficiaryCompany)
                .Include(t => t.Contract)
                    .ThenInclude(c => c.SoftwareCompany)
                .Include(t => t.Creator)
                .Include(t => t.Handler)
                .Where(t => t.Status == parsedStatus)
                .ToListAsync();
        }


        public async Task<IEnumerable<Ticket>> GetByUserIdAsync(int userId)
        {
            if (_context == null)
            {
                return null;
            }

            return await _context.Tickets
                .Include(t => t.Contract)
                    .ThenInclude(c => c.BeneficiaryCompany)
                .Include(t => t.Contract)
                    .ThenInclude(c => c.SoftwareCompany)
                .Include(t => t.Creator)
                .Include(t => t.Handler)
                .Where(t => t.CreatorId == userId || t.HandlerId == userId)
                .ToListAsync();
        }

        public async Task<PagedList<Ticket>> GetByUserNameAsync(TicketParams ticketParams)
        {
            var query = _context.Tickets
                 .Include(t => t.Contract)
                     .ThenInclude(c => c.BeneficiaryCompany)
                 .Include(t => t.Contract)
                     .ThenInclude(c => c.SoftwareCompany)
                 .Include(t => t.Creator)
                 .Include(t => t.Handler)
                 .AsQueryable();

            if (!string.IsNullOrEmpty(ticketParams.Username))
            {
                query = query.Where(t => t.Creator.UserName == ticketParams.Username ||
                                         t.Handler.UserName == ticketParams.Username);
            }

            if (!string.IsNullOrEmpty(ticketParams.Status))
            {
                if (ticketParams.Status.Equals("notClosed", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(t => t.Status != TicketStatus.Closed);
                }
                else if (ticketParams.Status.Equals("all", StringComparison.OrdinalIgnoreCase))
                {
                    // No status filtering
                }
                else if (Enum.TryParse<TicketStatus>(ticketParams.Status, true, out var statusEnum))
                {
                    query = query.Where(t => t.Status == statusEnum);
                }
            }
            else
            {
                query = query.Where(t => t.Status != TicketStatus.Closed);
            }

            if (!string.IsNullOrEmpty(ticketParams.Priority))
            {
                if (Enum.TryParse<TicketPriority>(ticketParams.Priority, true, out var priorityEnum))
                {
                    query = query.Where(t => t.Priority == priorityEnum);
                }
            }
            if (!string.IsNullOrEmpty(ticketParams.Title))
            {
                query = query.Where(t => t.Title.Contains(ticketParams.Title));
            }

            // NEW: Sorting logic with sort direction
            if (!string.IsNullOrEmpty(ticketParams.OrderBy))
            {
                // Determine the sort order: ascending or descending.
                bool ascending = ticketParams.SortDirection.Equals("asc", StringComparison.OrdinalIgnoreCase);

                if (ticketParams.OrderBy.Equals("date", StringComparison.OrdinalIgnoreCase))
                {
                    query = ascending
                        ? query.OrderBy(t => t.CreatedAt)
                        : query.OrderByDescending(t => t.CreatedAt);
                }
                else if (ticketParams.OrderBy.Equals("priority", StringComparison.OrdinalIgnoreCase))
                {
                    query = ascending
                        ? query.OrderBy(t => t.Priority)
                        : query.OrderByDescending(t => t.Priority);
                }
                else
                {
                    // Default sorting by date
                    query = ascending
                        ? query.OrderBy(t => t.CreatedAt)
                        : query.OrderByDescending(t => t.CreatedAt);
                }
            }
            else
            {
                query = query.OrderByDescending(t => t.CreatedAt);
            }

            return await PagedList<Ticket>.CreateAsync(query, ticketParams.PageNumber, ticketParams.PageSize);
        }


        public async Task<IEnumerable<Ticket>> GetByPriorityAsync(string priority)
        {
            if (_context == null)
            {
                return null;
            }

            if (!Enum.TryParse<TicketPriority>(priority, true, out var parsedPriority))
            {
                return Enumerable.Empty<Ticket>();
            }

            return await _context.Tickets
                .Include(t => t.Contract)
                    .ThenInclude(c => c.BeneficiaryCompany)
                .Include(t => t.Contract)
                    .ThenInclude(c => c.SoftwareCompany)
                .Include(t => t.Creator)
                .Include(t => t.Handler)
                .Where(t => t.Priority == parsedPriority)
                .ToListAsync();
        }

        public async Task<IEnumerable<Ticket>> GetByHandlerUsernameAsync(string handlerUsername)
        {
            if (_context == null)
            {
                return null;
            }

            return await _context.Tickets
                .Include(t => t.Contract)
                    .ThenInclude(c => c.BeneficiaryCompany)
                .Include(t => t.Contract)
                    .ThenInclude(c => c.SoftwareCompany)
                .Include(t => t.Creator)
                .Include(t => t.Handler)
                .Where(t => t.Handler != null && t.Handler.UserName == handlerUsername)
                .ToListAsync();
        }

        public async Task<string> GenerateSummaryForTicketAsync(int ticketId)
        {
            // Fetch the ticket from the database
            var ticket = await GetByIdAsync(ticketId);
            if (ticket == null || string.IsNullOrWhiteSpace(ticket.Description))
            {
                throw new Exception($"Ticket with ID {ticketId} not found or has no description.");
            }

            // Use the ticket's language, default to English if missing
            var originalLanguage = string.IsNullOrWhiteSpace(ticket.Language) ? "English" : ticket.Language;

            // Build prompt in English, but instruct the model to respond in the original language
            var prompt = $@"
            You are a support assistant helping summarize issues related to software services.

            All tickets are related to problems, bugs, or requests in a software system. Read the ticket description below and generate a **very brief summary (1–2 sentences max)** that clearly captures the main issue or request.

            Focus on the **software-related problem** the user is experiencing. Be specific, clear, and concise.

            Write the summary in **{originalLanguage}**.
            Avoid repetition, unnecessary details, or formatting. Return only the plain text summary.

            Ticket description:
            {ticket.Description}
            ";



            // Generate the summary in the original language
            var summaryResponse = await _llmRepository.GenerateResponseAsync(prompt);
            var summary = summaryResponse?.Response ?? "No summary generated.";

            // If a translation language is provided, translate the summary
            if (!string.IsNullOrWhiteSpace(ticket.TLanguage))
            {
                var translated = await _llmRepository.TranslateTextAsync(summary, originalLanguage, ticket.TLanguage);
                summary = translated ?? "Translation failed.";
            }

            return summary;
        }




        public async Task<string> TranslateDescriptionForTicketAsync(int ticketId, string sourceLanguage, string targetLanguage)
        {
            // Fetch the ticket from the database
            var ticket = await GetByIdAsync(ticketId);
            if (ticket == null || string.IsNullOrWhiteSpace(ticket.Description))
            {
                throw new Exception($"Ticket with ID {ticketId} not found or has no description.");
            }

            // Use the LLM repository to translate the description
            var translatedText = await _llmRepository.TranslateTextAsync(ticket.Description, sourceLanguage, targetLanguage);

            // Return the translated text
            return translatedText ?? "No translation generated.";
        }

        public async Task AddAsync(Ticket ticket)
        {
            if (_context == null)
            {
                throw new Exception("Database context is not initialized.");
            }

            try
            {
                // Start a transaction to ensure both the ticket and its status history are saved atomically
                using var transaction = await _context.Database.BeginTransactionAsync();

                // Add the ticket to the database
                await _context.Tickets.AddAsync(ticket);
                await _context.SaveChangesAsync();

                // Create a new TicketStatusHistory entry for the ticket
                var statusHistory = new TicketStatusHistory
                {
                    TicketId = ticket.Id,
                    Status = TicketStatus.Open,
                    Message = ticket.Title,
                    UpdatedAt = DateTime.UtcNow,
                    UpdatedByUserId = ticket.CreatorId, 
                    TicketUserRole = TicketUserRole.Creator
                };

                // Add the status history entry to the database
                await _context.TicketStatusHistories.AddAsync(statusHistory);
                await _context.SaveChangesAsync();

                // Commit the transaction
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                throw new Exception("An error occurred while adding the ticket and its status history.", ex);
            }
        }


        public async Task<IEnumerable<Models.Domain.Contract>> GetContractsByBeneficiaryCompanyNameAsync(string beneficiaryCompanyName)
        {
            if (_context == null)
            {
                return null;
            }

            return await _context.Contracts
                .Include(c => c.BeneficiaryCompany)
                    .ThenInclude(bc => bc.CompanyPhoto)  // Include BeneficiaryCompany photo
                .Include(c => c.SoftwareCompany)
                    .ThenInclude(sc => sc.CompanyPhoto) // Include SoftwareCompany photo
                .Where(c => c.BeneficiaryCompany.Name == beneficiaryCompanyName)
                .ToListAsync();
        }

        public async Task<bool> TakeOverTicketAsync(int ticketId, int handlerId)
        {
            if (_context == null)
            {
                throw new Exception("Database context is not initialized.");
            }

            var ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.Id == ticketId);
            if (ticket == null)
            {
                throw new Exception($"Ticket with ID {ticketId} not found.");
            }

            ticket.HandlerId = handlerId;

            try
            {
                _context.Tickets.Update(ticket);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                throw new Exception("An error occurred while taking over the ticket.", ex);
            }
        }

        public async Task<PagedList<Ticket>> GetByContractIdAsync(int contractId, TicketContractsParams ticketContractsParams)
        {
            if (_context == null)
            {
                throw new Exception("Database context is not initialized.");
            }

            var query = _context.Tickets
                        .Include(t => t.Contract)
                            .ThenInclude(c => c.BeneficiaryCompany)
                        .Include(t => t.Contract)
                            .ThenInclude(c => c.SoftwareCompany)
                        .Include(t => t.Creator)
                        .Include(t => t.Handler)
                        .Where(t => t.ContractId == contractId)
                        .AsQueryable();

            // Filter by handler username if provided
            if (!string.IsNullOrEmpty(ticketContractsParams.HandlerUsername))
            {
                query = query.Where(t => t.Handler != null && t.Handler.UserName.Contains(ticketContractsParams.HandlerUsername));
            }

            // Apply sorting based on the provided SortBy and SortDirection
            if (!string.IsNullOrEmpty(ticketContractsParams.SortBy))
            {
                bool ascending = ticketContractsParams.SortDirection.Equals("asc", StringComparison.OrdinalIgnoreCase);
                switch (ticketContractsParams.SortBy.ToLower())
                {
                    case "assigned":
                        // Sort tickets so that assigned ones appear first (or vice versa)
                        // Here we sort by whether HandlerId is null (0 if assigned, 1 if not)
                        query = ascending
                            ? query.OrderBy(t => t.HandlerId == null ? 1 : 0)
                            : query.OrderByDescending(t => t.HandlerId == null ? 1 : 0);
                        break;
                    case "unassigned":
                        // Reverse of "assigned": unassigned ones first
                        query = ascending
                            ? query.OrderBy(t => t.HandlerId != null ? 1 : 0)
                            : query.OrderByDescending(t => t.HandlerId != null ? 1 : 0);
                        break;
                    case "priority":
                        query = ascending
                            ? query.OrderBy(t => t.Priority)
                            : query.OrderByDescending(t => t.Priority);
                        break;
                    case "status":
                        query = ascending
                            ? query.OrderBy(t => t.Status)
                            : query.OrderByDescending(t => t.Status);
                        break;
                    default:
                        // Default sorting by creation date
                        query = ascending
                            ? query.OrderBy(t => t.CreatedAt)
                            : query.OrderByDescending(t => t.CreatedAt);
                        break;
                }
            }
            else
            {
                // Default sort by creation date descending
                query = query.OrderByDescending(t => t.CreatedAt);
            }

            return await PagedList<Ticket>.CreateAsync(query, ticketContractsParams.PageNumber, ticketContractsParams.PageSize);
        }



        public async Task<bool> UpdateAsync(Ticket ticket)
        {
            if (_context == null)
            {
                throw new Exception("Database context is not initialized.");
            }

            try
            {
                var existingTicket = await _context.Tickets.FirstOrDefaultAsync(t => t.Id == ticket.Id);
                if (existingTicket == null)
                {
                    throw new Exception($"Ticket with ID {ticket.Id} not found.");
                }

                // Update the properties of the existing ticket
                existingTicket.Title = ticket.Title;
                existingTicket.Description = ticket.Description;
                existingTicket.Status = ticket.Status;
                existingTicket.Priority = ticket.Priority;
                existingTicket.Type = ticket.Type;
                existingTicket.TLanguage = ticket.TLanguage;
                existingTicket.TLanguageCode = ticket.TLanguageCode;
                existingTicket.TCountryCode = ticket.TCountryCode;

                // Save changes to the database
                _context.Tickets.Update(existingTicket);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                throw new Exception("An error occurred while updating the ticket.", ex);
            }
        }

        public async Task<IEnumerable<Ticket>> GetFeedbackTicketByUserNameAsync(string username)
        {
            if (_context == null)
            {
                return null;
            }

            return await _context.Tickets
                .Include(t => t.Contract)
                    .ThenInclude(c => c.BeneficiaryCompany)
                .Include(t => t.Contract)
                    .ThenInclude(c => c.SoftwareCompany)
                .Include(t => t.Creator)
                .Include(t => t.Handler)
                .Where(t => (t.Creator.UserName == username || t.Handler.UserName == username) &&
                            t.Status == TicketStatus.Closed &&
                            !_context.Feedbacks.Any(f => f.TicketId == t.Id &&
                                                         f.FromUserId == _context.Users
                                                             .Where(u => u.UserName == username)
                                                             .Select(u => u.Id)
                                                             .FirstOrDefault())) 
                .ToListAsync();
        }


        public async Task<IEnumerable<object>> GetTicketsGroupedBySoftwareCompanyAsync(string username)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);
            if (user == null)
            {
                return new List<object>(); // Return empty if user not found
            }

            return await _context.Tickets
                .Where(t => t.HandlerId == user.Id) // Filter by Handler (Assigned User)
                .Include(t => t.Contract)
                .ThenInclude(c => c.SoftwareCompany)
                .GroupBy(t => new { t.Contract.SoftwareCompanyId, t.Contract.SoftwareCompany.Name }) 
                .Select(group => new
                {
                    SoftwareCompanyId = group.Key.SoftwareCompanyId,
                    SoftwareCompanyName = group.Key.Name,
                    TotalTickets = group.Count(),
                    TicketsByStatus = group.GroupBy(t => t.Status)
                                           .Select(statusGroup => new
                                           {
                                               Status = statusGroup.Key.ToString(),
                                               Count = statusGroup.Count()
                                           })
                                           .ToList()
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<TicketGroupedByCompanyDTO>> GetTicketsGroupedByBeneficiaryCompanyAsync(string username)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);
            if (user == null)
            {
                return new List<TicketGroupedByCompanyDTO>(); // ✅ Return empty list if user not found
            }

            return await _context.Tickets
                .Where(t => t.HandlerId == user.Id) // ✅ Filter by Handler (Assigned User)
                .Include(t => t.Contract)
                .ThenInclude(c => c.BeneficiaryCompany)
                .GroupBy(t => new { t.Contract.BeneficiaryCompanyId, t.Contract.BeneficiaryCompany.Name })
                .Select(group => new TicketGroupedByCompanyDTO
                {
                    BeneficiaryCompanyId = group.Key.BeneficiaryCompanyId,
                    BeneficiaryCompanyName = group.Key.Name,
                    TotalTickets = group.Count(),
                    TicketsByStatus = group.GroupBy(t => t.Status)
                                           .Select(statusGroup => new TicketStatusDTO
                                           {
                                               Status = statusGroup.Key.ToString(),
                                               Count = statusGroup.Count()
                                           })
                                           .ToList()
                })
                .ToListAsync();
        }

     

        public async Task<IEnumerable<object>> GetTicketsGroupedByContractAsync(string username)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);
            if (user == null)
            {
                return new List<object>(); // Return empty if user not found
            }

            return await _context.Tickets
                .Where(t => t.CreatorId == user.Id) // Filter tickets created by user
                .Include(t => t.Contract)
                .GroupBy(t => new { t.Contract.Id, t.Contract.ProjectName })
                .Select(group => new
                {
                    ContractId = group.Key.Id,
                    ProjectName = group.Key.ProjectName,
                    TotalTickets = group.Count(),
                    TicketsByStatus = group.GroupBy(t => t.Status)
                                           .Select(statusGroup => new
                                           {
                                               Status = statusGroup.Key.ToString(),
                                               Count = statusGroup.Count()
                                           })
                                           .ToList()
                })
                .ToListAsync();
        }


        public async Task<IEnumerable<object>> GetTicketsGroupedByUserStatusAsync(string username)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);
            if (user == null)
            {
                return new List<object>(); // Return empty if user not found
            }

            return await _context.Tickets
                .Where(t => t.CreatorId == user.Id)
                .GroupBy(t => t.Status)
                .Select(group => new
                {
                    Status = group.Key.ToString(),
                    TotalTickets = group.Count()
                })
                .ToListAsync();
        }


    }
}
