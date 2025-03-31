using MetaApi.SqlServer.Entities.VirtualFit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MetaApi.SqlServer.Configurations
{
    public class FittingResultConfiguration : IEntityTypeConfiguration<FittingResultEntity>
    {
        public void Configure(EntityTypeBuilder<FittingResultEntity> builder)
        {
            // Таблица
            builder.ToTable("FittingResults");

            // Первичный ключ
            builder.HasKey(f => f.Id);

            // Поля
            builder.Property(f => f.GarmentImgUrl)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(f => f.HumanImgUrl)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(f => f.ResultImgUrl)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(f => f.CreatedUtcDate)
                .IsRequired()
                .HasColumnType("datetime2");

            // Связь с PromocodeEntity (внешний ключ)
            builder.HasOne(f => f.Account)
                .WithMany(p => p.FittingResults)
                .HasForeignKey(f => f.AccountId)
                .OnDelete(DeleteBehavior.Cascade); // Удаление связанных данных

            // Индекс на PromocodeId
            builder.HasIndex(f => f.AccountId)
                   .HasDatabaseName("IX_FittingResults_AccountId");

            // Связь с PromocodeEntity (внешний ключ)
            /*builder.HasOne(f => f.Promocode)
                .WithMany(p => p.FittingResults)
                .HasForeignKey(f => f.PromocodeId)
                .OnDelete(DeleteBehavior.Cascade); // Удаление связанных данных

            // Индекс на PromocodeId
            builder.HasIndex(f => f.PromocodeId)
                   .HasDatabaseName("IX_FittingResults_PromocodeId");*/
        }
    }
}
