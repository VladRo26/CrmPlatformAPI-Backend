using AutoMapper;
using CrmPlatformAPI.Models.Domain;
using CrmPlatformAPI.Models.DTO;
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

        public SoftwareCompanyController(IRepositorySoftwareCompany repositorySoftwareCompanies, IMapper mapper)
        {
            _repositorySoftwareCompany = repositorySoftwareCompanies;
            _mapper = mapper;
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



    }
}
