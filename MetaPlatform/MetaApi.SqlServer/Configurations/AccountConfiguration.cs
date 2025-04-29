using MetaApi.SqlServer.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace MetaApi.SqlServer.Configurations
{
    public sealed class AccountConfiguration : IEntityTypeConfiguration<AccountEntity>
    {
        public void Configure(EntityTypeBuilder<AccountEntity> builder)
        {
            builder.ToTable("Accounts");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.UserName)
                   .HasMaxLength(512);

            builder.Property(x => x.ExternalId)
                   .HasMaxLength(256);                                      

            builder.Property(x => x.JwtRefreshToken)
                   .HasMaxLength(128);

            builder.Property(x => x.AuthType)
                   .IsRequired();

            builder.Property(x => x.Role)
                   .IsRequired();

            builder.Property(x => x.CreatedUtcDate)
                .IsRequired()
                .HasColumnType("datetime2");

            builder.Property(x => x.UpdateUtcDate)
                .IsRequired()
                .HasColumnType("datetime2");

            // Связь с FittingResultEntity (коллекция)
            builder.HasMany(p => p.FittingResults)
                .WithOne(f => f.Account)
                .HasForeignKey(f => f.AccountId)
                //.IsRequired()
                .OnDelete(DeleteBehavior.Cascade); // Удаление связанных данных


            builder.HasIndex(x => x.JwtRefreshToken)
                   .IsUnique();

            builder.HasIndex(x => x.ExternalId)
                   .IsUnique();
        }
    }
}
