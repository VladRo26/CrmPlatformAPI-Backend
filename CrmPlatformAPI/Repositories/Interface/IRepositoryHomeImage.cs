using CrmPlatformAPI.Models.Domain;

namespace CrmPlatformAPI.Repositories.Interface
{
    public interface IRepositoryHomeImage
    {
        Task<IEnumerable<HomeImage>> GetImagesAsync();

        Task<HomeImage> CreateAsync(HomeImage homeImage);
    }
}
