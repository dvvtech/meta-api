using MetaApi.Models.VirtualFit;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using Image = SixLabors.ImageSharp.Image;
using SixLabors.ImageSharp.PixelFormats;
using MetaApi.Consts;
using SixLabors.ImageSharp.Formats.Jpeg;

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
            file = ResizeImageAndCorrectOrientation(file);

            // Расчёт CRC для загружаемого файла
            string fileCrc = await CalculateCrcAsync(file);

            // Проверка существования файла
            if (fileType == FileType.Upload && _fileCrcService.FileCrcDictionary.TryGetValue(fileCrc, out var existingFileName))
            {
                _logger.LogInformation($"{existingFileName} get from cache");
                return GenerateFileUrl(existingFileName, fileType, host);
            }

            // Сохранение файла на диск c окончанием _v
            var uniqueFileName = await SaveFileAsync(file, fileType);
            
            //сохраняем уменьшенную копию
            await ResizeAndSaveFile(file, fileType, uniqueFileName);

            await PaddingAndSave(file, fileType, uniqueFileName);

            // Добавление CRC в словарь
            _fileCrcService.AddFileCrc(fileCrc, uniqueFileName);   
            
            // Генерация публичной ссылки
            return GenerateFileUrl(uniqueFileName, Path.GetExtension(file.FileName), fileType, host);            
        }

        private async Task PaddingAndSave(IFormFile file, FileType fileType, string fileName)
        {
            Image image = null;
            try
            {
                using var inputStream = file.OpenReadStream();

                // Определяем формат изображения
                IImageFormat format = Image.DetectFormat(inputStream);
                if (format == null)
                    return; // Неподдерживаемый формат изображения

                inputStream.Position = 0; // Сброс позиции потока
                image = Image.Load(inputStream);
                //todo для горизонтальных фото нижняя строка не валидна
                //вычисляем отношение высоты к ширине
                float imageRatio = (float)image.Height / image.Width;
                if (imageRatio < 1.22 || imageRatio > 1.4)
                {
                    //info: обычно для айфоновских фото сюда уже не заходим
                    using (Image newImage = PerformPadding(image, FittingConstants.ASPECT_RATIO))
                    {                        
                        // Определяем формат на основе расширения или указываем его явно
                        var jpegEncoder = new JpegEncoder
                        {
                            Quality = 85 // Настройка качества изображения (можно изменить)
                        };

                        string uploadsPath = Path.Combine(_env.WebRootPath, fileType.GetFolderName());                        
                        string newFileName = $"{fileName}_r{Path.GetExtension(file.FileName)}";
                        string newfilePath = Path.Combine(uploadsPath, newFileName);

                        // Сохраняем изображение в файл
                        newImage.Save(newfilePath, jpegEncoder);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            finally 
            {
                image?.Dispose();
            }
        }

        private async Task<string> CalculateCrcAsync(IFormFile file)
        {
            using var stream = file.OpenReadStream();
            using var crc32 = new Crc32();            
            var hash = await crc32.ComputeHashAsync(stream);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }

        private async Task ResizeAndSaveFile(IFormFile file, FileType fileType, string fileName)
        {            
            //Сохраняем уменьшенную копию для раздела история
            byte[] resizedBytes = ImageResizer.ResizeImage(file, FittingConstants.THUMBNAIL_WIDTH);

            var uploadsPath = Path.Combine(_env.WebRootPath, fileType.GetFolderName());
            string newFileName = $"{fileName}_t{Path.GetExtension(file.FileName)}";
            string newfilePath = Path.Combine(uploadsPath, newFileName);

            await File.WriteAllBytesAsync(newfilePath, resizedBytes);
        }

        private async Task<string> SaveFileAsync(IFormFile file, FileType fileType)
        {
            var uploadsPath = Path.Combine(_env.WebRootPath, fileType.GetFolderName());

            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
            }

            string fileName = Guid.NewGuid().ToString();
            string uniqueFileName = $"{fileName}_v{Path.GetExtension(file.FileName)}";
            string filePath = Path.Combine(uploadsPath, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }            

            return fileName;
        }

        /// <summary>
        /// Ресазим изображение до ширины 768 и исправление ориентации изображения на основе EXIF-метаданных.
        /// </summary>
        /// <param name="file">Входное изображение в формате IFormFile.</param>
        /// <returns>Исправленное изображение в виде IFormFile или исходный файл, если EXIF Orientation отсутствует.</returns>
        private IFormFile ResizeImageAndCorrectOrientation(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Файл не должен быть пустым", nameof(file));

            Image image = null;
            try
            {
                using var inputStream = file.OpenReadStream();

                // Определяем формат изображения
                IImageFormat format = Image.DetectFormat(inputStream);
                if (format == null)
                    return file; // Неподдерживаемый формат изображения

                inputStream.Position = 0; // Сброс позиции потока
                image = Image.Load(inputStream);                
                
                //если фото неайфоновское
                if (image.Metadata?.ExifProfile == null ||
                    !image.Metadata.ExifProfile.TryGetValue(ExifTag.Orientation, out var orientationValue))
                {                    
                    ResizeIfNeeded(image, 768);
                }
                else
                {                    
                    ResizeIfNeeded(image, 1024);
                }

                // Затем фиксируем ориентацию над уже уменьшенным изображением
                FixOrientation(image);

                return SaveImageToIFormFile(image, format, file);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка при обработке изображения: {ex.Message}");
                return file;
            }
            finally 
            {
                image?.Dispose();                
            }
        }

        /// <summary>
        /// Дорисовываем к фото полоски справа и слева 
        /// </summary>
        /// <param name="image"></param>
        /// <param name="targetAspectRatio"></param>
        /// <returns></returns>
        private Image PerformPadding(Image image, double targetAspectRatio)
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

        private void PerformCropping(Image image, double targetAspectRatio)
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

        private void FixOrientation(Image image)
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

        private void ResizeIfNeeded(Image image, int maxWidth)
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

        private IFormFile SaveImageToIFormFile(Image image, IImageFormat format, IFormFile originalFile)
        {
            var outputStream = new MemoryStream();
            image.Save(outputStream, format);
            outputStream.Position = 0;

            return new FormFile(outputStream, 0, outputStream.Length, originalFile.Name, originalFile.FileName)
            {
                Headers = originalFile.Headers,
                ContentType = originalFile.ContentType
            };
        }

        

        private string GenerateFileUrl(string fileName, string extension, FileType fileType, string host)
        {
            //todo проверить что это https Request.Scheme
            return $"https://{host}/{fileType.GetFolderName()}/{fileName}_v{extension}";
        }

        private string GenerateFileUrl(string fileName, FileType fileType, string host)
        {
            //todo проверить что это https Request.Scheme
            return $"https://{host}/{fileType.GetFolderName()}/{fileName}";
        }
    }
}
