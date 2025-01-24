using CrmPlatformAPI.Models.Domain;

namespace CrmPlatformAPI.Repositories.Interface
{
    public interface ITokenService
    {
        Task<string> CreateToken(User user);
    }
}
