using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaApi.SqlServer.Context
{
    public class MetaContext : DbContext
    {
        public DbSet<PromocodeEntity> Promocode { get; set; }
    }
}
