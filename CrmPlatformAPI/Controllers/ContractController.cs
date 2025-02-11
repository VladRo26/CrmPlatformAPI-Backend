﻿using AutoMapper;
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


    }
}
