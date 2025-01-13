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
    }
}
