using MetaApi.Models.VirtualFit;
using MetaApi.SqlServer.Context;
using MetaApi.Utilities;
using System.Text.Json;
using System.Text;
using MetaApi.Core.OperationResults.Base;
using MetaApi.Core.OperationResults;
using MetaApi.SqlServer.Entities.VirtualFit;

namespace MetaApi.Services
{
    public class VirtualFitService
    {
        private readonly MetaDbContext _metaDbContext;
        private readonly IHttpClientFactory _httpClientFactory;

        public VirtualFitService(MetaDbContext metaContext,
                                 IHttpClientFactory httpClientFactory)
        {
            _metaDbContext = metaContext;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<string> GeneratePromocode(GeneratePromocodeRequest request)
        {
            // Генерация уникального промокода
            var promocode = GenerateUniquePromocode();

            // Создание сущности промокода
            var promocodeEntity = new PromocodeEntity
            {
                Promocode = promocode,
                Name = request.Name,
                UsageLimit = request.UsageLimit,
                RemainingUsage = request.UsageLimit,
                CreatedUtcDate = DateTime.UtcNow,
                UpdateUtcDate = DateTime.UtcNow
            };

            // Сохранение в базе данных
            _metaDbContext.Promocode.Add(promocodeEntity);
            await _metaDbContext.SaveChangesAsync();

            return promocode;
        }

        /// <summary>
        /// Попытка примерки одежды. Возвращает url результирующего изображения
        /// </summary>
        public async Task<Result<string>> TryOnClothesAsync(Request requestData)
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
                var errorContent = await response.Content.ReadAsStringAsync();
                return Result<string>.Failure(VirtualFitError.ThirdPartyServiceError(errorContent, response.StatusCode));                
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
                    return Result<string>.Failure(VirtualFitError.ThirdPartyServiceError(errorContent, checkResponse.StatusCode));                    
                }

                var checkContent = await checkResponse.Content.ReadAsStringAsync();
                using var checkDocument = JsonDocument.Parse(checkContent);

                // Обновляем статус и проверяем на завершение
                status = checkDocument.RootElement.GetProperty("status").GetString();

                if (status == "succeeded")
                {
                    var outputUrl = checkDocument.RootElement.GetProperty("output").GetString();
                    return Result<string>.Success(outputUrl);                    
                }

                retryCount++;
            }

            return Result<string>.Failure(VirtualFitError.VirtualFitServiceError("Something went wrong"));
        }

        private string GenerateUniquePromocode()
        {
            string promocode;
            var random = new Random();

            do
            {
                // Генерация промокода из 6 символов (буквы и цифры)
                promocode = new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", 6)
                    .Select(s => s[random.Next(s.Length)]).ToArray());
            }
            while (_metaDbContext.Promocode.Any(p => p.Promocode == promocode)); // Проверка уникальности

            return promocode;
        }
    }
}
