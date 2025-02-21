using AutoMapper;
using CrmPlatformAPI.Extensions;
using CrmPlatformAPI.Helpers;
using CrmPlatformAPI.Models.Domain;
using CrmPlatformAPI.Models.DTO;
using CrmPlatformAPI.Repositories.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CrmPlatformAPI.Controllers
{
    [ServiceFilter(typeof(ActivityLog))]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IRepositoryUser _repositoryUser;
        private readonly IRepositoryCompanyPhoto _repositoryCompanyPhoto;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;

        public UserController(IRepositoryUser repositoryUser, IRepositoryCompanyPhoto repositoryCompanyPhoto, IMapper mapper, IPhotoService photoService)
        {
            _repositoryUser = repositoryUser;
            _repositoryCompanyPhoto = repositoryCompanyPhoto;
            _mapper = mapper;
            _photoService = photoService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery] UserParams userParams)
        {
            userParams.CurrentUserName = User.GetUsername();
            var users = await _repositoryUser.GetAllAsync(userParams);
            Response.AddPagination(users);

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

        //[HttpGet]
        //public async Task<IActionResult> GetUsers()
        //{
        //    var users = await _repositoryUser.GetAllAsync();
        //    var userDtos = new List<UserAppDTO>();

        //    foreach (var user in users)
        //    {
        //        var companyPhotoUrl = await _repositoryCompanyPhoto.GetComapanyPhotoUrlAsync(user.Id);
        //        var userDto = _mapper.Map<UserAppDTO>(user);
        //        userDto.CompanyPhotoUrl = companyPhotoUrl;
        //        userDtos.Add(userDto);
        //    }

        //    return Ok(userDtos);
        //}

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

        [HttpGet("userappdto/username/{username}")]
        public async Task<IActionResult> GetUserAppDtoByUsername(string username)
        {
            var user = await _repositoryUser.GetByUserNameAsync(username);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Fetch additional details like CompanyPhotoUrl
            var companyPhotoUrl = await _repositoryCompanyPhoto.GetComapanyPhotoUrlAsync(user.Id);

            // Use AutoMapper to map the user entity to UserAppDTO
            var userAppDto = _mapper.Map<UserAppDTO>(user);

            // Assign additional fields that are not directly mapped
            userAppDto.CompanyPhotoUrl = companyPhotoUrl;

            return Ok(userAppDto);
        }


        [HttpPut]
        public async Task<IActionResult> UpdateUser(UpdateUserDTO updateUserDTO)
        {

            var user = await _repositoryUser.GetByUserNameAsync(User.GetUsername());

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

       [HttpGet("photo-url")]
        public async Task<ActionResult<object>> GetPhotoUrl()
        {
            var user = await _repositoryUser.GetByUserNameAsync(User.GetUsername());

            if (user == null)
            {
                return NotFound("User not found");
            }

            var photoUrl = user.Photo?.Url;

            if (string.IsNullOrEmpty(photoUrl))
            {
                return NotFound("Photo URL not available");
            }

            return Ok(new { url = photoUrl }); // Wrap the URL in an object
        }

        [HttpGet("photo-url/{username}")]
        public async Task<ActionResult<object>> GetPhotoUrlByUsername(string username)
        {
            var photoUrl = await _repositoryUser.GetPhotoUrlByUsernameAsync(username);

            // Always return a JSON object, even if the photoUrl is null or empty
            return Ok(new { url = photoUrl ?? string.Empty });
        }



        [HttpPost("upload-photo")]
        public async Task<ActionResult<ImageDTO>> UploadPhoto(IFormFile file)
        {

            var user = await _repositoryUser.GetByUserNameAsync(User.GetUsername());

            if (user == null)
            {
                return BadRequest("Cannot update user");
            }

            var res = await _photoService.AddPhotoAsync(file);

            if (res.Error  != null)
            {
                return BadRequest(res.Error.Message);
            }

            var photo = new Photo
            {
                Url = res.SecureUrl.AbsoluteUri,
                PublicId = res.PublicId
            };

            user.Photo = photo;

            if(await _repositoryUser.SaveAllAsync())
            {
                return CreatedAtAction(nameof(GetUserByUsername), new { username = user.UserName }, _mapper.Map<ImageDTO>(photo));
            }

            return BadRequest("Problem uploading photo");

        }

        [HttpDelete("delete-photo")]
        public async Task<ActionResult> DeletePhoto()
        {
            var user = await _repositoryUser.GetByUserNameAsync(User.GetUsername());

            if (user == null)
            {
                return BadRequest("User not found");
            }

            if (user.Photo == null)
            {
                return BadRequest("Photo not found");
            }

            var res = await _photoService.DeletePhotoAsync(user.Photo.PublicId);

            if (res.Error != null)
            {
                return BadRequest(res.Error.Message);
            }

            user.Photo = null;

            if (await _repositoryUser.SaveAllAsync())
            {
                return Ok();
            }

            return BadRequest("Problem deleting photo");
        }

        [HttpDelete("username/{username}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(string username)
        {
            var user = await _repositoryUser.GetByUserNameAsync(username);
            if (user == null)
                return NotFound("User not found.");

            // Use the user's id to delete them
            bool success = await _repositoryUser.DeleteUserAsync(user.Id);
            if (!success)
                return StatusCode(500, "Error deleting user.");

            return Ok(new { message = "User deleted successfully" });
        }



    }
}
