using CrmPlatformAPI.Models.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CrmPlatformAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController(UserManager<User> userManager) : Controller
    {
        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("GetUserWithRoles")] 
        public async Task<ActionResult>GetUserWithRoles()
        {
            var users = await userManager.Users
                .OrderBy(u => u.UserName)
                .Select(u => new
                {
                    u.Id,
                    Username = u.UserName,
                    Roles = u.UserRoles.Select(ur => ur.Role.Name).ToList()
                })
                .ToListAsync();

            return Ok(users);

        }
        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("EditRoles/{username}")]
        public async Task<ActionResult> EditRoles(string username, string roles)
        {
            if(string.IsNullOrEmpty(roles))
            {
                return BadRequest("Roles cannot be empty");
            }

            var selectedRoles = roles.Split(",");

            var user = await userManager.FindByNameAsync(username);

            if (user == null)
            {
                return NotFound("Could not find user");
            }

            var userRoles = await userManager.GetRolesAsync(user);

            var result = await userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));

            if (!result.Succeeded)
            {
                return BadRequest("Failed to add roles");
            }

            result = await userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));

            return Ok(await userManager.GetRolesAsync(user));

        }
    }
}
