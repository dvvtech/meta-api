using MetaApi.Core.Domain.FittingHistory;
using MetaApi.Core.Interfaces.Repositories;
using MetaApi.Core.Interfaces.Services;
using Microsoft.Extensions.Logging;


namespace MetaApi.Core.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IFittingHistoryRepository _fittingHistoryRepository;
        private readonly ITryOnLimitService _tryOnLimitService;
        private readonly IAccountRepository _accountRepository;
        private readonly ILogger<ProfileService> _logger;

        public ProfileService(IFittingHistoryRepository fittingHistoryRepository,
                              ITryOnLimitService tryOnLimitService,
                              IAccountRepository accountRepository,
                              ILogger<ProfileService> logger)
        {
            _fittingHistoryRepository = fittingHistoryRepository;
            _tryOnLimitService = tryOnLimitService;
            _accountRepository = accountRepository;
            _logger = logger;
        }

        public async Task<FittingProfile> GetProfile(int userId)
        {
            var limit = await _tryOnLimitService.GetLimit(userId);

            var limitToday = $"({limit.AttemptsUsed}/{limit.MaxAttempts})";
            var totalAttemptsUsed = limit.TotalAttemptsUsed;

            DateTime dateOfLastFitting = await _fittingHistoryRepository.GetDateOfLastFittingAsync(userId);

            var account = await _accountRepository.GetById(userId);
            
            _logger.LogInformation($"accountId: {account.Id}");
            
            var fittingProfile = new FittingProfile
            {
                Name = $"{account.UserName} ({account.AuthType.ToString()})",
                Email = account.Email,
                CountFittingToday = limitToday,
                TotalAttemptsUsed = limit.TotalAttemptsUsed.ToString(),
                LastFittingDate = dateOfLastFitting.ToString("dd MM yyyy HH:mm:ss"),                
            };

            return fittingProfile;
        }
    }
}
