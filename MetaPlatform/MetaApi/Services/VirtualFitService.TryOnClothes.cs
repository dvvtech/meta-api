using MetaApi.Core.OperationResults.Base;
using MetaApi.Core.OperationResults;
using MetaApi.Models.VirtualFit;
using MetaApi.SqlServer.Entities.VirtualFit;
using MetaApi.Consts;
using MetaApi.Utilities;

namespace MetaApi.Services
{
    public partial class VirtualFitService
    {        
        /// <summary>
        /// Попытка примерки одежды.
        /// </summary>
        public async Task<Result<FittingResultResponse>> TryOnClothesAsync(FittingRequest request, string host, int userId)
        {            
            var predictionResult = await _replicateClientService.ProcessPredictionAsync(request);

            if (predictionResult.status == "succeeded")
            {
                var urlResult = await _fileService.UploadResultFileAsync(predictionResult.outputUrl, host, request.HumanImg);

                var fittingResultEntity = CreateFittingResultEntity(request, urlResult, userId);

                await _fittingHistoryCache.AddToHistory(fittingResultEntity);

                await _tryOnLimitService.DecrementTryOnLimitAsync(userId);

                return Result<FittingResultResponse>.Success(new FittingResultResponse
                {
                    Url = urlResult ?? string.Empty,
                    RemainingUsage = await _tryOnLimitService.GetRemainingUsage(userId)
                });
            }

            return Result<FittingResultResponse>.Failure(VirtualFitError.VirtualFitServiceError("Something went wrong"));            
        }        

        private FittingResultEntity CreateFittingResultEntity(FittingRequest request, string urlResult, int userId)
        {
            return new FittingResultEntity
            {
                GarmentImgUrl = ImageUrlHelper.GetUrl(request.GarmImg).Replace(FittingConstants.PADDING_SUFFIX_URL, FittingConstants.THUMBNAIL_SUFFIX_URL)
                                                                      .Replace(FittingConstants.FULLSIZE_SUFFIX_URL, FittingConstants.THUMBNAIL_SUFFIX_URL),
                HumanImgUrl = ImageUrlHelper.GetUrl(request.HumanImg).Replace(FittingConstants.PADDING_SUFFIX_URL, FittingConstants.THUMBNAIL_SUFFIX_URL)
                                                                     .Replace(FittingConstants.FULLSIZE_SUFFIX_URL, FittingConstants.THUMBNAIL_SUFFIX_URL),
                ResultImgUrl = urlResult.Replace(FittingConstants.FULLSIZE_SUFFIX_URL, FittingConstants.THUMBNAIL_SUFFIX_URL),
                AccountId = userId,
                CreatedUtcDate = DateTime.UtcNow
            };
        }


        public async Task<Result<FittingResultResponse>> TryOnClothesFakeAsync(FittingRequest request)
        {            
            await Task.Delay(5000);

            return Result<FittingResultResponse>.Success(new FittingResultResponse
            {
                Url = "https://a30944-8332.x.d-f.pw/result/d211d593-59b4-497b-8368-8d13b14f8dc1.jpg",
                RemainingUsage = 3
            });
        }
    }
}
