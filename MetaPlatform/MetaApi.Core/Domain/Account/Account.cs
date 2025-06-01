
namespace MetaApi.Core.Domain.Account
{
    public class Account
    {
        public int Id { get; set; }

        /// <summary>
        /// Идентификатор пользователя из внешней системы (Google, VK и т.д.)
        /// </summary>
        public string ExternalId { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public string JwtRefreshToken { get; set; }

        public AuthType AuthType { get; set; }

        public Role Role { get; set; }

        /// <summary>
        /// Заблокирован ли пользователь
        /// </summary>
        public bool IsBlocked { get; set; }

        private Account(string externalId,
                        string userName,
                        string email,
                        string jwtRefreshToken,
                        AuthType authType,
                        Role role)
        {            
            ExternalId = externalId;
            UserName = userName;
            Email = email;
            JwtRefreshToken = jwtRefreshToken;
            AuthType = authType;
            Role = role;
        }

        private Account(int id,
                        string externalId,
                        string userName,
                        string email, 
                        string jwtRefreshToken,
                        AuthType authType,
                        Role role)
        {
            Id = id;
            ExternalId = externalId;
            UserName = userName;
            Email = email;
            JwtRefreshToken = jwtRefreshToken;
            AuthType = authType;
            Role = role;
        }

        public static Account Create(string externalId,
                                     string userName,
                                     string email,
                                     string jwtRefreshToken,
                                     AuthType authType,
                                     Role role)
        {
            return new Account(externalId, userName, email, jwtRefreshToken, authType, role);
        }

        public static Account Create(int id,
                                     string externalId,
                                     string userName,
                                     string email,
                                     string jwtRefreshToken,
                                     AuthType authType,
                                     Role role)
        {
            return new Account(id, externalId, userName, email, jwtRefreshToken, authType, role);
        }
    }
}
