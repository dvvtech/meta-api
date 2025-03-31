using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaApi.SqlServer.Entities.VirtualFit
{
    public class PromocodeEntity
    {
        public int Id { get; set; }

        public string Promocode { get; set; }

        /// <summary>
        /// Необязятельное поле (имя человека которому выдал промокод)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Ограничение на кол-во вызовов
        /// </summary>
        public int UsageLimit { get; set; }

        /// <summary>
        /// Кол-во оставшихся вызовов
        /// </summary>
        public int RemainingUsage { get; set; }

        public DateTime CreatedUtcDate { get; set; }

        public DateTime UpdateUtcDate { get; set; }

        /// <summary>
        /// Коллекция связанных FittingResultEntity
        /// </summary>
        //public ICollection<FittingResultEntity> FittingResults { get; set; }
    }
}
