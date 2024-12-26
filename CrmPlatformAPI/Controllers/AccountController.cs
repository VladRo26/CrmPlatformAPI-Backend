using AutoMapper;
using CrmPlatformAPI.Helpers.Enums;
using CrmPlatformAPI.Models.Domain;
using CrmPlatformAPI.Models.DTO;
using CrmPlatformAPI.Repositories.Implementation;
using CrmPlatformAPI.Repositories.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace CrmPlatformAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController(UserManager<User> userManager,IRepositorySoftwareCompany _repositorySoftwareCompany, IRepositoryBeneficiaryCompany _repositoryBeneficiaryCompany ,IMapper _mapper, ITokenService tokenService) : Controller
    {
        [HttpPost("register")]
        public async Task<ActionResult<UserDTO>> RegisterAsync([FromBody] RegisterDTO registerDTO)
        {
            if (await UserExists(registerDTO.UserName))
            {
                return BadRequest("Username already exists");
            }

            User user = _mapper.Map<User>(registerDTO);
            user.UserName = registerDTO.UserName.ToLower();

            if (registerDTO.UserType == UserType.SoftwareCompanyUser)
            {
                var existingSoftwareCompany = await _repositorySoftwareCompany.GetSoftwareCompanyByNameAsync(registerDTO.CompanyName);
                if (existingSoftwareCompany == null && !string.IsNullOrEmpty(registerDTO.CompanyName))
                {
                    return BadRequest("Software company does not exist");
                }
                user.SoftwareCompanyId = existingSoftwareCompany?.Id;
            }
            else if (registerDTO.UserType == UserType.BeneficiaryCompanyUser)
            {
                var existingBeneficiaryCompany = await _repositoryBeneficiaryCompany.GetBeneficiaryCompanyByNameAsync(registerDTO.CompanyName);
                if (existingBeneficiaryCompany == null && !string.IsNullOrEmpty(registerDTO.CompanyName))
                {
                    return BadRequest("Beneficiary company does not exist");
                }
                user.BeneficiaryCompanyId = existingBeneficiaryCompany?.Id;
            }

            var result = await userManager.CreateAsync(user, registerDTO.Password);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            string? companyName = user.UserType == UserType.SoftwareCompanyUser
                ? user.SoftwareCompany?.Name
                : user.BeneficiaryCompany?.Name;

            return new UserDTO
            {
                UserName = user.UserName,
                Token = tokenService.CreateToken(user),
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                CompanyName = companyName,
                UserType = user.UserType
            };
        }


        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> LoginAsync([FromBody] LoginDTO loginDTO)
        {
            if (string.IsNullOrEmpty(loginDTO.UserName) || string.IsNullOrEmpty(loginDTO.Password))
            {
                return Unauthorized("Invalid username or password");
            }

            // Fetch the user including both company types
            var user = await userManager.Users
                .Include(u => u.SoftwareCompany)
                .Include(u => u.BeneficiaryCompany)
                .FirstOrDefaultAsync(u => u.NormalizedUserName == loginDTO.UserName.ToUpper());

            if (user == null || user.UserName == null)
            {
                return Unauthorized("Invalid username or password");
            }

            var result = await userManager.CheckPasswordAsync(user, loginDTO.Password);

            if (!result)
            {
                return Unauthorized("Invalid username or password");
            }

            // Determine the company name based on UserType
            string? companyName = user.UserType switch
            {
                UserType.SoftwareCompanyUser => user.SoftwareCompany?.Name,
                UserType.BeneficiaryCompanyUser => user.BeneficiaryCompany?.Name,
                _ => null
            };

            return new UserDTO
            {
                UserName = user.UserName,
                Token = tokenService.CreateToken(user),
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                PhotoUrl = user.Photo?.Url,
                CompanyName = companyName,
                UserType = user.UserType
            };
        }


        private async Task<bool> UserExists(string username)
        {
            return await userManager.Users.AnyAsync(us => us.UserName == username.ToLower());
        }

    }
}
