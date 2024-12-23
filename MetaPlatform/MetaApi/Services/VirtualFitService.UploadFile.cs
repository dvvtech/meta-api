using MetaApi.Models.VirtualFit;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using Image = SixLabors.ImageSharp.Image;

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
            file = CorrectOrientationAndResizeImage(file);            
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
        private IFormFile CorrectOrientationAndResizeImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Файл не должен быть пустым", nameof(file));

            try
            {
                using var inputStream = file.OpenReadStream();

                // Определяем формат изображения
                IImageFormat format = Image.DetectFormat(inputStream);
                if (format == null)
                    return file; // Неподдерживаемый формат изображения

                inputStream.Position = 0; // Сброс позиции потока
                using var image = Image.Load(inputStream);

                bool isChangeImage = false;
                //если фото неайфоновское
                if (image.Metadata?.ExifProfile == null ||
                    !image.Metadata.ExifProfile.TryGetValue(ExifTag.Orientation, out var orientationValue))
                {
                    // Сначала меняем размер изображения
                    isChangeImage = ResizeIfNeeded(image, 768);
                }
                else
                {
                    // Сначала меняем размер изображения
                    isChangeImage = ResizeIfNeeded(image, 1024);
                }

                // Затем фиксируем ориентацию над уже уменьшенным изображением
                bool orientationFixed = FixOrientation(image);

                float imageRatio = (float)image.Height / image.Width;
                if (imageRatio < 1.2 || imageRatio > 1.4)
                {
                    //info: обычно для айфоновских фото сюда уже не заходим
                    PerformCropping(image, 1.3);
                    isChangeImage = true;
                }

                if (!isChangeImage && !orientationFixed)
                    return file; // Если ничего не изменилось, возвращаем исходный файл

                return SaveImageToIFormFile(image, format, file);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка при обработке изображения: {ex.Message}");
                return file;
            }
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

        private bool FixOrientation(Image image)
        {
            if (image.Metadata?.ExifProfile == null ||
                !image.Metadata.ExifProfile.TryGetValue(ExifTag.Orientation, out var orientationValue))
            {
                return false; // Нет EXIF ориентации
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
                    return false; // Ориентация не требует изменений
            }

            image.Metadata.ExifProfile.RemoveValue(ExifTag.Orientation); // Удаляем EXIF Orientation
            return true;
        }

        private bool ResizeIfNeeded(Image image, int maxWidth)
        {
            //const int MaxWidth = 1024;

            if (image.Width <= maxWidth)
                return false;

            // Вычисляем пропорциональную высоту
            float ratio = (float)maxWidth / image.Width;
            int targetHeight = (int)(image.Height * ratio);

            image.Mutate(x => x.Resize(new ResizeOptions
            {
                Mode = ResizeMode.Max,
                Size = new Size(maxWidth, targetHeight)
            }));
            return true;
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


        private string GenerateFileUrl(string fileName, FileType fileType, string host)
        {
            //todo проверить что это https Request.Scheme
            return $"https://{host}/{fileType.GetFolderName()}/{fileName}";
        }
    }
}
