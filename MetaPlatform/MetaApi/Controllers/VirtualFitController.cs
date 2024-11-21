using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
using MetaApi.Utilities;
using MetaApi.Models.VirtualFit;

namespace MetaApi.Controllers
{
    [Route("api/virtual-fit")]
    [ApiController]
    public class VirtualFitController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IWebHostEnvironment _env;        

        public VirtualFitController(IHttpClientFactory httpClientFactory,
                                    IWebHostEnvironment env)
        {
            _httpClientFactory = httpClientFactory;
            _env = env;            
        }

        [HttpGet("test")]
        public string Test()
        {
            return "123";
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Файл не выбран или пустой.");
            }

            try
            {
                // Путь для сохранения файла
                var uploadsPath = Path.Combine(_env.WebRootPath, "uploads");                
                if (!Directory.Exists(uploadsPath))
                {
                    Directory.CreateDirectory(uploadsPath);
                }

                // Уникальное имя файла
                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

                // Полный путь для сохранения
                var filePath = Path.Combine(uploadsPath, uniqueFileName);

                // Сохранение файла
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Генерация публичной ссылки
                var fileUrl = $"{Request.Scheme}://{Request.Host}/uploads/{uniqueFileName}";
                return Ok(new { url = fileUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Внутренняя ошибка сервера: {ex.Message}");
            }
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendPostRequest([FromBody] Request requestData)
        {
            //модель https://replicate.com/cuuupid/idm-vton

            var httpClient = _httpClientFactory.CreateClient("ReplicateAPI");

            // Подготовьте данные для отправки            
            var internalRequestData = new PredictionRequest
            {
                Version = "c871bb9b046607b680449ecbae55fd8c6d945e0a1948644bf2361b3d021d3ff4",
                Input = new InputData
                {
                    Crop = false,
                    Seed = 42,
                    Steps = 30,
                    Category = "upper_body",//lower_body, dresses
                    ForceDc = false,
                    GarmImg = "https://ir.ozone.ru/s3/multimedia-1-r/wc1000/6906894111.jpg",//requestData.GarmImg,
                    HumanImg = "https://i.postimg.cc/vB7x1Vjs/IMG-6718.jpg",//requestData.HumanImg                                
                    MaskOnly = false,
                    GarmentDes = ""
                }
            };

            // Сериализуйте объект данных в JSON для отправки
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = new SnakeCaseNamingPolicy(),
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            };
            var jsonContent = JsonSerializer.Serialize(internalRequestData, options);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Отправьте POST запрос к другому сервису
            var response = await httpClient.PostAsync("predictions", content);

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
            }

            // Получаем ID созданного предсказания из первого ответа
            var responseContent = await response.Content.ReadAsStringAsync();
            using var document = JsonDocument.Parse(responseContent);
            var predictionId = document.RootElement.GetProperty("id").GetString();
            var status = document.RootElement.GetProperty("status").GetString();

            // Повторяем запрос на получение результата, если статус не "succeeded"            
            var checkUrl = $"predictions/{predictionId}";
            int maxRetries = 15;
            int retryCount = 0;
            int delay = 2000; // Задержка между запросами в миллисекундах (2 секунды)

            while (status != "succeeded" && status != "failed" && retryCount < maxRetries)
            {
                await Task.Delay(delay);
                var checkResponse = await httpClient.GetAsync(checkUrl);

                if (!checkResponse.IsSuccessStatusCode)
                {
                    var errorContent = await checkResponse.Content.ReadAsStringAsync();
                    return StatusCode((int)checkResponse.StatusCode, errorContent);
                }

                var checkContent = await checkResponse.Content.ReadAsStringAsync();
                using var checkDocument = JsonDocument.Parse(checkContent);

                // Обновляем статус и проверяем на завершение
                status = checkDocument.RootElement.GetProperty("status").GetString();

                if (status == "succeeded")
                {
                    var outputUrl = checkDocument.RootElement.GetProperty("output").GetString();
                    return Ok(outputUrl);
                }

                retryCount++;
            }

            // Если статус "failed" или истек лимит повторных попыток
            return StatusCode(500, status == "failed" ? "The process failed." : "The process did not complete in time.");
        }
    }
}
