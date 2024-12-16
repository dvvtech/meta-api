using MetaApi.Models.VirtualFit;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace MetaApi.Services
{
    public partial class VirtualFitService
    {
        public int GetCount()
        { 
            return _fileCrcService.FileCrcDictionary.Count;
        }

        /// <summary>
        /// Возвращает ссылку на файл
        /// </summary>
        /// <param name="file"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<string> UploadFileAsync(IFormFile file, FileType fileType, string host)
        {
            file = FixImageOrientation(file);

            // Расчёт CRC для загружаемого файла
            string fileCrc = await CalculateCrcAsync(file);
            
            // Проверка существования файла
            if (fileType == FileType.Upload && _fileCrcService.FileCrcDictionary.TryGetValue(fileCrc, out var existingFileName))
            {
                _logger.LogInformation($"{existingFileName} get from cache");
                return GenerateFileUrl(existingFileName, fileType, host);                
            }
            
            // Сохранение файла на диск
            var uniqueFileName = await SaveFileAsync(file, fileType);
            
            // Добавление CRC в словарь
            _fileCrcService.AddFileCrc(fileCrc, uniqueFileName);

            // Генерация публичной ссылки
            return GenerateFileUrl(uniqueFileName, fileType, host);            
        }

        private async Task<string> CalculateCrcAsync(IFormFile file)
        {
            using var stream = file.OpenReadStream();
            using var crc32 = new Crc32();            
            var hash = await crc32.ComputeHashAsync(stream);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }

        private async Task<string> SaveFileAsync(IFormFile file, FileType fileType)
        {            
            var uploadsPath = Path.Combine(_env.WebRootPath, fileType.GetFolderName());            
            if (!Directory.Exists(uploadsPath))
            {                
                Directory.CreateDirectory(uploadsPath);                
            }

            string fileName = Guid.NewGuid().ToString();
            string uniqueFileName = fileName + Path.GetExtension(file.FileName);
            string filePath = Path.Combine(uploadsPath, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            if (fileType != FileType.Upload)
            {                
                byte[] resizedBytes = ImageResizer.ResizeImage(file, 135);
                string newFileName = $"{fileName}_t{Path.GetExtension(file.FileName)}";
                string newfilePath = Path.Combine(uploadsPath, newFileName);
                await File.WriteAllBytesAsync(newfilePath, resizedBytes);                
            }

            return uniqueFileName;
        }

        /// <summary>
        /// Исправляет ориентацию изображения на основе EXIF-метаданных.
        /// </summary>
        /// <param name="file">Входное изображение в формате IFormFile.</param>
        /// <returns>Исправленное изображение в виде IFormFile или исходный файл, если EXIF Orientation отсутствует.</returns>
        private IFormFile FixImageOrientation(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Файл не должен быть пустым", nameof(file));

            using var inputStream = file.OpenReadStream();

            // Определяем формат изображения
            IImageFormat format = Image.DetectFormat(inputStream);
            inputStream.Position = 0; // Сбрасываем позицию потока для повторного чтения

            // Загружаем изображение
            using var image = Image.Load(inputStream);

            // Попытка получить значение ориентации из EXIF
            if (image.Metadata.ExifProfile == null ||
                !image.Metadata.ExifProfile.TryGetValue(ExifTag.Orientation, out var orientationValue))
            {
                // Если тега Orientation нет, возвращаем исходный файл
                return file;
            }

            ushort orientation = orientationValue.Value;
            bool orientationFixed = false;

            // Применяем поворот на основе значения ориентации
            switch (orientation)
            {
                case 3: // Поворот на 180 градусов
                    image.Mutate(x => x.Rotate(RotateMode.Rotate180));
                    orientationFixed = true;
                    break;
                case 6: // Поворот на 90 градусов по часовой стрелке
                    image.Mutate(x => x.Rotate(RotateMode.Rotate90));
                    orientationFixed = true;
                    break;
                case 8: // Поворот на 270 градусов против часовой стрелки
                    image.Mutate(x => x.Rotate(RotateMode.Rotate270));
                    orientationFixed = true;
                    break;
            }

            if (!orientationFixed)
                return file;

            // Удаляем EXIF Orientation, чтобы предотвратить повторное применение
            image.Metadata.ExifProfile.RemoveValue(ExifTag.Orientation);

            // Сохраняем исправленное изображение в MemoryStream
            var outputStream = new MemoryStream();
            image.Save(outputStream, format); // Сохраняем в исходном формате
            outputStream.Position = 0; // Сброс позиции потока для чтения

            // Создаём новый IFormFile из исправленного изображения
            var fixedFile = new FormFile(outputStream, 0, outputStream.Length, file.Name, file.FileName)
            {
                Headers = file.Headers,
                ContentType = file.ContentType
            };

            return fixedFile;
        }

        private string GenerateFileUrl(string fileName, FileType fileType, string host)
        {
            //todo проверить что это https Request.Scheme
            return $"https://{host}/{fileType.GetFolderName()}/{fileName}";
        }
    }
}
