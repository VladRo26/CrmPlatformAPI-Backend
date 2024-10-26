using AutoMapper;
using CrmPlatformAPI.Models.DTO;
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

       public HomeImageController(IRepositoryHomeImage repositoryHomeImage, IMapper mapper)
        {
            _repositoryHomeImage = repositoryHomeImage;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetHomeImages()
        {
            var homeImages = await _repositoryHomeImage.GetImagesAsync();

            var response = _mapper.Map<IEnumerable<ImageDTO>>(homeImages);

            return Ok(response);
        }
    }
}
