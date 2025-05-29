using CloudinaryDotNet.Actions;

namespace CrmPlatformAPI.Repositories.Interface
{
    public interface IFileService
    {
        Task<RawUploadResult> UploadFileAsync(IFormFile file);
        Task<DeletionResult> DeleteFileAsync(string publicId);
    }
}
