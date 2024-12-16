using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;

namespace MetaApi.Services
{
    public partial class VirtualFitService
    {
        /// <summary>
        /// Исправляет ориентацию изображения на основе EXIF-метаданных.
        /// </summary>
        /// <param name="file">Входное изображение в формате IFormFile.</param>
        /// <returns>Исправленное изображение в виде IFormFile или исходный файл, если EXIF Orientation отсутствует.</returns>
        public static IFormFile FixImageOrientation(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Файл не должен быть пустым", nameof(file));

            using var inputStream = file.OpenReadStream();

            // Загружаем изображение из входного потока
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

            // Если ориентация не была исправлена, вернуть исходный файл
            if (!orientationFixed)
                return file;

            // Удаляем EXIF Orientation, чтобы предотвратить повторное применение
            image.Metadata.ExifProfile.RemoveValue(ExifTag.Orientation);

            // Сохраняем исправленное изображение в MemoryStream
            var outputStream = new MemoryStream();
            image.SaveAsJpeg(outputStream); // Сохраняем в формате JPEG
            outputStream.Position = 0; // Сброс позиции потока для чтения

            // Создаём новый IFormFile из исправленного изображения
            var fixedFile = new FormFile(outputStream, 0, outputStream.Length, file.Name, file.FileName)
            {
                Headers = file.Headers,
                ContentType = "image/jpeg"
            };

            return fixedFile;
        }
    }
}
