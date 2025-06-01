using MetaApi.Core.Domain.FittingHistory;
using MetaApi.Core.Interfaces.Repositories;
using MetaApi.Core.Interfaces.Services;


namespace MetaApi.Core.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IFittingHistoryRepository _fittingHistoryRepository;
        private readonly ITryOnLimitService _tryOnLimitService;
        private readonly IAccountRepository _accountRepository;

        public ProfileService(IFittingHistoryRepository fittingHistoryRepository,
                              ITryOnLimitService tryOnLimitService,
                              IAccountRepository accountRepository)
        {
            _fittingHistoryRepository = fittingHistoryRepository;
            _tryOnLimitService = tryOnLimitService;
            _accountRepository = accountRepository;
        }

        public async Task<FittingProfile> GetProfile(int userId)
        {
            var limit = await _tryOnLimitService.GetLimit(userId);

            var limitToday = $"({limit.AttemptsUsed}/{limit.MaxAttempts})";
            var totalAttemptsUsed = limit.TotalAttemptsUsed;

            DateTime dateOfLastFitting = await _fittingHistoryRepository.GetDateOfLastFittingAsync(userId);

            var account = await _accountRepository.GetById(userId);

            var fittingProfile = new FittingProfile
            {
                Name = account.UserName,
                Email = "qwe@er.ru",
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
