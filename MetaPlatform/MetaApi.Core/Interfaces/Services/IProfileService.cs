using MetaApi.Core.Domain.FittingHistory;


namespace MetaApi.Core.Interfaces.Services
{
    public interface IProfileService
    {
        Task<FittingProfile> GetProfile(int userId);
    }
}
