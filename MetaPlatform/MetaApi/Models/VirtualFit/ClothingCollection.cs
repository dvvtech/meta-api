namespace MetaApi.Models.VirtualFit
{
    public class ClothingCollection
    {
        /// <summary>
        /// Фото мужской одежды
        /// </summary>
        public ClothingItem[] ManСlothingItems { get; set; }

        /// <summary>
        /// Фото женской одежды
        /// </summary>
        public ClothingItem[] WomanСlothingItems { get; set; }

        /// <summary>
        /// Фото мужчин в одежде
        /// </summary>
        public ClothingItem[] Man { get; set; }

        /// <summary>
        /// Фото женщин в одежде
        /// </summary>
        public ClothingItem[] Woman { get; set; }
    }

    public class ClothingItem
    {
        public string Category { get; init; }

        public string Link { get; init; }
    }
}
