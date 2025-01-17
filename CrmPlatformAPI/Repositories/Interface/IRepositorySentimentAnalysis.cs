using CrmPlatformAPI.Models.DTO;

namespace CrmPlatformAPI.Repositories.Interface
{
    public interface IRepositorySentimentAnalysis
    {
        Task<SentimentResponseDTO> AnalyzeSentimentAsync(string textContent);

    }
}
