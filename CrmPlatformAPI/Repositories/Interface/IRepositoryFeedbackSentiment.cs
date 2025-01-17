using CrmPlatformAPI.Models.Domain;

namespace CrmPlatformAPI.Repositories.Interface
{
    public interface IRepositoryFeedbackSentiment
    {
        Task AddSentimentAsync(FeedBackSentiment sentiment);

        Task<FeedBackSentiment> GetSentimentByFeedbackIdAsync(int feedbackId);


    }
}
