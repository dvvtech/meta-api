﻿namespace MetaApi.Models.VirtualFit
{
    public class FittingHistoryResponse
    {
        public int Id { get; set; }

        public string GarmentImgUrl { get; set; }

        public string HumanImgUrl { get; set; }

        public string ResultImgUrl { get; set; }

        /// <summary>
        /// Кол-во оставшихся попыток
        /// </summary>
        public int RemainingUsage { get; set; }
    }
}
