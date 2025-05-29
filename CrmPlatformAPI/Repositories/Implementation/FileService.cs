using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using CrmPlatformAPI.Helpers;
using CrmPlatformAPI.Repositories.Interface;
using Microsoft.Extensions.Options;

namespace CrmPlatformAPI.Repositories.Implementation
{
    public class FileService : IFileService
    {
        private readonly Cloudinary _cloudinary;

        public FileService(IOptions<CloudinarySettings> config)
        {
            var account = new Account(
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
            );

            _cloudinary = new Cloudinary(account);
        }
        public async Task<RawUploadResult> UploadFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Invalid file");

            using var stream = file.OpenReadStream();
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file.FileName);

            var uploadParams = new RawUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                PublicId = $"CRMPlatform/{fileNameWithoutExtension}",
                Overwrite = true
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            return uploadResult;
        }

        public async Task<DeletionResult> DeleteFileAsync(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);
            var deletionResult = await _cloudinary.DestroyAsync(deleteParams);
            return deletionResult;
        }
    }
}
