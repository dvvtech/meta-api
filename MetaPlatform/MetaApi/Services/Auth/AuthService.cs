using MetaApi.Core;
using MetaApi.Core.OperationResults;
using MetaApi.Core.OperationResults.Base;
using MetaApi.SqlServer.Entities;
using MetaApi.SqlServer.Repositories;

namespace MetaApi.Services.Auth
{
    public class AuthService
    {        
        private readonly AccountRepository _accountRepository;
        private readonly JwtProvider _jwtProvider;

        public AuthService(AccountRepository accountRepository,
                           JwtProvider jwtProvider)
        {            
            _accountRepository = accountRepository;
            _jwtProvider = jwtProvider;
        }

        public async Task<Result> LogoutAsync(int userId)
        {
            AccountEntity userEntity = await _accountRepository.GetById(userId);            
            if (userEntity is null)
            {
                return Result.Failure(UserErrors.UserNotFound("logout:"));
            }

            userEntity.JwtRefreshToken = string.Empty;
            await _accountRepository.UpdateRefreshToken(userEntity);

            return Result.Success();
        }

        public async Task<Result<MetaApi.Models.Auth.TokenResponse>> RefreshTokenAsync(string refreshToken)
        {
            AccountEntity? account = await _accountRepository.GetByRefreshToken(refreshToken);
            if (account is null)
            {
                return Result<MetaApi.Models.Auth.TokenResponse>.Failure(UserErrors.UserNotFound("refresh token:"));
            }
            if (account.IsBlocked)
            {
                return Result<MetaApi.Models.Auth.TokenResponse>.Failure(UserErrors.UserIsBlocked());
            }

            var newAccessToken = _jwtProvider.GenerateToken(account.UserName, account.Id);
            var newRefreshToken = _jwtProvider.GenerateRefreshToken();

            account.JwtRefreshToken = newRefreshToken;
            await _accountRepository.UpdateRefreshToken(account);

            return Result<MetaApi.Models.Auth.TokenResponse>.Success(new MetaApi.Models.Auth.TokenResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            });
        }
    }
}
