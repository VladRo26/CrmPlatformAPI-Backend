using CrmPlatformAPI.Models.Domain;
using CrmPlatformAPI.Models.DTO;
using Microsoft.AspNetCore.Identity;

namespace CrmPlatformAPI.Repositories.Interface
{
    public interface IRepositoryAccount
    {
        Task<RegisterDTO> CreateAsync(UserManager<User> userManager,User user,RegisterDTO registerDTO);

        Task<bool> UserExists(string username);
    }
}
