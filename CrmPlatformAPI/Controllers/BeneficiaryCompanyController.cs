using AutoMapper;
using CrmPlatformAPI.Models.Domain;
using CrmPlatformAPI.Models.DTO;
using CrmPlatformAPI.Repositories.Interface;
using Microsoft.AspNetCore.Mvc;

namespace CrmPlatformAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BeneficiaryCompanyController : Controller
    {
        private readonly IRepositoryBeneficiaryCompany _repositoryBeneficiaryCompany;
        private readonly IMapper _mapper;

        public BeneficiaryCompanyController(IRepositoryBeneficiaryCompany repositoryBeneficiaryCompanies, IMapper mapper)
        {
            _repositoryBeneficiaryCompany = repositoryBeneficiaryCompanies;
            _mapper= mapper;
        }

        [HttpPost]
        public async Task<IActionResult> CreateBeneficiaryCompany(CreateBeneficiaryCompanyDTO createBeneficiaryCompanyDTO)
        {
           var createBeneficiaryCompany = _mapper.Map<BeneficiaryCompany>(createBeneficiaryCompanyDTO);

           await _repositoryBeneficiaryCompany.CreateAsync(createBeneficiaryCompany);

           var response = _mapper.Map<BeneficiaryCompanyDTO>(createBeneficiaryCompany);

            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetBeneficiaryCompanies()
        {
            var beneficiaryCompanies = await _repositoryBeneficiaryCompany.GetBeneficiaryCompaniesAsync();

            var response = _mapper.Map<IEnumerable<BeneficiaryCompanyDTO>>(beneficiaryCompanies);

            return Ok(response);
        }

        [HttpGet("ByUsername/{username}")]
        public async Task<IActionResult> GetCompanyByUsername(string username)
        {
            var company = await _repositoryBeneficiaryCompany.GetCompanyByUsernameAsync(username);

            if (company == null)
            {
                return NotFound(new { message = $"No company found for user '{username}'." });
            }

            var response = _mapper.Map<BeneficiaryCompanyDTO>(company);
            return Ok(response);
        }

        [HttpGet("ByUserId/{userId}")]
        public async Task<IActionResult> GetBeneficiaryCompanyByUserId(int userId)
        {
            var company = await _repositoryBeneficiaryCompany.GetBeneficiaryCompanyByUserIdAsync(userId);

            if (company == null)
            {
                // Return an empty object or null instead of a 404 error
                return Ok(null);
            }

            var response = _mapper.Map<BeneficiaryCompanyDTO>(company);
            return Ok(response);
        }

    }
}
