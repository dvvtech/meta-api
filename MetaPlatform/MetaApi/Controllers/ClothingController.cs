using MetaApi.Models.VirtualFit;
using MetaApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MetaApi.Controllers
{
    [Route("api/clothing")]
    [ApiController]
    public class ClothingController : ControllerBase
    {
        private readonly FileService _fileService;

        public ClothingController(FileService fileService)
        {
            _fileService = fileService;
        }

        // Получить коллекцию одежды
        [HttpGet, Authorize]
        public IActionResult GetClothingCollection()
        {
            var collection = _fileService.GetClothingCollection(Request.Host.Value);
            return Ok(collection);
        }

        // Загрузить файл в коллекцию
        [HttpPost("upload"), Authorize]
        public async Task<IActionResult> UploadToCollection(IFormFile file, [FromQuery] FileType fileType)
        {
            //http://localhost:5023/api/clothing/upload?fileType=3
            if (file == null || file.Length == 0)
            {
                return BadRequest("Файл не выбран или пустой.");
            }
            //todo проверка на админский token

            var url = await _fileService.UploadFileAsync(file, fileType, Request.Host.Value);
            return Ok(new { url });
        }
    }
}
