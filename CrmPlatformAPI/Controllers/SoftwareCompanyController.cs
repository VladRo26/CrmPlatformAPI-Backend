using AutoMapper;
using CrmPlatformAPI.Extensions;
using CrmPlatformAPI.Helpers;
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

        [HttpGet("paged")]
        public async Task<IActionResult> GetSoftwareCompaniesPaged([FromQuery] CompanyParams companyParams)
        {
            var companies = await _repositorySoftwareCompany.GetCompaniesAsync(companyParams);
            Response.AddPagination(companies);
            var response = _mapper.Map<IEnumerable<SoftwareCompanyDTO>>(companies);
            return Ok(response);
        }

        [HttpPut("ByName/{companyName}")]
        [Authorize]
        public async Task<IActionResult> UpdateSoftwareCompanyByName(string companyName, [FromForm] UpdateSoftwareCompanyDTO dto)
        {
            // Retrieve the existing software company by name.
            var existingCompany = await _repositorySoftwareCompany.GetSoftwareCompanyByNameAsync(companyName);
            if (existingCompany == null)
            {
                return NotFound(new { message = "Software company not found." });
            }

            // Map the update DTO to a domain model.
            // Make sure your AutoMapper profile for UpdateSoftwareCompanyDTO → SoftwareCompany ignores the CompanyPhoto.
            var updatedCompany = _mapper.Map<SoftwareCompany>(dto);
            updatedCompany.Id = existingCompany.Id; // Preserve the existing company's Id

            // If a new photo file is provided, upload it.
            if (dto.File != null)
            {
                var uploadResult = await _photoService.AddCompanyPhotoAsync(dto.File);
                if (uploadResult.Error != null)
                {
                    return BadRequest(uploadResult.Error.Message);
                }
                updatedCompany.CompanyPhoto = new CompanyPhoto
                {
                    Url = uploadResult.SecureUrl.AbsoluteUri,
                    PublicId = uploadResult.PublicId
                };
            }
            else
            {
                // Retain the existing photo if no new file is provided.
                updatedCompany.CompanyPhoto = existingCompany.CompanyPhoto;
            }

            // Update the company using the repository.
            var result = await _repositorySoftwareCompany.UpdateAsync(updatedCompany);
            if (result == null)
            {
                return BadRequest("Failed to update software company.");
            }

            var response = _mapper.Map<SoftwareCompanyDTO>(result);
            return Ok(response);
        }

        [HttpGet("ByName/{companyName}")]
        public async Task<IActionResult> GetSoftwareCompanyByName(string companyName)
        {
            var company = await _repositorySoftwareCompany.GetSoftwareCompanyByNameAsync(companyName);
            if (company == null)
            {
                return NotFound(new { message = $"Software company with name '{companyName}' not found." });
            }
            var response = _mapper.Map<SoftwareCompanyDTO>(company);
            return Ok(response);
        }

    }
}
