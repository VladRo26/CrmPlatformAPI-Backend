using AutoMapper;
using CrmPlatformAPI.Models.DTO;
using CrmPlatformAPI.Repositories.Interface;
using Microsoft.AspNetCore.Mvc;

namespace CrmPlatformAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IRepositoryUser _repositoryUser;
        private readonly IMapper _mapper;

        public UserController(IRepositoryUser repositoryUser, IMapper mapper)
        {
            _repositoryUser = repositoryUser;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _repositoryUser.GetAllAsync();

            var response = _mapper.Map<IEnumerable<UserDTO>>(users);

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _repositoryUser.GetByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            var response = _mapper.Map<UserDTO>(user);

            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UpdateUserDTO updateUserDTO)
        {
            var user = await _repositoryUser.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound(); // 404 Not Found
            }

            _mapper.Map(updateUserDTO, user);

            bool success = await _repositoryUser.UpdateAsync(user);
            if (success)
            {
                return NoContent(); // 204 No Content
            }
            else
            {
                return StatusCode(500, "Update failed."); // 500 Internal Server Error
            }
        }

    }
}
