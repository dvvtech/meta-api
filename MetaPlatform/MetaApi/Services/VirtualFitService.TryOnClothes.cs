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
        public async Task<Result<(string ResultImageUrl, int RemainingUsage)>> TryOnClothesAsync(FittingData fittingData)
        {
            Result<string> predictionResult = await _replicateClientService.ProcessPredictionAsync(fittingData);
            
            if (predictionResult.IsSuccess)
            {
                string urlResult = await _fileService.UploadResultFileAsync(predictionResult.Value, fittingData.Host, fittingData.HumanImg);
                
                FittingHistory fittingHistory = CreateFittingHistory(fittingData, urlResult);

                await _fittingHistoryRepository.AddToHistoryAsync(fittingHistory);

                await _tryOnLimitService.DecrementTryOnLimitAsync(fittingData.AccountId);

                return Result<(string ResultImageUrl, int RemainingUsage)>.Success(
                (
                    urlResult ?? string.Empty,
                    await _tryOnLimitService.GetRemainingUsage(fittingData.AccountId)
                ));
            }
            else
            {                
                return Result<(string ResultImageUrl, int RemainingUsage)>.Failure(VirtualFitError.VirtualFitServiceError("Something went wrong"));
            }
        }        

        private FittingHistory CreateFittingHistory(FittingData data, string urlResult)
        {
            return FittingHistory.Create(
                accountId: data.AccountId,
                garmentImgUrl: ImageUrlHelper.GetUrl(data.GarmImg).Replace(FittingConstants.PADDING_SUFFIX_URL, FittingConstants.THUMBNAIL_SUFFIX_URL)
                                                                      .Replace(FittingConstants.FULLSIZE_SUFFIX_URL, FittingConstants.THUMBNAIL_SUFFIX_URL),
                humanImgUrl: ImageUrlHelper.GetUrl(data.HumanImg).Replace(FittingConstants.PADDING_SUFFIX_URL, FittingConstants.THUMBNAIL_SUFFIX_URL)
                                                                     .Replace(FittingConstants.FULLSIZE_SUFFIX_URL, FittingConstants.THUMBNAIL_SUFFIX_URL),
                resultImgUrl: urlResult.Replace(FittingConstants.FULLSIZE_SUFFIX_URL, FittingConstants.THUMBNAIL_SUFFIX_URL));            
        }


        public async Task<Result<FittingResultResponse>> TryOnClothesFakeAsync(FittingRequest request)
        {            
            await Task.Delay(30000);

            return Result<FittingResultResponse>.Success(new FittingResultResponse
            {
                Url = "https://a33140-9deb.k.d-f.pw/woman/8c4d8641-2373-4e0b-a6a9-64f76a584e47_t.png",
                RemainingUsage = 3
            });
        }
    }
}
