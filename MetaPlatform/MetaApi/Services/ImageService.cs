using SixLabors.ImageSharp.Metadata.Profiles.Exif;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using Image = SixLabors.ImageSharp.Image;
using SixLabors.ImageSharp.PixelFormats;

namespace MetaApi.Services
{
    public class ImageService
    {
        public void PerformCropping(Image image, double targetAspectRatio)
        {
            int originalWidth = image.Width;
            int originalHeight = image.Height;

            int targetWidth = originalWidth;
            int targetHeight = (int)(originalWidth * targetAspectRatio);

            if (targetHeight > originalHeight)
            {
                targetHeight = originalHeight;
                targetWidth = (int)(originalHeight / targetAspectRatio);
            }

            int cropX = (originalWidth - targetWidth) / 2;
            int cropY = (originalHeight - targetHeight) / 2;

            image.Mutate(x => x.Crop(new Rectangle(cropX, cropY, targetWidth, targetHeight)));
        }

        /// <summary>
        /// Дорисовываем к фото полоски справа и слева 
        /// </summary>
        /// <param name="image"></param>
        /// <param name="targetAspectRatio"></param>
        /// <returns></returns>
        public Image PerformPadding(Image image, double targetAspectRatio)
        {
            int originalWidth = image.Width;
            int originalHeight = image.Height;

            // Вычисляем целевые размеры
            int targetWidth;
            int targetHeight;

            //фото горизонтальное
            if (originalHeight < originalWidth)
            {
                // Если текущее соотношение меньше целевого, увеличиваем ширину
                targetWidth = (int)(originalHeight * targetAspectRatio);
                targetHeight = originalHeight;
            }
            //фото вертикальное
            else
            {
                targetHeight = originalHeight;
                targetWidth = (int)(originalHeight / targetAspectRatio);
            }

            // Создаём новое изображение с нужными размерами и чёрным фоном
            var paddedImage = new Image<Rgba32>(targetWidth, targetHeight, Color.Black);

            // Рассчитываем координаты, чтобы центрировать оригинальное изображение
            int offsetX = (targetWidth - originalWidth) / 2;
            int offsetY = (targetHeight - originalHeight) / 2;

            // Вставляем оригинальное изображение в центр нового
            paddedImage.Mutate(ctx => ctx.DrawImage(image, new Point(offsetX, offsetY), 1.0f));

            return paddedImage;
        }

        public void FixOrientation(Image image)
        {
            if (image.Metadata?.ExifProfile == null ||
                !image.Metadata.ExifProfile.TryGetValue(ExifTag.Orientation, out var orientationValue))
            {
                return; // Нет EXIF ориентации
            }

            switch (orientationValue.Value)
            {
                case 3:
                    image.Mutate(x => x.Rotate(RotateMode.Rotate180));
                    break;
                case 6:
                    image.Mutate(x => x.Rotate(RotateMode.Rotate90));
                    break;
                case 8:
                    image.Mutate(x => x.Rotate(RotateMode.Rotate270));
                    break;
                default:
                    return; // Ориентация не требует изменений
            }

            image.Metadata.ExifProfile.RemoveValue(ExifTag.Orientation); // Удаляем EXIF Orientation            
        }

        public void ResizeIfNeeded(Image image, int maxWidth)
        {
            if (image.Width <= maxWidth)
                return;

            // Вычисляем пропорциональную высоту
            float ratio = (float)maxWidth / image.Width;
            int targetHeight = (int)(image.Height * ratio);

            image.Mutate(x => x.Resize(new ResizeOptions
            {
                Mode = ResizeMode.Max,
                Size = new Size(maxWidth, targetHeight)
            }));
        }
    }
}
