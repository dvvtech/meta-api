using MetaApi.SqlServer.Entities.VirtualFit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MetaApi.SqlServer.Configurations
{
    public class PromocodeConfiguration : IEntityTypeConfiguration<PromocodeEntity>
    {
        public void Configure(EntityTypeBuilder<PromocodeEntity> builder)
        {
            // Таблица
            builder.ToTable("Promocodes");

            // Первичный ключ
            builder.HasKey(p => p.Id);

            // Поля
            builder.Property(p => p.Promocode)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.Name)
                .HasMaxLength(200); // Необязательное поле

            builder.Property(p => p.UsageLimit)
                .IsRequired();

            builder.Property(p => p.RemainingUsage)
                .IsRequired();

            builder.Property(p => p.CreatedUtcDate)
                .IsRequired()
                .HasColumnType("datetime2");

            builder.Property(p => p.UpdateUtcDate)
                .IsRequired()
                .HasColumnType("datetime2");

            // Связь с FittingResultEntity (коллекция)
            /*builder.HasMany(p => p.FittingResults)
                .WithOne(f => f.Promocode)
                .HasForeignKey(f => f.PromocodeId)
                .OnDelete(DeleteBehavior.Cascade); // Удаление связанных данных*/

            // Индекс на поле Promocode
            builder.HasIndex(p => p.Promocode)
                   .IsUnique() // Индекс уникальный
                   .HasDatabaseName("IX_Promocodes_Promocode"); // Опциональное имя индекса
        }
    }
}
