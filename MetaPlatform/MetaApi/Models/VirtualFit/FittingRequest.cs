namespace MetaApi.Models.VirtualFit
{
    public class FittingRequest
    {
        /// <summary>
        /// Фото одежды, которую примерить
        /// </summary>
        public string GarmImg { get; set; }

        /// <summary>
        /// Фото человека, на которого примерять
        /// </summary>
        public string HumanImg { get; set; }

        /// <summary>
        /// Возможные значения: upper_body, lower_body, dresses
        /// </summary>
        public string Category { get; set; }
    }
}
