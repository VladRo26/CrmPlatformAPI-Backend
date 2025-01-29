using CrmPlatformAPI.Data;
using CrmPlatformAPI.Helpers.Enums;
using CrmPlatformAPI.Models.Domain;
using CrmPlatformAPI.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace CrmPlatformAPI.Repositories.Implementation
{
    public class RepositoryTicket : IRepositoryTicket
    {
        private readonly ApplicationDbContext _context;
        private readonly IRepositoryLLM _llmRepository;


        public RepositoryTicket(ApplicationDbContext context, IRepositoryLLM llmRepository)
        {
            _context = context;
            _llmRepository = llmRepository;

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


        public async Task<IEnumerable<Ticket>> GetByUserNameAsync(string username)
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
                .Where(t => t.Creator.UserName == username || t.Handler.UserName == username)
                .ToListAsync();

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

            // Construct a detailed prompt using the ticket description
            var prompt = $"Summarize this ticket description very very briefly: {ticket.Description}. Just write the summary";

            // Use the LLM repository to generate a summary
            var summaryResponse = await _llmRepository.GenerateResponseAsync(prompt);
            var summary = summaryResponse?.Response ?? "No summary generated.";

            // Check if the ticket's language is specified and translate if necessary
            if (!string.IsNullOrWhiteSpace(ticket.Language))
            {
                // Assuming the source language is English (or the language of the summary)
               string sourceLanguage = ticket.Language;

                // Use the TranslateTextAsync function to translate the summary
                summary = await _llmRepository.TranslateTextAsync(summary, sourceLanguage, ticket.TLanguage)
                          ?? "Translation failed.";
            }

            // Return the generated summary (translated, if applicable)
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

        public async Task<IEnumerable<Ticket>> GetByContractIdAsync(int contractId)
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
                .Where(t => t.ContractId == contractId)
                .ToListAsync();
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



    }
}
