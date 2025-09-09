
namespace MetaApi.Models.VirtualHair
{
    public class HairCollectionResponse
    {        
        /// <summary>
        /// Фото мужчских причесок
        /// </summary>
        public HairItemDto[] Man { get; set; }

        /// <summary>
        /// Фото женских причесок
        /// </summary>
        public HairItemDto[] Woman { get; set; }
    }

    public class HairItemDto
    {
        public string Link { get; init; }
    }
}
