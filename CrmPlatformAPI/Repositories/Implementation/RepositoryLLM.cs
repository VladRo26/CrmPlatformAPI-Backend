using CrmPlatformAPI.Models.DTO;
using CrmPlatformAPI.Repositories.Interface;

namespace CrmPlatformAPI.Repositories.Implementation
{
    public class RepositoryLLM : IRepositoryLLM
    {
        private readonly HttpClient _httpClient;

        private const string Model = "llama-3.3-70b-versatile";
        private const int MaxTokens = 100;

        public RepositoryLLM(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("LLMClient");
        }

        public async Task<LLMResponseDTO> GenerateResponseAsync(string prompt)
        {
            var request = new LLMRequestDTO
            {
                Prompt = prompt,
                Model = Model,
                MaxTokens = MaxTokens
            };

            var response = await _httpClient.PostAsJsonAsync("/generate-response", request);

            if (response.IsSuccessStatusCode)
            {
                // Parse and return the response
                var result = await response.Content.ReadFromJsonAsync<LLMResponseDTO>();
                return result;
            }

            // Handle error responses
            var errorMessage = await response.Content.ReadAsStringAsync();
            throw new Exception($"FastAPI error: {errorMessage}");
        }


    }
}