using CrmPlatformAPI.Models.Domain;
using CrmPlatformAPI.Models.DTO;

namespace CrmPlatformAPI.Repositories.Interface
{
    public interface IRepositoryFeedbackSentiment
    {
        Task AddSentimentAsync(FeedBackSentiment sentiment);

        Task<FeedBackSentiment> GetSentimentByFeedbackIdAsync(int feedbackId);

        Task<AverageFeedbackSentimentDTO?> GetAverageSentimentByUsernameAsync(string username);


    }
}
