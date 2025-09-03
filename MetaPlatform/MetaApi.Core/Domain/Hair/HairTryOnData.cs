using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaApi.Core.Domain.Hair
{
    public class HairTryOnData
    {
        /// <summary>
        /// Фото, на кого примерить прическу
        /// </summary>
        public string FaceImg { get; set; }

        /// <summary>
        /// Фото прически
        /// </summary>
        public string HairImg { get; set; }

        public int AccountId { get; set; }

        public string Host { get; set; }
    }
}
