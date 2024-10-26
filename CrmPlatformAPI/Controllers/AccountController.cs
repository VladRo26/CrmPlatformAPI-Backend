using AutoMapper;
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
    public class AccountController(UserManager<User> userManager,IRepositorySoftwareCompany _repositorySoftwareCompany, IMapper _mapper, ITokenService tokenService) : Controller
    {
        [HttpPost("register")]
        public async Task<ActionResult<UserDTO>> RegisterAsync([FromBody] RegisterDTO registerDTO)
        {
            if (await UserExists(registerDTO.UserName))
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

            var result = await userManager.CreateAsync(user, registerDTO.Password);


            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return new UserDTO
            {
                UserName = user.UserName,
                Token = tokenService.CreateToken(user),
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                SoftwareCompanyName = existingCompany?.Name
            };

        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> LoginAsync([FromBody] LoginDTO loginDTO)
        {

            if(string.IsNullOrEmpty(loginDTO.UserName) || string.IsNullOrEmpty(loginDTO.Password))
            {
                return Unauthorized("Invalid username or password");
            }

            var user = await userManager.Users
                .Include(u => u.SoftwareCompany)
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

            return new UserDTO
            {
                UserName = user.UserName,
                Token = tokenService.CreateToken(user),
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                SoftwareCompanyName = user.SoftwareCompany?.Name
            };

        }

        private async Task<bool> UserExists(string username)
        {
            return await userManager.Users.AnyAsync(us => us.UserName == username.ToLower());
        }

    }
}
