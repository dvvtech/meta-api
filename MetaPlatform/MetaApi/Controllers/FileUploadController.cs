using MetaApi.Models.VirtualFit;
using MetaApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MetaApi.Controllers
{
    [Route("api/uploads")]
    [ApiController]
    public class FileUploadController : ControllerBase
    {
        private readonly FileService _fileService;

        public FileUploadController(FileService fileService)
        {
            _fileService = fileService;
        }

        [HttpPost, Authorize]
        public async Task<ActionResult<string>> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Файл не выбран или пустой.");
            }

            try
            {
                var fileUrl = await _fileService.UploadFileAsync(file, FileType.Upload, Request.Host.Value);
                return Ok(new { url = fileUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Внутренняя ошибка сервера: {ex.Message}");
            }
        }        
    }
}
