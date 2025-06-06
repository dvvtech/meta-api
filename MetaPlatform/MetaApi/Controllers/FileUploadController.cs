﻿using MetaApi.Models.VirtualFit;
using MetaApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MetaApi.Controllers
{
    /// <summary>
    /// Загрузка пользовательских фото
    /// </summary>
    [Route("api/uploads")]
    [ApiController]
    public class FileUploadController : ControllerBase
    {
        private readonly IFileService _fileService;
        private readonly ILogger<FileUploadController> _logger;

        public FileUploadController(IFileService fileService, ILogger<FileUploadController> logger)
        {
            _fileService = fileService;
            _logger = logger;
        }

        [HttpPost, Authorize]
        public async Task<ActionResult<string>> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Файл не выбран или пустой.");
            }
            _logger.LogInformation("Upload1");
            try
            {
                var fileUrl = await _fileService.UploadFileAsync(file, FileType.Upload, Request.Host.Value);
                _logger.LogInformation("Upload2");
                return Ok(new { url = fileUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Внутренняя ошибка сервера: {ex.Message}");
            }
        }

        [HttpGet("test")]
        public IResult Test()
        {
            //_fileService.UploadResultFileAsync("https://a33140-9deb.k.d-f.pw/oldv2/woman/85983a8d-e655-447d-85d0-5233aec3d332_t.png", "", "");      
            return Results.Ok("777");
        }
    }
}
