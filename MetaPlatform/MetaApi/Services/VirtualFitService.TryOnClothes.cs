using MetaApi.Core.OperationResults.Base;
using MetaApi.Core.OperationResults;
using MetaApi.Models.VirtualFit;
using MetaApi.Consts;
using MetaApi.Utilities;
using MetaApi.Core.Domain.FittingHistory;

namespace MetaApi.Services
{
    public partial class VirtualFitService
    {        
        /// <summary>
        /// Примерка одежды
        /// </summary>
        public async Task<Result<FittingResultResponse>> TryOnClothesAsync(FittingRequest request, string host, int userId)
        {            
            Result<string> predictionResult = await _replicateClientService.ProcessPredictionAsync(request);

            if (predictionResult.IsSuccess)
            {
                var urlResult = await _fileService.UploadResultFileAsync(predictionResult.Value, host, request.HumanImg);

                var fittingHistory = CreateFittingHistory(request, urlResult, userId);

                await _fittingHistoryRepository.AddToHistoryAsync(fittingHistory);

                await _tryOnLimitService.DecrementTryOnLimitAsync(userId);

                return Result<FittingResultResponse>.Success(new FittingResultResponse
                {
                    Url = urlResult ?? string.Empty,
                    RemainingUsage = await _tryOnLimitService.GetRemainingUsage(userId)
                });
            }
            else
            {                
                return Result<FittingResultResponse>.Failure(VirtualFitError.VirtualFitServiceError("Something went wrong"));
            }
        }        

        private FittingHistory CreateFittingHistory(FittingRequest request, string urlResult, int userId)
        {
            return FittingHistory.Create(
                accountId: userId,
                garmentImgUrl: ImageUrlHelper.GetUrl(request.GarmImg).Replace(FittingConstants.PADDING_SUFFIX_URL, FittingConstants.THUMBNAIL_SUFFIX_URL)
                                                                      .Replace(FittingConstants.FULLSIZE_SUFFIX_URL, FittingConstants.THUMBNAIL_SUFFIX_URL),
                humanImgUrl: ImageUrlHelper.GetUrl(request.HumanImg).Replace(FittingConstants.PADDING_SUFFIX_URL, FittingConstants.THUMBNAIL_SUFFIX_URL)
                                                                     .Replace(FittingConstants.FULLSIZE_SUFFIX_URL, FittingConstants.THUMBNAIL_SUFFIX_URL),
                resultImgUrl: urlResult.Replace(FittingConstants.FULLSIZE_SUFFIX_URL, FittingConstants.THUMBNAIL_SUFFIX_URL));            
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
