using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing; // для методов обработки (Resize)
using SixLabors.ImageSharp.Formats.Jpeg;
using MetaApi.Consts; // для конкретного формата JPEG

namespace MetaApi.Services
{
    public static class ImageResizer
    {
        public static byte[] ResizeImage(IFormFile file, int targetWidth)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("Файл отсутствует или пуст.");
            }

            using var inputStream = file.OpenReadStream();
            using var image = Image.Load(inputStream);
            return ResizeImage(image, targetWidth);
        }

        public static byte[] ResizeImage(byte[] imageBytes, int targetWidth)
        {
            if (imageBytes == null || imageBytes.Length == 0)
            {
                throw new ArgumentException("Изображение отсутствует или пусто.");
            }

            using var inputStream = new MemoryStream(imageBytes);
            using var image = Image.Load(inputStream);
            return ResizeImage(image, targetWidth);
        }

        public static byte[] ResizeImage(Image image, int targetWidth)
        {
            try
            {                
                // Меняем размер изображения
                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Mode = ResizeMode.Max,
                    Size = new Size(targetWidth, 0) // Автоматически сохраняет пропорции
                }));

                // Сохраняем результат в JPEG
                using var outputStream = new MemoryStream();
                image.Save(outputStream, new JpegEncoder
                {
                    Quality = FittingConstants.QUALITY_JPEG
                });

                return outputStream.ToArray();
            }
            catch (UnknownImageFormatException)
            {
                throw new ArgumentException("Формат изображения не поддерживается.");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Произошла ошибка при обработке изображения.", ex);
            }
        }
    }
}
