
namespace MetaApi.Consts
{
    public static class FittingConstants
    {
        public const int PROMOCODE_CURRENT_LENGTH = 6;

        public const int PROMOCODE_MAX_LENGTH = 50;

        /// <summary>
        /// Ширина уменьшенной копии изображения
        /// </summary>
        public const int THUMBNAIL_WIDTH = 135;

        /// <summary>
        /// Окончание url для уменьшенных изображений
        /// </summary>
        public const string THUMBNAIL_SUFFIX_URL = "_t";

        /// <summary>
        /// Окончание url для больших изображений
        /// </summary>
        public const string FULLSIZE_SUFFIX_URL = "_v";

        /// <summary>
        /// Окончание url для больших изображений
        /// </summary>
        public const string PADDING_SUFFIX_URL = "_p";

        /// <summary>
        /// Отношение высоты к ширине
        /// </summary>
        public const float ASPECT_RATIO = 1.333f;

        /// <summary>
        /// Настройка качества JPEG
        /// </summary>
        public const int QUALITY_JPEG = 85;
    }
}
