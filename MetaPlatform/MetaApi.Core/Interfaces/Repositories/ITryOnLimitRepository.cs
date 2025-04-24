using MetaApi.Core.Domain.UserTryOnLimit;

namespace MetaApi.Core.Interfaces.Repositories
{
    public interface ITryOnLimitRepository
    {
        Task<UserTryOnLimit> GetLimit(int userId);
        Task AddLimit(UserTryOnLimit userTryOnLimitEntity);
        Task UpdateLimit(UserTryOnLimit userTryOnLimitEntity);
    }
}
