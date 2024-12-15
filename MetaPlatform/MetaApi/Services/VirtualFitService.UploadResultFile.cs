using MetaApi.Models.VirtualFit;

namespace MetaApi.Services
{
    public partial class VirtualFitService
    {
        public async Task<string> UploadResultFileAsync(string imageUrl, string host)
        {
            // Сохранение файла на диск
            var uniqueFileName = await SaveFileAsync(imageUrl, FileType.Result);

            // Генерация публичной ссылки
            return GenerateFileUrl(uniqueFileName, FileType.Result, host);
        }

        private async Task<string> SaveFileAsync(string imageUrl, FileType fileType)
        {
            var uploadsPath = Path.Combine(_env.WebRootPath, fileType.GetFolderName());
            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
            }

            // Загрузка изображения по URL
            byte[] imageBytes;
            using (var httpClient = _httpClientFactory.CreateClient())
            {
                imageBytes = await httpClient.GetByteArrayAsync(imageUrl);
            }

            // Определение расширения файла из URL 
            // (берём часть пути из URL и извлекаем расширение)
            var uri = new Uri(imageUrl);
            var extension = Path.GetExtension(uri.AbsolutePath);
            if (string.IsNullOrWhiteSpace(extension))
            {
                // Если невозможно определить расширение, зададим по умолчанию .jpg
                extension = ".jpg";
            }

            string fileName = Guid.NewGuid().ToString();
            string uniqueFileName = fileName + extension;
            string filePath = Path.Combine(uploadsPath, uniqueFileName);

            // Сохраняем оригинальный файл
            await File.WriteAllBytesAsync(filePath, imageBytes);

            return uniqueFileName;
        }
    }
}
