using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace MetaApi.AppStart
{
    public partial class Startup
    {
        void ConfigureAuth()
        {
            _builder.Services.AddAuthorization();
            _builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme) //валидация токена из хедера (м/б из куки)
                .AddJwtBearer(option =>
                {
                    option.TokenValidationParameters = new()
                    {
                        //валидация издателя
                        ValidateIssuer = true,
                        //валидация получателя
                        ValidateAudience = true,
                        //валидация времени токена
                        ValidateLifetime = true,

                        RequireExpirationTime = true,
                        ClockSkew = TimeSpan.Zero,
                        //валидация ключа издателя (это key из конфигурации )
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = _jwtConf.Issuer,
                        ValidAudience = _jwtConf.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConf.Key))
                    };
                });
        }
    }
}
