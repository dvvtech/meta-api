
namespace MetaApi.Core.Domain.Hair
{
    public class HairCollection
    {
        /// <summary>
        /// Фото мужчижских причесок
        /// </summary>
        public HairItem[] Man { get; set; }

        /// <summary>
        /// Фото женских причесок
        /// </summary>
        public HairItem[] Woman { get; set; }
    }

    public class HairItem
    {
        public string Link { get; init; }
    }
}
