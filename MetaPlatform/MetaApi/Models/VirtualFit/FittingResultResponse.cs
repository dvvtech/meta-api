namespace MetaApi.Models.VirtualFit
{
    public class FittingResultResponse
    {
        /// <summary>
        /// Url изображение с результатом примерки
        /// </summary>
        public string Url {  get; set; }

        /// <summary>
        /// Кол-во оставшихся попыток
        /// </summary>
        public int RemainingUsage { get; set; }        
    }
}
