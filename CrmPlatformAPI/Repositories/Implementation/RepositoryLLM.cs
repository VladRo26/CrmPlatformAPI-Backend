﻿using CrmPlatformAPI.Models.DTO;
using CrmPlatformAPI.Repositories.Interface;

namespace CrmPlatformAPI.Repositories.Implementation
{
    public class RepositoryLLM : IRepositoryLLM
    {
        private readonly HttpClient _httpClient;

        private const string Model = "gemma2-9b-it";
        private const int MaxTokens = 3000;

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


        public async Task<string> TranslateTextAsync(string text, string sourceLanguage, string targetLanguage)
        {
            // Construct a translation-specific prompt
            var prompt = $"Write only the translation Translate the following text from {sourceLanguage} to {targetLanguage}: \"{text}\"";

            // Call the LLM to generate the translation
            var response = await GenerateResponseAsync(prompt);

            // Return the translation response
            return response.Response ?? "No translation generated.";
        }


    }
}