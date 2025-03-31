using MetaApi.SqlServer.Context;
using MetaApi.SqlServer.Entities;
using Microsoft.EntityFrameworkCore;

namespace MetaApi.SqlServer.Repositories
{
    public class AccountRepository
    {
        private readonly MetaDbContext _dbContext;

        public AccountRepository(MetaDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<AccountEntity?> GetByExternalId(string externalId)
        {
            return await _dbContext.Accounts
                                   .AsNoTracking()                                       
                                   .FirstOrDefaultAsync(user => user.ExternalId == externalId);            
        }

        public async Task<AccountEntity?> GetByRefreshToken(string refreshToken)
        {
            return await _dbContext.Accounts
                                   .AsNoTracking()
                                   .FirstOrDefaultAsync(x => x.JwtRefreshToken == refreshToken);            
        }

        public async Task<int> Add(AccountEntity user)
        {
            await _dbContext.Accounts.AddAsync(user);
            await _dbContext.SaveChangesAsync();
            return user.Id;
        }

        public async Task UpdateRefreshToken(AccountEntity accountEntity)
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
    }
}
