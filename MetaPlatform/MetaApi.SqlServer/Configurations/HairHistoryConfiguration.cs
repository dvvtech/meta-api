using MetaApi.SqlServer.Entities.VirtualHair;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MetaApi.SqlServer.Configurations
{
    public class HairHistoryConfiguration : IEntityTypeConfiguration<HairHistoryEntity>
    {
        public void Configure(EntityTypeBuilder<HairHistoryEntity> builder)
        {
            // Таблица
            builder.ToTable("HairHistory");

            // Первичный ключ
            builder.HasKey(f => f.Id);

            // Поля
            builder.Property(f => f.HairImgUrl)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(f => f.FaceImgUrl)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(f => f.ResultImgUrl)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(f => f.CreatedUtcDate)
                .IsRequired()
                .HasColumnType("datetime2");

            // Связь с AccountEntity (внешний ключ)
            builder.HasOne(f => f.Account)
                .WithMany(p => p.HairHistory)
                .HasForeignKey(f => f.AccountId)
                .OnDelete(DeleteBehavior.Cascade); // Удаление связанных данных            
        }
    }
}
