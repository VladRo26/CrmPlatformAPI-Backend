using AutoMapper;
using CrmPlatformAPI.Models;
using CrmPlatformAPI.Models.Domain;
using CrmPlatformAPI.Models.DTO;
using CrmPlatformAPI.Repositories.Interface;
using Microsoft.AspNetCore.Authorization;
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

        [Authorize(Policy = "RequireModeratorRole")]
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

            var response = _mapper.Map<IEnumerable<ContractDTO>>(contracts);

            return Ok(response);
        }

        [HttpGet("by-software")]
        public async Task<IActionResult> GetContractsBySoftwareCompanyName([FromQuery] string softwareCompanyName)
        {
            if (string.IsNullOrEmpty(softwareCompanyName))
            {
                return BadRequest(new { message = "Software company name must be provided." });
            }

            var contracts = await _repositoryContract.GetContractsBySoftwareCompanyNameAsync(softwareCompanyName);

            var response = _mapper.Map<IEnumerable<ContractDTO>>(contracts);

            return Ok(response);
        }

        [HttpGet("by-ticket/{ticketId}")]
        public async Task<IActionResult> GetContractByTicketId(int ticketId)
        {
            var contract = await _repositoryContract.GetContractByTicketIdAsync(ticketId);
            if (contract == null)
            {
                return NotFound(new { message = "No contract found for the given ticket id." });
            }
            var response = _mapper.Map<ContractDTO>(contract);
            return Ok(response);
        }

        [HttpGet("count")]
        public async Task<IActionResult> GetContractCount()
        {
            try
            {
                int count = await _repositoryContract.CountContractsAsync();
                return Ok(new { count });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to count contracts.", error = ex.Message });
            }
        }

        [Authorize(Policy = "RequireModeratorRole")]
        [HttpPost("create-contract")]
        public async Task<IActionResult> CreateContractByName([FromForm] CreateContractByNameDTO dto)
        {
            var contract = _mapper.Map<Contract>(dto);

            try
            {
                // This method in your repository will look up company IDs by name.
                var savedContract = await _repositoryContract.CreateAsync(
                    contract,
                    dto.BeneficiaryCompanyName,
                    dto.SoftwareCompanyName);

                var response = _mapper.Map<ContractDTO>(savedContract);
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Policy = "RequireModeratorRole")]
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateContractStatus(int id, [FromBody] UpdateContractStatusDTO dto)
        {
            var updatedContract = await _repositoryContract.UpdateContractStatusAsync(id, dto.Status);
            if (updatedContract == null)
            {
                return NotFound(new { message = "Contract not found." });
            }
            var response = _mapper.Map<ContractDTO>(updatedContract);
            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateContract(int id, [FromForm] UpdateContractDTO dto)
        {
            // Map the update DTO to your domain model
            var contractToUpdate = _mapper.Map<Contract>(dto);
            // Ensure the correct contract is updated by setting the Id
            contractToUpdate.Id = id;

            var updatedContract = await _repositoryContract.UpdateContractAsync(contractToUpdate);
            if (updatedContract == null)
            {
                return NotFound(new { message = "Contract not found." });
            }
            var response = _mapper.Map<ContractDTO>(updatedContract);
            return Ok(response);
        }



        [HttpGet("{id}")]
        public async Task<IActionResult> GetContractById(int id)
        {
            var contract = await _repositoryContract.GetContractByIdAsync(id);
            if (contract == null)
            {
                return NotFound(new { message = "Contract not found." });
            }
            var response = _mapper.Map<ContractDTO>(contract);
            return Ok(response);
        }


    }
}
