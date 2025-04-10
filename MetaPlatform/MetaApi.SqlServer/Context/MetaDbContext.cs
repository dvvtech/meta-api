using MetaApi.SqlServer.Configurations;
using MetaApi.SqlServer.Entities;
using MetaApi.SqlServer.Entities.VirtualFit;
using Microsoft.EntityFrameworkCore;

namespace MetaApi.SqlServer.Context
{
    public class MetaDbContext : DbContext
    {        
        public DbSet<FittingResultEntity> FittingResult { get; set; }

        public DbSet<AccountEntity> Accounts { get; set; }

        public DbSet<UserTryOnLimitEntity> UserTryOnLimits { get; set; }

        public MetaDbContext(DbContextOptions<MetaDbContext> options) : base(options)
        {
            //Database.EnsureCreated();
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new FittingResultConfiguration());            
            modelBuilder.ApplyConfiguration(new AccountConfiguration());
            modelBuilder.ApplyConfiguration(new UserTryOnLimitConfiguration());

            base.OnModelCreating(modelBuilder);

            //Seed(modelBuilder);
        }

        private void Seed(ModelBuilder modelBuilder)
        {
            /*DateTime dtUtcNow = DateTime.UtcNow;
            modelBuilder.Entity<PromocodeEntity>().HasData(
                new PromocodeEntity
                { 
                    Id = 1,
                    Promocode = "PRBA34YNI9!QWC7IZS",
                    Name = "admin",
                    UsageLimit = 500000,
                    RemainingUsage = 500000,
                    CreatedUtcDate = dtUtcNow,
                    UpdateUtcDate = dtUtcNow,
                });*/
        }
    }
}
