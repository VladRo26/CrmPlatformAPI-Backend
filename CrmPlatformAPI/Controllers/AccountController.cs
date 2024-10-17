using AutoMapper;
using CrmPlatformAPI.Models.Domain;
using CrmPlatformAPI.Models.DTO;
using CrmPlatformAPI.Repositories.Implementation;
using CrmPlatformAPI.Repositories.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;


namespace CrmPlatformAPI.Controllers
{
    public class AccountController(UserManager<User> userManager, IRepositoryAccount _repositoryAccount,IRepositorySoftwareCompany _repositorySoftwareCompany, IMapper _mapper) : Controller
    {
       
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDTO registerDTO)
        {
            if (await _repositoryAccount.UserExists(registerDTO.UserName))
            {
                return BadRequest("Username already exists");
            }

            var existingCompany = await _repositorySoftwareCompany.GetSoftwareCompanyByIdAsync(registerDTO.CompanyName);

            if (existingCompany == null && !string.IsNullOrEmpty(registerDTO.CompanyName))
            {
                return BadRequest("Company does not exist");

            }


            var user = _mapper.Map<User>(registerDTO);

            user.SoftwareCompanyId = existingCompany?.Id;

            user.UserName = registerDTO.UserName.ToLower();

            await _repositoryAccount.CreateAsync(userManager, user, registerDTO);

            var response = _mapper.Map<RegisterDTO>(user);

            return Ok(response);
        }

      

    }
}
