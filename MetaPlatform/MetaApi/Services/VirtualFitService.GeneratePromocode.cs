using MetaApi.Models.VirtualFit;
using MetaApi.SqlServer.Entities.VirtualFit;

namespace MetaApi.Services
{
    public partial class VirtualFitService
    {
        public async Task<string> GeneratePromocode(GeneratePromocodeRequest request)
        {
            // Генерация уникального промокода
            var promocode = GenerateUniquePromocode();

            // Создание сущности промокода
            var promocodeEntity = new PromocodeEntity
            {
                Promocode = promocode,
                Name = request.Name,
                UsageLimit = request.UsageLimit,
                RemainingUsage = request.UsageLimit,
                CreatedUtcDate = DateTime.UtcNow,
                UpdateUtcDate = DateTime.UtcNow
            };

            // Сохранение в базе данных
            _metaDbContext.Promocode.Add(promocodeEntity);
            await _metaDbContext.SaveChangesAsync();

            return promocode;
        }

        private string GenerateUniquePromocode()
        {
            string promocode;
            var random = new Random();

            do
            {
                // Генерация промокода из 6 символов (буквы и цифры)
                promocode = new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", 6)
                    .Select(s => s[random.Next(s.Length)]).ToArray());
            }
            while (_metaDbContext.Promocode.Any(p => p.Promocode == promocode)); // Проверка уникальности

            return promocode;
        }
    }
}
