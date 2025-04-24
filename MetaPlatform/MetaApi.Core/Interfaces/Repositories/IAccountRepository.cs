using MetaApi.Core.Domain.Account;

namespace MetaApi.Core.Interfaces.Repositories
{
    public interface IAccountRepository
    {
        Task<Account?> GetById(int userId);

        Task<Account?> GetByExternalId(string externalId);

        Task<Account?> GetByRefreshToken(string refreshToken);

        Task<int> Add(Account user);

        Task UpdateRefreshToken(Account accountEntity);

        Task SaveChanges();
    }
}
