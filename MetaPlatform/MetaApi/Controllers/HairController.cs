using MetaApi.Models.VirtualHair;
using MetaApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MetaApi.Controllers
{
    [Route("api/hair")]
    [ApiController]
    public class HairController : ControllerBase
    {
        private readonly IFileService _fileService;

        public HairController(IFileService fileService)
        {
            _fileService = fileService;
        }

        // Получить коллекцию 
        [HttpGet, Authorize]
        public ActionResult<HairCollectionResponse> GetHairCollection()
        {
            var collection = _fileService.GetHairCollection(Request.Host.Value);

            var clothingResult = new HairCollectionResponse
            {                
                Man = collection.Man.Select(item => new HairItemDto { Link = item.Link }).ToArray(),
                Woman = collection.Woman.Select(item => new HairItemDto { Link = item.Link }).ToArray(),
            };

            return Ok(clothingResult);
        }
    }
}
