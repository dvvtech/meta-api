
namespace MetaApi.SqlServer.Entities.VirtualHair
{
    public class HairHistoryEntity
    {
        public int Id { get; set; }

        /// <summary>
        /// Фото прически
        /// </summary>
        public string HairImgUrl { get; set; }

        /// <summary>
        /// Фото на кого примеряем прическу
        /// </summary>
        public string FaceImgUrl { get; set; }

        public string ResultImgUrl { get; set; }

        public int AccountId { get; set; }


        public DateTime CreatedUtcDate { get; set; }

        /// <summary>
        /// Флаг, указывающий на то, удалена ли запись
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Навигационное свойство на AccountEntity
        /// </summary>
        public AccountEntity Account { get; set; }
    }
}
