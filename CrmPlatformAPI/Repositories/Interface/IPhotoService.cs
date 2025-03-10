using CloudinaryDotNet.Actions;

namespace CrmPlatformAPI.Repositories.Interface
{
    public interface IPhotoService
    {
        Task<ImageUploadResult> AddPhotoAsync(IFormFile file);

        Task<ImageUploadResult> AddCompanyPhotoAsync(IFormFile file);

        Task<DeletionResult> DeletePhotoAsync(string publicId);

        Task<ImageUploadResult> AddHomeImageAsync(IFormFile file);
    }
}
