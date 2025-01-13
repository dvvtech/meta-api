using MetaApi.Core.OperationResults.Base;
using MetaApi.Core.OperationResults;
using MetaApi.Models.VirtualFit;
using MetaApi.SqlServer.Entities.VirtualFit;
using MetaApi.Utilities;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text;
using MetaApi.Consts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace MetaApi.Services
{
    public partial class VirtualFitService
    {
        private string GetUrl(string url)
        {            
            string imgFileName = System.IO.Path.GetFileNameWithoutExtension(url);
            var count = imgFileName.Count(s => s == '_');
            if (count == 1)
            {
                //todo если оканчивается на _t то заменить на _v
                if (imgFileName.EndsWith("_t", StringComparison.OrdinalIgnoreCase))
                {
                    return url.Replace("_t", FittingConstants.FULLSIZE_SUFFIX_URL);
                }

                return url;
            }

            /*if (imgFileName.EndsWith("_p", StringComparison.OrdinalIgnoreCase))
            {
                return url;
            }*/

            //если фото из внутренней коллекции и для него есть паддинг то нужно заменить название файла чтоб оканчивался на _p
            if (imgFileName.EndsWith("_v", StringComparison.OrdinalIgnoreCase))
            {
                return url.Replace("_v", FittingConstants.PADDING_SUFFIX_URL);                
            }

            //если фото из внутренней коллекции 
            if (imgFileName.EndsWith("_t", StringComparison.OrdinalIgnoreCase))
            {
                return url.Replace("_t", FittingConstants.PADDING_SUFFIX_URL);
            }

            throw new Exception("internal error");
        }

        /// <summary>
        /// Попытка примерки одежды.
        /// </summary>
        public async Task<Result<FittingResultResponse>> TryOnClothesAsync(FittingRequest request, string host)
        {
            if (string.IsNullOrWhiteSpace(request.Promocode) ||
                request.Promocode.Length > FittingConstants.PROMOCODE_MAX_LENGTH)
            {
                return Result<FittingResultResponse>.Failure(VirtualFitError.NotValidPromocodeError());
            }

            PromocodeEntity? promocode = await _metaDbContext.Promocode.FirstOrDefaultAsync(p => p.Promocode == request.Promocode);
            if (promocode == null)
            {
                return Result<FittingResultResponse>.Failure(VirtualFitError.NotValidPromocodeError());
            }
            if (promocode.RemainingUsage <= 0)
            {
                return Result<FittingResultResponse>.Failure(VirtualFitError.LimitIsOverError());
            }

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
                    Category = request.Category,
                    ForceDc = false,
                    GarmImg = GetUrl(request.GarmImg),
                    HumanImg = GetUrl(request.HumanImg),
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
                return Result<FittingResultResponse>.Failure(VirtualFitError.ThirdPartyServiceError(errorContent, response.StatusCode));
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
                    return Result<FittingResultResponse>.Failure(VirtualFitError.ThirdPartyServiceError(errorContent, checkResponse.StatusCode));
                }

                var checkContent = await checkResponse.Content.ReadAsStringAsync();
                using var checkDocument = JsonDocument.Parse(checkContent);

                // Обновляем статус и проверяем на завершение
                status = checkDocument.RootElement.GetProperty("status").GetString();

                if (status == "succeeded")
                {
                    var outputUrl = checkDocument.RootElement.GetProperty("output").GetString();
                    if (string.IsNullOrEmpty(outputUrl))
                    {
                        throw new InvalidOperationException("Output URL is missing or empty.");
                    }
                                        
                    string urlResult = await UploadResultFileAsync(outputUrl, host, request.HumanImg);

                    var fitingResult = new FittingResultEntity
                    {
                        GarmentImgUrl = request.GarmImg.Replace("_p", FittingConstants.THUMBNAIL_SUFFIX_URL).Replace("_v", FittingConstants.THUMBNAIL_SUFFIX_URL),
                        HumanImgUrl = request.HumanImg.Replace("_p", FittingConstants.THUMBNAIL_SUFFIX_URL).Replace("_v", FittingConstants.THUMBNAIL_SUFFIX_URL),
                        ResultImgUrl = urlResult.Replace("_v", FittingConstants.THUMBNAIL_SUFFIX_URL),
                        PromocodeId = promocode.Id,
                        CreatedUtcDate = DateTime.UtcNow,
                    };

                    _metaDbContext.FittingResult.Add(fitingResult);
                    promocode.RemainingUsage--;
                    promocode.UpdateUtcDate = DateTime.UtcNow;
                    // Сохранение в базе данных
                    await _metaDbContext.SaveChangesAsync();

                    return Result<FittingResultResponse>.Success(new FittingResultResponse
                    {
                        Url = urlResult ?? string.Empty,
                        RemainingUsage = promocode.RemainingUsage
                    });
                }

                retryCount++;
            }

            return Result<FittingResultResponse>.Failure(VirtualFitError.VirtualFitServiceError("Something went wrong"));
        }

        public static string ReplaceEndingToT(string input)
        {            
            return input.Substring(0, input.Length - 2) + "_t";            
        }

        private double? GetImageRatio(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return null;

            var parts = fileName.Split('_');
            if (parts.Length <= 1) return null;

            return double.TryParse(parts[1], out var ratio) ? ratio : null;
        }

        private string AppendSuffixToUrl(string url, string suffix)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentException("URL не может быть пустым или null", nameof(url));
            }

            ReadOnlySpan<char> urlSpan = url.AsSpan();
            int lastDotIndex = urlSpan.LastIndexOf('.');

            if (lastDotIndex == -1 || lastDotIndex <= urlSpan.LastIndexOf('/'))
            {
                throw new ArgumentException("Некорректный формат URL: отсутствует расширение файла", nameof(url));
            }

            Span<char> result = stackalloc char[url.Length + suffix.Length];
            urlSpan.Slice(0, lastDotIndex).CopyTo(result);
            suffix.AsSpan().CopyTo(result.Slice(lastDotIndex));
            urlSpan.Slice(lastDotIndex).CopyTo(result.Slice(lastDotIndex + suffix.Length));

            return new string(result);
        }

        private async Task<string> UploadResultFileAsync(string imageUrl, string host, string humanImg)
        {
            byte[] imageBytes = await GetImageResult(imageUrl);

            var image = Image.Load(imageBytes);

            double? imageRatio = GetImageRatio(humanImg);
            if (imageRatio != null)
            {
                PerformCropping(image, imageRatio.Value);
            }

            // Сохранение файла на диск
            var uniqueFileName = await SaveFileAsync(imageUrl, image, FileType.Result);

            // Генерация публичной ссылки
            return GenerateFileUrl(uniqueFileName, FileType.Result, host);
        }

        public async Task<byte[]> GetImageResult(string imageUrl)
        {            
            byte[] imageBytes;
            using (var httpClient = _httpClientFactory.CreateClient())
            {
                imageBytes = await httpClient.GetByteArrayAsync(imageUrl);
            }

            return imageBytes;
        }

        private async Task<string> SaveFileAsync(string imageUrl, Image image, FileType fileType)
        {
            var uploadsPath = Path.Combine(_env.WebRootPath, fileType.GetFolderName());
            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
            }

            // Определение расширения файла из URL 
            // (берём часть пути из URL и извлекаем расширение)
            var uri = new Uri(imageUrl);
            var extension = Path.GetExtension(uri.AbsolutePath);
            if (string.IsNullOrWhiteSpace(extension))
            {
                // Если невозможно определить расширение, зададим по умолчанию .jpg
                extension = ".jpg";
            }

            string fileName = Guid.NewGuid().ToString();
            string uniqueFileName = $"{fileName}_v{extension}";
            string filePath = Path.Combine(uploadsPath, uniqueFileName);

            // Сохраняем оригинальный файл            
            image.Save(filePath, new JpegEncoder
            {
                Quality = FittingConstants.QUALITY_JPEG
            });

            //Сохраняем уменьшенную копию для раздела история            
            byte[] resizedBytes = ImageResizer.ResizeImage(image, FittingConstants.THUMBNAIL_WIDTH);
            string newFileName = $"{fileName}_t{extension}";
            string newfilePath = Path.Combine(uploadsPath, newFileName);
            await File.WriteAllBytesAsync(newfilePath, resizedBytes);

            return uniqueFileName;
        }

        public async Task<Result<FittingResultResponse>> TryOnClothesFakeAsync(FittingRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Promocode) ||
                request.Promocode.Length > FittingConstants.PROMOCODE_MAX_LENGTH)
            {
                return Result<FittingResultResponse>.Failure(VirtualFitError.NotValidPromocodeError());
            }

            PromocodeEntity? promocode = await _metaDbContext.Promocode.FirstOrDefaultAsync(p => p.Promocode == request.Promocode);
            if (promocode == null)
            {
                return Result<FittingResultResponse>.Failure(VirtualFitError.NotValidPromocodeError());
            }
            if (promocode.RemainingUsage <= 0)
            {
                return Result<FittingResultResponse>.Failure(VirtualFitError.LimitIsOverError());
            }

            await Task.Delay(5000);

            promocode.RemainingUsage--;
            promocode.UpdateUtcDate = DateTime.UtcNow;
            // Сохранение в базе данных
            await _metaDbContext.SaveChangesAsync();

            return Result<FittingResultResponse>.Success(new FittingResultResponse
            {
                Url = "https://a30944-8332.x.d-f.pw/result/d211d593-59b4-497b-8368-8d13b14f8dc1.jpg",
                RemainingUsage = promocode.RemainingUsage
            });
        }
    }
}
