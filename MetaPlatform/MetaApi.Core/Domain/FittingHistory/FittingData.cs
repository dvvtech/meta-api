
namespace MetaApi.Core.Domain.FittingHistory
{
    public class FittingData
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

        public int AccountId { get; set; }

        public string Host { get; set; }
    }
}
