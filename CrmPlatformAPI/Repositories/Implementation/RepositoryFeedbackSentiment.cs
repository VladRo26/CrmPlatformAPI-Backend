using CrmPlatformAPI.Data;
using CrmPlatformAPI.Models.Domain;

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
    }
}
