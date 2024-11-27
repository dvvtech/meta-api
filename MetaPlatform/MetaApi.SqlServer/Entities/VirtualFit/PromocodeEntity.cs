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

        public int AttemptsLimit { get; set; }

        public DateTime CreatedUtcDate { get; set; }

        public DateTime UpdateUtcDate { get; set; }
    }
}
