namespace CrmPlatformAPI.Repositories.Interface
{
    public interface IRepositoryCompanyPhoto
    {
        Task<string> GetComapanyPhotoUrlAsync(int userId);
    }
}
