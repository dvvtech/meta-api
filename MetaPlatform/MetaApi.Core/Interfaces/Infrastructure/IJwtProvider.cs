
namespace MetaApi.Core.Interfaces.Infrastructure
{
    public interface IJwtProvider
    {
        string GenerateToken(string userName, int accountId);

        string GenerateRefreshToken();
    }
}
