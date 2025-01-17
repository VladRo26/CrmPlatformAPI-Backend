using CrmPlatformAPI.Models.Domain;

namespace CrmPlatformAPI.Repositories.Implementation
{
    public interface IRepositoryFeedbackSentiment
    {
        Task AddSentimentAsync(FeedBackSentiment sentiment);

    }
}
