using MetaApi.SqlServer.Entities.VirtualFit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaApi.SqlServer.Configurations
{
    internal class FittingResultConfiguration : IEntityTypeConfiguration<FittingResultEntity>
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
            builder.HasOne(f => f.Promocode)
                .WithMany(p => p.FittingResults)
                .HasForeignKey(f => f.PromocodeId)
                .OnDelete(DeleteBehavior.Cascade); // Удаление связанных данных
        }
    }
}
