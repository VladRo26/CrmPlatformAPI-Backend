using AutoMapper;
using CrmPlatformAPI.Models.Domain;
using CrmPlatformAPI.Models.DTO;
using CrmPlatformAPI.Repositories.Interface;
using Microsoft.AspNetCore.Mvc;

namespace CrmPlatformAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContractController : Controller
    {
        private readonly IRepositoryContract _repositoryContract;
        private readonly IMapper _mapper;

        public ContractController(IRepositoryContract repositoryContract, IMapper mapper)
        {
            _repositoryContract = repositoryContract;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> CreateContract(CreateContractDTO createContractDTO)
        {
            var createContract = _mapper.Map<Contract>(createContractDTO);

            try
            {
                var savedContract = await _repositoryContract.CreateAsync(createContract, createContractDTO.BeneficiaryCompanyName, createContractDTO.SoftwareCompanyName);

                var response = _mapper.Map<ContractDTO>(savedContract);

                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetContracts()
        {
            var contracts = await _repositoryContract.GetContractsAsync();

            var response = _mapper.Map<IEnumerable<ContractDTO>>(contracts);

            return Ok(response);
        }

        [HttpGet("search")]
        public async Task<IActionResult> GetContractsByName([FromQuery] string? SoftComp = null, [FromQuery] string? BenefComp = null)
        {
            var contracts = await _repositoryContract.GetContractsByNameAsync(SoftComp, BenefComp);

            if (contracts == null || !contracts.Any())
            {
                return NotFound();
            }

            var response = _mapper.Map<IEnumerable<ContractDTO>>(contracts);

            return Ok(response);
        }

        [HttpGet("by-beneficiary")]
        public async Task<IActionResult> GetContractsByBeneficiaryCompanyName([FromQuery] string beneficiaryCompanyName)
        {
            if (string.IsNullOrEmpty(beneficiaryCompanyName))
            {
                return BadRequest(new { message = "Beneficiary company name must be provided." });
            }

            var contracts = await _repositoryContract.GetContractsByBeneficiaryCompanyNameAsync(beneficiaryCompanyName);

            if (contracts == null || !contracts.Any())
            {
                return NotFound(new { message = $"No contracts found for beneficiary company '{beneficiaryCompanyName}'." });
            }

            var response = _mapper.Map<IEnumerable<ContractDTO>>(contracts);

            return Ok(response);
        }

    }
}
