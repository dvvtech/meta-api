using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaApi.SqlServer.Entities.VirtualFit
{
    public class FittingResultEntity
    {
        public int Id { get; set; }

        /// <summary>
        /// Фото одежды
        /// </summary>
        public string GarmentImgUrl { get; set; }

        public string HumanImgUrl { get; set; }

        public string ResultImgUrl { get; set; }

        public int PromocodeId { get; set; }

        public DateTime CreatedUtcDate { get; set; }        
    }
}
