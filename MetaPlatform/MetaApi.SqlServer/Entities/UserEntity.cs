
namespace MetaApi.SqlServer.Entities
{
    public class UserEntity
    {
        public int Id { get; set; }

        /// <summary>
        /// Идентификатор пользователя из внешней системы (Google, VK и т.д.)
        /// </summary>
        public string ExternalId { get; set; }

        public string UserName { get; set; }
        public string JwtRefreshToken { get; set; }
        public string AuthJson { get; set; } // Это будет хранить JSON

        public AuthTypeEntity AuthType { get; set; }

        public RoleEntity Role { get; set; }

        /// <summary>
        /// Заблокирован ли пользователь
        /// </summary>
        public bool IsBlocked { get; set; }

        public DateTime CreatedUtcDate { get; set; }

        public DateTime UpdateUtcDate { get; set; }
    }

    public enum AuthTypeEntity
    {
        Unknown,
        Vk,
        Google
    }

    public enum RoleEntity
    {
        None = 0,
        User = 1,
        Admin = 2,
    }
}
