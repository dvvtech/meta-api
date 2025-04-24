using MetaApi.Core.OperationResults.Base;

namespace MetaApi.Services.Interfaces
{
    public interface IAuthService
    {
        Task<Result> LogoutAsync(int userId);

        Task<Result<MetaApi.Models.Auth.TokenResponse>> RefreshTokenAsync(string refreshToken);
    }
}
