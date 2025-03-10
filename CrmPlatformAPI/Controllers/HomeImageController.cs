using AutoMapper;
using CrmPlatformAPI.Models.Domain;
using CrmPlatformAPI.Models.DTO;
using CrmPlatformAPI.Repositories.Implementation;
using CrmPlatformAPI.Repositories.Interface;
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

        [HttpPost("upload")]
        public async Task<IActionResult> CreateHomeImage([FromForm] CreateHomeImageDTO createHomeImageDTO)
        {
            // Map the DTO to a HomeImage domain model.
            var homeImage = _mapper.Map<HomeImage>(createHomeImageDTO);

            // Upload the image using the new method in PhotoService.
            if (createHomeImageDTO.File != null)
            {
                var uploadResult = await _photoService.AddHomeImageAsync(createHomeImageDTO.File);
                if (uploadResult.Error != null)
                {
                    return BadRequest(uploadResult.Error.Message);
                }
                homeImage.Url = uploadResult.SecureUrl.AbsoluteUri;
            }

            homeImage.Title = createHomeImageDTO.Title;
            homeImage.UploadDate = DateTime.Now;

            // Save the new HomeImage record.
            var createdImage = await _repositoryHomeImage.CreateAsync(homeImage);
            var response = _mapper.Map<ImageDTO>(createdImage);

            return Ok(response);
        }
    }
}
