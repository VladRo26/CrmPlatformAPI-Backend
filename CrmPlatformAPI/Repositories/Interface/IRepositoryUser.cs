using CrmPlatformAPI.Models.Domain;

namespace CrmPlatformAPI.Repositories.Interface
{
    public interface IRepositoryUser
    {
        Task<bool> UpdateAsync(User user);

        Task<IEnumerable<User>> GetAllAsync();

        Task<User> GetByIdAsync(int id);

        Task<IEnumerable<User>> GetByCompanyAsync(string name);

        Task<IEnumerable<User>> GetByNameAsync(string Name);

        Task<User?> GetByUserNameAsync(string username);

        Task<bool> SaveAllAsync();

        Task<string?> GetPhotoUrlByUsernameAsync(string username);


    }
}
