using AutoMapper;
using CrmPlatformAPI.Models.Domain;
using CrmPlatformAPI.Models.DTO;
using CrmPlatformAPI.Repositories.Interface;
using Microsoft.AspNetCore.Mvc;

namespace CrmPlatformAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BeneficiaryCompaniesController : Controller
    {
        private readonly IRepositoryBeneficiaryCompanies _repositoryBeneficiaryCompanies;
        private readonly IMapper _mapper;

        public BeneficiaryCompaniesController(IRepositoryBeneficiaryCompanies repositoryBeneficiaryCompanies, IMapper mapper)
        {
            _repositoryBeneficiaryCompanies = repositoryBeneficiaryCompanies;
            _mapper= mapper;
        }

        [HttpPost]
        public async Task<IActionResult> CreateBeneficiaryCompany(CreateBeneficiaryCompanyDTO createBeneficiaryCompanyDTO)
        {
           var createBeneficiaryCompany = _mapper.Map<BeneficiaryCompanies>(createBeneficiaryCompanyDTO);

           await _repositoryBeneficiaryCompanies.CreateAsync(createBeneficiaryCompany);

           var response = _mapper.Map<BeneficiaryCompaniesDTO>(createBeneficiaryCompany);

            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetBeneficiaryCompanies()
        {
            var beneficiaryCompanies = await _repositoryBeneficiaryCompanies.GetBeneficiaryCompaniesAsync();

            var response = _mapper.Map<IEnumerable<BeneficiaryCompaniesDTO>>(beneficiaryCompanies);

            return Ok(response);
        }

    }
}
