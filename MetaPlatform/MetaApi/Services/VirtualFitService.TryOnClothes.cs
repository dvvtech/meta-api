﻿using MetaApi.Core.OperationResults.Base;
using MetaApi.Core.OperationResults;
using MetaApi.Models.VirtualFit;
using MetaApi.SqlServer.Entities.VirtualFit;
using MetaApi.Utilities;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text;
using MetaApi.Consts;

namespace MetaApi.Services
{
    public partial class VirtualFitService
    {
        public async Task<Result<FittingResultResponse>> TryOnClothesFakeAsync(FittingRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Promocode) ||
                request.Promocode.Length < FittingConstants.PROMOCODE_MAX_LENGTH)
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
                Url = "https://replicate.delivery/yhqm/ieUAqd1K7ekDZkNcPBVUC4D6YIRAO5HLLIbgKHJa6MG24K4TA/output.jpg",
                RemainingUsage = promocode.RemainingUsage
            });
        }
        /// <summary>
        /// Попытка примерки одежды.
        /// </summary>
        public async Task<Result<FittingResultResponse>> TryOnClothesAsync(FittingRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Promocode) ||
                request.Promocode.Length < FittingConstants.PROMOCODE_MAX_LENGTH)
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
                    GarmImg = request.GarmImg,
                    HumanImg = request.HumanImg,
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

                    var fitingResult = new FittingResultEntity
                    {
                        GarmentImgUrl = request.GarmImg,
                        HumanImgUrl = request.HumanImg,
                        ResultImgUrl = outputUrl ?? string.Empty,
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
                        Url = outputUrl ?? string.Empty,
                        RemainingUsage = promocode.RemainingUsage
                    });
                }

                retryCount++;
            }

            return Result<FittingResultResponse>.Failure(VirtualFitError.VirtualFitServiceError("Something went wrong"));
        }
    }
}
