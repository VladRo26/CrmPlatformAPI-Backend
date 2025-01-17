using CrmPlatformAPI.Models.DTO;
using CrmPlatformAPI.Repositories.Interface;

namespace CrmPlatformAPI.Repositories.Implementation
{
    public class RepositorySentimentAnalysis : IRepositorySentimentAnalysis
    {
        private readonly HttpClient _httpClient;

        public RepositorySentimentAnalysis(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("SentimentClient");
        }

        public async Task<SentimentResponseDTO> AnalyzeSentimentAsync(string textContent)
        {
            var request = new SentimentRequestDTO
            {
                TextContent = textContent
            };

            var response = await _httpClient.PostAsJsonAsync("/analyze-sentiment", request);

            if (response.IsSuccessStatusCode)
            {
                // Parse and return the sentiment analysis response
                var result = await response.Content.ReadFromJsonAsync<SentimentResponseDTO>();
                return result;
            }

            // Handle error responses
            var errorMessage = await response.Content.ReadAsStringAsync();
            throw new Exception($"FastAPI error: {errorMessage}");
        }

    }
}
