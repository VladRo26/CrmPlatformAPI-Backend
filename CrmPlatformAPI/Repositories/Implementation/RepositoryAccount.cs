using CrmPlatformAPI.Data;
using CrmPlatformAPI.Models.Domain;
using CrmPlatformAPI.Models.DTO;
using CrmPlatformAPI.Repositories.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CrmPlatformAPI.Repositories.Implementation
{
    public class RepositoryAccount : IRepositoryAccount
    {
        private readonly ApplicationDbContext _context;


        public RepositoryAccount(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<RegisterDTO> CreateAsync(UserManager<User> userManager, User user, RegisterDTO registerDTO)
        {
            await userManager.CreateAsync(user, registerDTO.Password);
            return registerDTO;
        }

        public async Task<bool> UserExists(string username)
        {
            //verifica daca sunt useri in baza de date


            if(_context.Users.Count() == 0)
            {
                return false;
            }else
            {

            return await _context.Users.AnyAsync(x => x.UserName.ToLower() == username.ToLower());

            }

            
        }
    }
}
