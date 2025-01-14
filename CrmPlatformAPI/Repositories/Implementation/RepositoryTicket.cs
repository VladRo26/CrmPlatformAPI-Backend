﻿using CrmPlatformAPI.Data;
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

        public async Task<string> GenerateSummaryForTicketAsync(int ticketId, string model,int maxTokens)
        {
            // Fetch the ticket
            var ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.Id == ticketId);

            if (ticket == null || string.IsNullOrWhiteSpace(ticket.Description))
            {
                throw new Exception($"Ticket with ID {ticketId} not found or has no description.");
            }

            // Generate a summary using LLM
            var summary = await _llmRepository.GenerateResponseAsync($"Summarize this ticket description: {ticket.Description}");

            return summary.Response ?? "No summary generated.";
        }


    }
}
