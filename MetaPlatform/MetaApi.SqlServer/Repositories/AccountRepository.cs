using MetaApi.Core.Domain.Account;
using MetaApi.Core.Interfaces.Repositories;
using MetaApi.SqlServer.Context;
using MetaApi.SqlServer.Entities;
using Microsoft.EntityFrameworkCore;

namespace MetaApi.SqlServer.Repositories
{    
    public class AccountRepository : IAccountRepository
    {
        private readonly MetaDbContext _dbContext;

        public AccountRepository(MetaDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Account?> GetById(int userId)
        {
            var accountEntity = await _dbContext.Accounts
                                                .AsNoTracking()
                                                .FirstOrDefaultAsync(user => user.Id == userId);

            return accountEntity == null ? null : CreateAccountFromEntity(accountEntity);
        }

        public async Task<Account?> GetByExternalId(string externalId)
        {
            var accountEntity = await _dbContext.Accounts
                                                .AsNoTracking()                                       
                                                .FirstOrDefaultAsync(user => user.ExternalId == externalId);

            return accountEntity == null ? null : CreateAccountFromEntity(accountEntity);
        }

        public async Task<Account?> GetByRefreshToken(string refreshToken)
        {
            var accountEntity = await _dbContext.Accounts
                                                .AsNoTracking()
                                                .FirstOrDefaultAsync(x => x.JwtRefreshToken == refreshToken);

            return accountEntity == null ? null : CreateAccountFromEntity(accountEntity);
        }

        public async Task<int> Add(Account account)
        {
            var newAccount = new AccountEntity
            {
                ExternalId = account.ExternalId,
                UserName = account.UserName,
                JwtRefreshToken = account.JwtRefreshToken,
                AuthType = (AuthTypeEntity)account.AuthType,
                Role = (RoleEntity)account.Role,
                CreatedUtcDate = DateTime.UtcNow,
                UpdateUtcDate = DateTime.UtcNow
            };

            await _dbContext.Accounts.AddAsync(newAccount);
            await _dbContext.SaveChangesAsync();
            return newAccount.Id;
        }

        public async Task UpdateRefreshToken(Account accountEntity)
        {
            await _dbContext.Accounts
                                    .Where(updateUser => updateUser.Id == accountEntity.Id)
                                    .ExecuteUpdateAsync(updateUser => updateUser                                        
                                        .SetProperty(c => c.JwtRefreshToken, accountEntity.JwtRefreshToken)
                                        .SetProperty(c => c.UpdateUtcDate, DateTime.UtcNow));
        }

        public async Task SaveChanges()
        {
            await _dbContext.SaveChangesAsync();
        }

        private Account CreateAccountFromEntity(AccountEntity accountEntity)
        {
            return Account.Create(                
                accountEntity.ExternalId,
                accountEntity.UserName,
                accountEntity.JwtRefreshToken,
                (AuthType)accountEntity.AuthType,
                (Role)accountEntity.Role                               
            );
        }
    }
}
