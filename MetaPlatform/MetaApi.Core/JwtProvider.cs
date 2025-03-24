using MetaApi.Core.Configurations;
using MetaApi.Core.Domain.User;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace MetaApi.Core
{
    public class JwtProvider
    {
        private readonly JwtConfig _options;

        public JwtProvider(IOptions<JwtConfig> options)
        {
            _options = options.Value;
        }

        /// <summary>
        /// Genetate jwt access token
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public string GenerateToken(string userName, string userId)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, userName),
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Role, Role.User.ToString())
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new JwtSecurityToken(_options.Issuer,
                                                       _options.Audience,
                                                       claims,
                                                       expires: DateTime.Now.Add(_options.ExpiresDuration),
                                                       signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }

        /// <summary>
        /// Generate refresh token
        /// </summary>
        /// <returns></returns>
        public string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }
    }
}
