using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing; // для методов обработки (Resize)
using SixLabors.ImageSharp.Formats;   // для определений форматов
using SixLabors.ImageSharp.Formats.Jpeg; // для конкретного формата JPEG

namespace MetaApi.Services
{
    public static class ImageResizer
    {
        public static byte[] ResizeImage(IFormFile file, int targetWidth = 200)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("Файл отсутствует или пуст.");
            }

            // Открываем поток на чтение из загруженного файла
            using var inputStream = file.OpenReadStream();

            // Загружаем изображение с определением формата
            
            using var image = Image.Load(inputStream);

            // Исходные размеры
            int originalWidth = image.Width;
            int originalHeight = image.Height;

            // Вычисляем пропорциональную высоту
            float ratio = (float)targetWidth / (float)originalWidth;
            int targetHeight = (int)(originalHeight * ratio);

            // Меняем размер с помощью ImageSharp
            image.Mutate(x => x.Resize(new ResizeOptions
            {
                Mode = ResizeMode.Max,
                Size = new Size(targetWidth, targetHeight)
            }));

            // Сохраняем результат в массив байт (например, в JPEG)
            using var outputStream = new MemoryStream();
            // Если хочется зафиксировать конкретный формат - например JPEG
            image.Save(outputStream, JpegFormat.Instance);

            return outputStream.ToArray();
        }
    }
}
