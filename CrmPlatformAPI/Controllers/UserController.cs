using AutoMapper;
using CrmPlatformAPI.Models.DTO;
using CrmPlatformAPI.Repositories.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CrmPlatformAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IRepositoryUser _repositoryUser;
        private readonly IRepositoryCompanyPhoto _repositoryCompanyPhoto;
        private readonly IMapper _mapper;

        public UserController(IRepositoryUser repositoryUser, IRepositoryCompanyPhoto repositoryCompanyPhoto, IMapper mapper)
        {
            _repositoryUser = repositoryUser;
            _repositoryCompanyPhoto = repositoryCompanyPhoto;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _repositoryUser.GetAllAsync();
            var userDtos = new List<UserAppDTO>();

            foreach (var user in users)
            {
                var companyPhotoUrl = await _repositoryCompanyPhoto.GetComapanyPhotoUrlAsync(user.Id);
                var userDto = _mapper.Map<UserAppDTO>(user);
                userDto.CompanyPhotoUrl = companyPhotoUrl;
                userDtos.Add(userDto);
            }

            return Ok(userDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _repositoryUser.GetByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            var companyPhotoUrl = await _repositoryCompanyPhoto.GetComapanyPhotoUrlAsync(id);
            var userDto = _mapper.Map<UserAppDTO>(user);
            userDto.CompanyPhotoUrl = companyPhotoUrl;

            return Ok(userDto);
        }

        [HttpGet("name/{name}")]
        public async Task<IActionResult> GetUserByName(string name)
        {
            var users = await _repositoryUser.GetByNameAsync(name);

            if (users == null || !users.Any())
            {
                return NotFound();
            }

            var userDtos = new List<UserAppDTO>();

            foreach (var user in users)
            {
                var companyPhotoUrl = await _repositoryCompanyPhoto.GetComapanyPhotoUrlAsync(user.Id);
                var userDto = _mapper.Map<UserAppDTO>(user);
                userDto.CompanyPhotoUrl = companyPhotoUrl;
                userDtos.Add(userDto);
            }

            return Ok(userDtos);
        }
        [HttpGet("username/{username}")]
        public async Task<IActionResult> GetUserByUsername(string username)
        {
            var user = await _repositoryUser.GetByUserNameAsync(username);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            var companyPhotoUrl = await _repositoryCompanyPhoto.GetComapanyPhotoUrlAsync(user.Id);
            var userDto = _mapper.Map<UserAppDTO>(user);
            userDto.CompanyPhotoUrl = companyPhotoUrl;

            return Ok(userDto);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUser(UpdateUserDTO updateUserDTO)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (username == null)
            {
                return NotFound();
            }

            var user = await _repositoryUser.GetByUserNameAsync(username);

            if(user == null)
            {
                return NotFound();
            }

            _mapper.Map(updateUserDTO, user);


            bool success = await _repositoryUser.UpdateAsync(user);
            if (!success)
            {
                return StatusCode(500, "Update failed.");
            }else
            {
                return Ok();
            }
        }
    }
}
