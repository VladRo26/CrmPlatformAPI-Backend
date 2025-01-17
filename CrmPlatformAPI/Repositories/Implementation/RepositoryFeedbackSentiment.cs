using CrmPlatformAPI.Data;
using CrmPlatformAPI.Models.Domain;
using CrmPlatformAPI.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace CrmPlatformAPI.Repositories.Implementation
{
    public class RepositoryFeedbackSentiment : IRepositoryFeedbackSentiment
    {

        private readonly ApplicationDbContext _context;

        public RepositoryFeedbackSentiment(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddSentimentAsync(FeedBackSentiment sentiment)
        {
            _context.FeedbackSentiments.Add(sentiment);
            await _context.SaveChangesAsync();
        }

        public async Task<FeedBackSentiment> GetSentimentByFeedbackIdAsync(int feedbackId)
        {
            if (_context == null)
            {
                return null;
            }
            return await _context.FeedbackSentiments
                .Include(s => s.Feedback) // Include Feedback navigation property if needed
                .FirstOrDefaultAsync(s => s.FeedbackId == feedbackId);
        }
    }
}
