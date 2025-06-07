
using MetaApi.SqlServer.Entities.VirtualFit;

namespace MetaApi.SqlServer.Entities
{
    public class AccountEntity
    {
        public int Id { get; set; }

        /// <summary>
        /// Идентификатор пользователя из внешней системы (Google, VK и т.д.)
        /// </summary>
        public string ExternalId { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }
        public string JwtRefreshToken { get; set; }
        
        public AuthTypeEntity AuthType { get; set; }

        public RoleEntity Role { get; set; }

        /// <summary>
        /// Заблокирован ли пользователь
        /// </summary>
        public bool IsBlocked { get; set; }

        public DateTime CreatedUtcDate { get; set; }

        public DateTime UpdateUtcDate { get; set; }

        /// <summary>
        /// Коллекция связанных FittingResultEntity
        /// </summary>
        public ICollection<FittingResultEntity> FittingResults { get; set; }
    }

    public enum AuthTypeEntity
    {
        Unknown,
        Vk,
        Google,
        Yandex,
        MailRu
    }

    public enum RoleEntity
    {
        None = 0,
        User = 1,
        Admin = 2,
    }
}
