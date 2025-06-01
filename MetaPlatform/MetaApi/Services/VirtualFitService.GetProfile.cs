using MetaApi.Core.Domain.FittingHistory;

namespace MetaApi.Services
{
    public partial class VirtualFitService
    {
        public async Task<FittingProfile> GetProfile(int userId)
        {
            var limit = await _tryOnLimitService.GetLimit(userId);

            var limitToday = $"({limit.AttemptsUsed}/{limit.MaxAttempts})";
            var totalAttemptsUsed = limit.TotalAttemptsUsed;

            DateTime dateOfLastFitting = await _fittingHistoryRepository.GetDateOfLastFittingAsync(userId);

            var fittingProfile = new FittingProfile
            {
                Name = "name",
                Email = "1@rt.ru",
                CountFittingToday = limitToday,
                TotalAttemptsUsed = limit.TotalAttemptsUsed.ToString(),
                LastFittingDate = dateOfLastFitting.ToString()
            };

            return fittingProfile;

            //IAccountRepository.Get(userId)
            //Name
            //Email
        }
    }
}
