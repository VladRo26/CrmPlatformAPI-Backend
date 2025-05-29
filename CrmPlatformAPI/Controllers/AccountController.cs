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
    public class AccountController(UserManager<User> userManager,IRepositorySoftwareCompany _repositorySoftwareCompany, IRepositoryBeneficiaryCompany _repositoryBeneficiaryCompany ,IMapper _mapper, ITokenService tokenService, IPhotoService _photoService, IRepositoryUser _repositoryUser) : Controller
    {
        [HttpPost("register")]

        public async Task<ActionResult<UserDTO>> RegisterAsync([FromForm] RegisterDTO registerDTO)
        {
            if (await UserExists(registerDTO.UserName))
            {
                return BadRequest("Username already exists");
            }

            // Trim the phone number
            if (!string.IsNullOrEmpty(registerDTO.PhoneNumber))
            {
                registerDTO.PhoneNumber = registerDTO.PhoneNumber.Replace(" ", ""); // Remove all spaces
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

            var roleResult = await userManager.AddToRoleAsync(user, "Default");
            if (!roleResult.Succeeded)
            {
                return BadRequest("Failed to assign the Default role to the user");
            }


            string? companyName = user.UserType == UserType.SoftwareCompanyUser
                ? user.SoftwareCompany?.Name
                : user.BeneficiaryCompany?.Name;

            if(registerDTO.File != null)
            {
                var res = await _photoService.AddPhotoAsync(registerDTO.File);

                if (res.Error != null)
                {
                    return BadRequest(res.Error.Message);
                }

                var photo = new Photo
                {
                    Url = res.SecureUrl.AbsoluteUri,
                    PublicId = res.PublicId
                };

                user.Photo = photo;

                await _repositoryUser.SaveAllAsync();

            }


            return new UserDTO
            {
                UserName = user.UserName,
                Token = await tokenService.CreateToken(user),
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                CompanyName = companyName,
                HireDate = user.HireDate,
                PhotoUrl = user.Photo?.Url,
                UserType = user.UserType,
                Rating = user.Rating

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
                .Include(u => u.Photo)
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
                Token = await tokenService.CreateToken(user),
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                PhotoUrl = user.Photo?.Url,
                CompanyName = companyName,
                HireDate = user.HireDate,
                UserType = user.UserType,
                Rating = user.Rating
            };
        }


        private async Task<bool> UserExists(string username)
        {
            return await userManager.Users.AnyAsync(us => us.UserName == username.ToLower());
        }

    }
}
