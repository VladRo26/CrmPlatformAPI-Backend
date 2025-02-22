using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using CrmPlatformAPI.Helpers;
using CrmPlatformAPI.Repositories.Interface;
using Microsoft.Extensions.Options;
namespace CrmPlatformAPI.Repositories.Implementation
{
    public class PhotoService : IPhotoService
    {
        private readonly Cloudinary _cloudinary;
        public PhotoService(IOptions<CloudinarySettings> configuration)
        {
            var account = new Account(configuration.Value.CloudName, configuration.Value.ApiKey, configuration.Value.ApiSecret);

            _cloudinary = new Cloudinary(account);
        }
        public async Task<ImageUploadResult> AddPhotoAsync(IFormFile file)
        {
           var uploadRes = new ImageUploadResult();

            if (file.Length > 0)
            {
                using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Transformation = new Transformation().Height(250).Width(250).Crop("fill").Gravity("face"),
                    Folder = "CRMPlatform"
                };

                uploadRes = await _cloudinary.UploadAsync(uploadParams);

            }
            return uploadRes;

        }

        public async Task<ImageUploadResult> AddCompanyPhotoAsync(IFormFile file)
        {
            var uploadRes = new ImageUploadResult();

            if (file.Length > 0)
            {
                using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Transformation = new Transformation().Height(770).Width(770).Crop("fill"),
                    Folder = "CRMPlatform"
                };

                uploadRes = await _cloudinary.UploadAsync(uploadParams);

            }
            return uploadRes;

        }

        public async Task<DeletionResult> DeletePhotoAsync(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);

            return await _cloudinary.DestroyAsync(deleteParams);
        }
    }
}
