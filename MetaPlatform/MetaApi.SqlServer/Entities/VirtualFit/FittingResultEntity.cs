
namespace MetaApi.SqlServer.Entities.VirtualFit
{
    public class FittingResultEntity
    {
        public int Id { get; set; }

        /// <summary>
        /// Фото одежды
        /// </summary>
        public string GarmentImgUrl { get; set; }

        public string HumanImgUrl { get; set; }

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
