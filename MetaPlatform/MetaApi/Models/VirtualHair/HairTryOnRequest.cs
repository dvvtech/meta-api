namespace MetaApi.Models.VirtualHair
{
    public class HairTryOnRequest
    {
        /// <summary>
        /// Фото, на кого примерить прическу
        /// </summary>
        public string FaceImg { get; set; }

        /// <summary>
        /// Фото прически
        /// </summary>
        public string HairImg { get; set; }
    }
}
