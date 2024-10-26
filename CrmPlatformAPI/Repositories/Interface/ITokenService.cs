using CrmPlatformAPI.Models.Domain;

namespace CrmPlatformAPI.Repositories.Interface
{
    public interface ITokenService
    {
        string CreateToken(User user);
    }
}
