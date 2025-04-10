using MetaApi.SqlServer.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace MetaApi.SqlServer.Configurations
{
    public sealed class UserTryOnLimitConfiguration : IEntityTypeConfiguration<UserTryOnLimitEntity>
    {
        public void Configure(EntityTypeBuilder<UserTryOnLimitEntity> builder)
        {
            builder.ToTable("UserTryOnLimits"); // Название таблицы в БД

            builder.HasKey(x => x.Id); // Первичный ключ

            // Внешний ключ на AccountEntity (связь один-к-одному или один-ко-многим)
            builder.HasOne(x => x.Account)
                   .WithMany() // Если у AccountEntity нет навигационного свойства к лимитам
                   .HasForeignKey(x => x.AccountId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Cascade); // Удаление лимита при удалении аккаунта

            // Настройка полей
            builder.Property(x => x.MaxAttempts)
                   .IsRequired()
                   .HasDefaultValue(3); // Дефолтное значение (можно менять)

            builder.Property(x => x.AttemptsUsed)
                   .IsRequired()
                   .HasDefaultValue(0);

            builder.Property(x => x.LastResetTime)
                   .IsRequired()
                   .HasColumnType("datetime2");

            // Хранение TimeSpan в БД (в типе time или bigint для миллисекунд)
            builder.Property(x => x.ResetPeriod)
                   .IsRequired()
                   .HasConversion(
                       v => v.Ticks,           // В БД храним как bigint (ticks)
                       v => TimeSpan.FromTicks(v) // При чтении преобразуем обратно
                   );

            // Уникальный индекс на AccountId (чтобы у пользователя был только один лимит)
            builder.HasIndex(x => x.AccountId)
                   .IsUnique();
        }
    }
}
