namespace MetaApi.Models.VirtualFit
{
    public class ClothingCollectionResponse
    {
        /// <summary>
        /// Фото мужской одежды
        /// </summary>
        public ClothingItemDto[] ManСlothingItems { get; set; }

        /// <summary>
        /// Фото женской одежды
        /// </summary>
        public ClothingItemDto[] WomanСlothingItems { get; set; }

        /// <summary>
        /// Фото мужчин в одежде
        /// </summary>
        public ClothingItemDto[] Man { get; set; }

        /// <summary>
        /// Фото женщин в одежде
        /// </summary>
        public ClothingItemDto[] Woman { get; set; }
    }

    public class ClothingItemDto
    {
        public string Link { get; init; }
    }
}
