
namespace MetaApi.Core.Domain.FittingHistory
{
    public class FittingProfile
    {
        public string Name { get; set; }
        
        public string Email { get; set; }

        /// <summary>
        /// Кол-во примерок сегодня
        /// </summary>
        public string CountFittingToday { get; set; }

        /// <summary>
        /// Общее кол-во примерок
        /// </summary>
        public string TotalAttemptsUsed { get; set; }

        /// <summary>
        /// Дата последней примерки
        /// </summary>
        public string LastFittingDate { get; set; }
    }
}
