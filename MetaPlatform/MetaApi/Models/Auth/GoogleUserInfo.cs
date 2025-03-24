using System.Text.Json.Serialization;

namespace MetaApi.Models.Auth
{
    public class GoogleUserInfo
    {
        public string Sub { get; set; } // Уникальный идентификатор пользователя
        public string Email { get; set; }

        /// <summary>
        /// Полное имя пользователя.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Имя пользователя.
        /// </summary>
        [JsonPropertyName("given_name")]
        public string GivenName { get; set; }

        /// <summary>
        /// Фамилия пользователя.
        /// </summary>
        [JsonPropertyName("family_name")]
        public string FamilyName { get; set; }

        /// <summary>
        /// Ссылка на изображение профиля пользователя.
        /// </summary>
        public string Picture { get; set; }
        // Другие поля, если необходимо
    }
}
