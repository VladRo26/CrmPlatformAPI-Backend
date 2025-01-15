using CrmPlatformAPI.Models.DTO;

namespace CrmPlatformAPI.Repositories.Interface
{
    public interface IRepositoryLLM 
    {
        Task<LLMResponseDTO> GenerateResponseAsync(string prompt);

        Task<string> TranslateTextAsync(string text, string sourceLanguage, string targetLanguage);

    }
}
