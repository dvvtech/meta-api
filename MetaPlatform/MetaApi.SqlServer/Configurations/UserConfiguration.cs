using MetaApi.SqlServer.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace MetaApi.SqlServer.Configurations
{
    public sealed class UserConfiguration : IEntityTypeConfiguration<UserEntity>
    {
        public void Configure(EntityTypeBuilder<UserEntity> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(entity => entity.Id);

            builder.Property(user => user.UserName)
                   .HasMaxLength(512);

            builder.Property(user => user.JwtRefreshToken)
                   .HasMaxLength(128);

            builder.Property(user => user.AuthJson)
                   .HasMaxLength(1024);

            builder.Property(user => user.AuthType)
                   .IsRequired();

            builder.Property(user => user.Role)
                   .IsRequired();

            builder.Property(p => p.CreatedUtcDate)
                .IsRequired()
                .HasColumnType("datetime2");

            builder.Property(p => p.UpdateUtcDate)
                .IsRequired()
                .HasColumnType("datetime2");

            builder.HasIndex(user => user.JwtRefreshToken)
                   .IsUnique();
        }
    }
}
