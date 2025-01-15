using CrmPlatformAPI.Models.DTO;
using CrmPlatformAPI.Repositories.Implementation;
using CrmPlatformAPI.Repositories.Interface;
using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CrmPlatformAPI.Controllers
{
    [ApiController]
    [Route("api/llm")]
    public class LLMController : Controller
    {
        private readonly IRepositoryLLM _llmRepository;


        public LLMController(IRepositoryLLM llmRepository)
        {
            _llmRepository = llmRepository;
        }

        [HttpPost("generate-response")]
        public async Task<IActionResult> GenerateResponse([FromBody] string prompt)
        {
            try
            {
                var result = await _llmRepository.GenerateResponseAsync(prompt);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpPost("translate")]
        public async Task<IActionResult> TranslateText([FromQuery] string text, [FromQuery] string sourceLanguage, [FromQuery] string targetLanguage)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(text) || string.IsNullOrWhiteSpace(sourceLanguage) || string.IsNullOrWhiteSpace(targetLanguage))
                {
                    return BadRequest(new { message = "Text, source language, and target language are required." });
                }

                var translation = await _llmRepository.TranslateTextAsync(text, sourceLanguage, targetLanguage);
                return Ok(new { sourceLanguage, targetLanguage, text, translation });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }
    }
}
