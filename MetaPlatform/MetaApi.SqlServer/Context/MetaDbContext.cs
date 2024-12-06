using MetaApi.SqlServer.Configurations;
using MetaApi.SqlServer.Entities.VirtualFit;
using Microsoft.EntityFrameworkCore;

namespace MetaApi.SqlServer.Context
{
    public class MetaDbContext : DbContext
    {
        public DbSet<PromocodeEntity> Promocode { get; set; }

        public DbSet<FittingResultEntity> FittingResult { get; set; }

        public MetaDbContext(DbContextOptions<MetaDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new FittingResultConfiguration());
            modelBuilder.ApplyConfiguration(new PromocodeConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}
