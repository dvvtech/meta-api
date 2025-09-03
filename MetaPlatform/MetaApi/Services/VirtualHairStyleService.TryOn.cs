using MetaApi.Consts;
using MetaApi.Core.Domain.Hair;
using MetaApi.Core.OperationResults;
using MetaApi.Core.OperationResults.Base;
using MetaApi.Utilities;

namespace MetaApi.Services
{
    public partial class VirtualHairStyleService
    {
        public async Task<Result<string>> TryOnAsync(HairTryOnData hairTryOnData)
        {
            Result<string> predictionResult = await _replicateClientService.ProcessPredictionAsync(hairTryOnData);

            if(predictionResult.IsSuccess)
            {
                _logger.LogInformation("Prediction success");

                string urlResult = await _fileService.UploadResultFileAsync(predictionResult.Value, hairTryOnData.Host, hairTryOnData.FaceImg);

                HairHistory hairHistory = CreateFittingHistory(hairTryOnData, urlResult);
                await _hairHistoryRepository.AddToHistoryAsync(hairHistory);

                await _tryOnLimitService.DecrementTryOnLimitAsync(hairTryOnData.AccountId);

                return Result<string>.Success(urlResult ?? string.Empty);
            }
            else
            {
                _logger.LogInformation($"Prediction error: {predictionResult.Error.Description}");
                return Result<string>.Failure(VirtualFitError.VirtualFitServiceError("Something went wrong"));
            }

            throw new NotImplementedException();
        }

        private HairHistory CreateFittingHistory(HairTryOnData data, string urlResult)
        {
            return HairHistory.Create(
                accountId: data.AccountId,
                hairImgImgUrl: ImageUrlHelper.GetUrl(data.HairImg).Replace(FittingConstants.PADDING_SUFFIX_URL, FittingConstants.THUMBNAIL_SUFFIX_URL)
                                                                      .Replace(FittingConstants.FULLSIZE_SUFFIX_URL, FittingConstants.THUMBNAIL_SUFFIX_URL),
                faceImgUrl: ImageUrlHelper.GetUrl(data.FaceImg).Replace(FittingConstants.PADDING_SUFFIX_URL, FittingConstants.THUMBNAIL_SUFFIX_URL)
                                                                     .Replace(FittingConstants.FULLSIZE_SUFFIX_URL, FittingConstants.THUMBNAIL_SUFFIX_URL),
                resultImgUrl: urlResult.Replace(FittingConstants.FULLSIZE_SUFFIX_URL, FittingConstants.THUMBNAIL_SUFFIX_URL));
        }
    }
}
