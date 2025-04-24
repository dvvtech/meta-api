
namespace MetaApi.Core.Interfaces.Services
{
    public interface ITryOnLimitService
    {
        Task<int> GetRemainingUsage(int userId);

        Task<TimeSpan> GetTimeUntilLimitResetAsync(int userId);

        Task<bool> CanUserTryOnAsync(int userId);

        Task DecrementTryOnLimitAsync(int userId);

        Task<int> GetRemainingTriesAsync(int userId);
    }
}
