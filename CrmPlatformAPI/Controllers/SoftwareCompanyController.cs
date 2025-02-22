using AutoMapper;
using CrmPlatformAPI.Models.Domain;
using CrmPlatformAPI.Models.DTO;
using CrmPlatformAPI.Repositories.Implementation;
using CrmPlatformAPI.Repositories.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CrmPlatformAPI.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class SoftwareCompanyController : Controller
    {
        private readonly IRepositorySoftwareCompany _repositorySoftwareCompany;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;


        public SoftwareCompanyController(IRepositorySoftwareCompany repositorySoftwareCompanies, IMapper mapper, IPhotoService photoService)
        {
            _repositorySoftwareCompany = repositorySoftwareCompanies;
            _mapper = mapper;
            _photoService = photoService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateSoftwareCompany(CreateSoftwareCompanyDTO createSoftwareCompanyDTO)
        {
            var createSoftwareCompany = _mapper.Map<SoftwareCompany>(createSoftwareCompanyDTO);

            await _repositorySoftwareCompany.CreateAsync(createSoftwareCompany);

            var response = _mapper.Map<SoftwareCompanyDTO>(createSoftwareCompany);

            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetSoftwareCompanies()
        {
            var softwareCompanies = await _repositorySoftwareCompany.GetSoftwareCompaniesAsync();

            var response = _mapper.Map<IEnumerable<SoftwareCompanyDTO>>(softwareCompanies);

            return Ok(response);
        }

        [HttpGet("ByUserId/{userId}")]
        public async Task<IActionResult> GetSoftwareCompanyByUserId(int userId)
        {
            var company = await _repositorySoftwareCompany.GetSoftwareCompanyByUserIdAsync(userId);

            if (company == null)
            {
                // Return an empty object or null instead of a 404 error
                return Ok(null);
            }

            var response = _mapper.Map<SoftwareCompanyDTO>(company);
            return Ok(response);
        }

        [HttpPost("register")]
        [Authorize]
        public async Task<IActionResult> RegisterSoftwareCompany([FromForm] CreateSoftwareCompanyWithPhotoDTO dto)
        {
            // Map DTO to domain model
            var softwareCompany = _mapper.Map<SoftwareCompany>(dto);

            // If a file is provided, upload the photo and assign to CompanyPhoto property
            if (dto.File != null)
            {
                var uploadResult = await _photoService.AddCompanyPhotoAsync(dto.File);
                if (uploadResult.Error != null)
                {
                    return BadRequest(uploadResult.Error.Message);
                }

                softwareCompany.CompanyPhoto = new CompanyPhoto
                {
                    Url = uploadResult.SecureUrl.AbsoluteUri,
                    PublicId = uploadResult.PublicId
                };
            }

            var createdCompany = await _repositorySoftwareCompany.CreateAsync(softwareCompany);
            if (createdCompany == null)
            {
                return BadRequest("Failed to create company.");
            }

            var response = _mapper.Map<SoftwareCompanyDTO>(createdCompany);
            return Ok(response);
        }

    }
}
