namespace MetaApi.Models.VirtualFit
{
    public class Request
    {
        /// <summary>
        /// Фото одежды, которую примерить
        /// </summary>
        public string GarmImg { get; set; }

        /// <summary>
        /// Фото человека, на которого примерять
        /// </summary>
        public string HumanImg { get; set; }

        public string Promocode {  get; set; }
    }
}
