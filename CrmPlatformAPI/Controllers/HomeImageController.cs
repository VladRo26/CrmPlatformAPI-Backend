using AutoMapper;
using CrmPlatformAPI.Models.Domain;
using CrmPlatformAPI.Models.DTO;
using CrmPlatformAPI.Repositories.Implementation;
using CrmPlatformAPI.Repositories.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CrmPlatformAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeImageController : Controller
    {
        private readonly IRepositoryHomeImage _repositoryHomeImage;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;

        public HomeImageController(IRepositoryHomeImage repositoryHomeImage, IMapper mapper, IPhotoService photoService)
        {
            _repositoryHomeImage = repositoryHomeImage;
            _mapper = mapper;
            _photoService = photoService;
        }

        [HttpGet]
        public async Task<IActionResult> GetHomeImages()
        {
            var homeImages = await _repositoryHomeImage.GetImagesAsync();

            var response = _mapper.Map<IEnumerable<ImageDTO>>(homeImages);

            return Ok(response);
        }

        [Authorize(Policy = "RequireModeratorRole")]
        [HttpPost("upload")]
        public async Task<IActionResult> CreateHomeImage([FromForm] CreateHomeImageDTO createHomeImageDTO)
        {
            // Map the DTO to the domain model.
            var homeImage = _mapper.Map<HomeImage>(createHomeImageDTO);

            if (createHomeImageDTO.File != null)
            {
                var uploadResult = await _photoService.AddHomeImageAsync(createHomeImageDTO.File);
                if (uploadResult.Error != null)
                {
                    return BadRequest(uploadResult.Error.Message);
                }
                // Manually set properties from the upload result.
                homeImage.Url = uploadResult.SecureUrl.AbsoluteUri;
                homeImage.PublicId = uploadResult.PublicId;
            }

            homeImage.Title = createHomeImageDTO.Title;
            homeImage.UploadDate = DateTime.UtcNow;

            var createdImage = await _repositoryHomeImage.CreateAsync(homeImage);
            var response = _mapper.Map<ImageDTO>(createdImage);

            return Ok(response);
        }

        [Authorize(Policy = "RequireModeratorRole")]
        [HttpDelete("{*publicId}")]
        public async Task<IActionResult> DeleteHomeImage(string publicId)
        {
            // First, delete the image record from the database.
            var deletedImage = await _repositoryHomeImage.DeleteImageAsync(publicId);
            if (deletedImage == null)
            {
                return NotFound(new { message = "Image not found in database" });
            }

            // Next, delete the image from Cloudinary.
            var cloudinaryDeletionResult = await _photoService.DeletePhotoAsync(publicId);
            if (cloudinaryDeletionResult.Result == "ok")
            {
                return Ok(new { message = "Image deleted successfully" });
            }
            else
            {
                // Optionally: you might consider restoring the record or logging the failure.
                return BadRequest(new { message = "Image deleted from database but failed to delete from Cloudinary" });
            }
        }



    }


}
