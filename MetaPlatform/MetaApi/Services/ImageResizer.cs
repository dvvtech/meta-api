using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing; // для методов обработки (Resize)
using SixLabors.ImageSharp.Formats.Jpeg; // для конкретного формата JPEG

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
            return ResizeImageFromStream(inputStream, targetWidth);
        }

        public static byte[] ResizeImage(byte[] imageBytes, int targetWidth)
        {
            if (imageBytes == null || imageBytes.Length == 0)
            {
                throw new ArgumentException("Изображение отсутствует или пусто.");
            }

            using var inputStream = new MemoryStream(imageBytes);
            return ResizeImageFromStream(inputStream, targetWidth);
        }

        private static byte[] ResizeImageFromStream(Stream inputStream, int targetWidth)
        {
            using var image = Image.Load(inputStream);

            // Вычисляем пропорциональные размеры
            int targetHeight = CalculateProportionalHeight(image.Width, image.Height, targetWidth);

            // Меняем размер изображения
            image.Mutate(x => x.Resize(new ResizeOptions
            {
                Mode = ResizeMode.Max,
                Size = new Size(targetWidth, targetHeight)
            }));

            // Сохраняем результат в JPEG
            using var outputStream = new MemoryStream();
            image.Save(outputStream, JpegFormat.Instance);

            return outputStream.ToArray();
        }

        private static int CalculateProportionalHeight(int originalWidth, int originalHeight, int targetWidth)
        {
            float ratio = (float)targetWidth / originalWidth;
            return (int)(originalHeight * ratio);
        }
    }
}
