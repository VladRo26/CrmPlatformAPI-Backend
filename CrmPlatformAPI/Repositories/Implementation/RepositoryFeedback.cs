﻿using CrmPlatformAPI.Data;
using CrmPlatformAPI.Models.Domain;
using CrmPlatformAPI.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace CrmPlatformAPI.Repositories.Implementation
{
    public class RepositoryFeedback : IRepositoryFeedback
    {
        private readonly ApplicationDbContext _context;

        public RepositoryFeedback(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Feedback>> GetByFromUserIdAsync(int fromUserId)
        {
            if (_context == null)
            {
                return null;
            }

            return await _context.Feedbacks
                .Include(f => f.FromUser) 
                .Include(f => f.ToUser)   
                .Include(f => f.Ticket)   
                .Where(f => f.FromUserId == fromUserId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Feedback>> GetByTicketIdAsync(int ticketId)
        {
            if (_context == null)
            {
                return null;
            }

            return await _context.Feedbacks
                .Include(f => f.FromUser) // Include FromUser navigation property
                .Include(f => f.ToUser)   // Include ToUser navigation property
                .Include(f => f.Ticket)   // Include Ticket navigation property
                .Where(f => f.TicketId == ticketId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Feedback>> GetByToUserIdAsync(int toUserId)
        {
            if (_context == null)
            {
                return null;
            }

            return await _context.Feedbacks
                .Include(f => f.FromUser) // Include FromUser navigation property
                .Include(f => f.ToUser)   // Include ToUser navigation property
                .Include(f => f.Ticket)   // Include Ticket navigation property
                .Where(f => f.ToUserId == toUserId)
                .ToListAsync();
        }



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
