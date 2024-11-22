using AutoMapper;
using CrmPlatformAPI.Repositories.Interface;
using Microsoft.AspNetCore.Mvc;

namespace CrmPlatformAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyPhotoController : Controller
    {
        private readonly IRepositoryCompanyPhoto _repositoryCompanyPhoto;
        private readonly IMapper _mapper;

        public CompanyPhotoController(IRepositoryCompanyPhoto repositoryCompanyPhoto, IMapper mapper)
        {
            _repositoryCompanyPhoto = repositoryCompanyPhoto;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetCompanyPhoto([FromQuery] int userId)
        {
            if (userId <= 0)
            {
                return BadRequest("Invalid userId. It must be greater than zero.");
            }

            var companyPhotoUrl = await _repositoryCompanyPhoto.GetComapanyPhotoUrlAsync(userId);

            if (string.IsNullOrEmpty(companyPhotoUrl))
            {
                return NotFound(new { Message = $"No company photo found for userId: {userId}" });
            }

            return Ok(new { PhotoUrl = companyPhotoUrl });
        }



    }
}
