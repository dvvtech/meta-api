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
            file = FixImageOrientationAndSize(file);            
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
        private IFormFile FixImageOrientationAndSize(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Файл не должен быть пустым", nameof(file));

            try
            {
                // Открываем поток для чтения файла
                using var inputStream = file.OpenReadStream();

                // Определяем формат изображения
                IImageFormat format = Image.DetectFormat(inputStream);
                if (format == null)
                    return file; // Неподдерживаемый формат изображения

                inputStream.Position = 0; // Сбрасываем позицию потока
                using var image = Image.Load(inputStream);

                // Проверяем наличие и значение EXIF ориентации
                if (image.Metadata?.ExifProfile == null ||
                    !image.Metadata.ExifProfile.TryGetValue(ExifTag.Orientation, out var orientationValue))
                {
                    return ResizeIfNeed(file, image, format);
                }

                ushort orientation = orientationValue.Value;
                bool orientationFixed = false;

                const int maxWidth = 1024;
                // Вычисляем пропорциональную высоту
                float ratio = (float)maxWidth / (float)image.Width;
                int targetHeight = (int)(image.Height * ratio);

                // Применяем поворот изображения в зависимости от ориентации
                switch (orientation)
                {
                    case 3:
                        {
                            if (image.Width > maxWidth)
                            {                                
                                // Меняем размер с помощью ImageSharp
                                image.Mutate(x => x.Resize(new ResizeOptions
                                {
                                    Mode = ResizeMode.Max,
                                    Size = new Size(maxWidth, targetHeight)
                                }));
                            }

                            image.Mutate(x => x.Rotate(RotateMode.Rotate180));
                            orientationFixed = true;
                            break;
                        }
                    case 6:
                        {
                            if (image.Width > maxWidth)
                            {
                                // Меняем размер с помощью ImageSharp
                                image.Mutate(x => x.Resize(new ResizeOptions
                                {
                                    Mode = ResizeMode.Max,
                                    Size = new Size(maxWidth, targetHeight)
                                }));
                            }

                            image.Mutate(x => x.Rotate(RotateMode.Rotate90));
                            orientationFixed = true;
                            break;
                        }
                    case 8:
                        {
                            if (image.Width > maxWidth)
                            {
                                // Меняем размер с помощью ImageSharp
                                image.Mutate(x => x.Resize(new ResizeOptions
                                {
                                    Mode = ResizeMode.Max,
                                    Size = new Size(maxWidth, targetHeight)
                                }));
                            }

                            image.Mutate(x => x.Rotate(RotateMode.Rotate270));
                            orientationFixed = true;
                            break;
                        }
                }

                if (!orientationFixed)
                {
                    return ResizeIfNeed(file, image, format); // Если изменений нет, возвращаем исходный файл
                }

                // Удаляем EXIF Orientation
                image.Metadata.ExifProfile.RemoveValue(ExifTag.Orientation);

                // Сохраняем исправленное изображение в MemoryStream
                var outputStream = new MemoryStream();
                image.Save(outputStream, format); // Используем исходный формат
                outputStream.Position = 0;

                // Создаём новый IFormFile из MemoryStream
                var fixedFile = new FormFile(outputStream, 0, outputStream.Length, file.Name, file.FileName)
                {
                    Headers = file.Headers,
                    ContentType = file.ContentType
                };

                return fixedFile;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка при обработке изображения: {ex.Message}");
                //Console.WriteLine($"Ошибка при обработке изображения: {ex.Message}");
                return file; // Возвращаем оригинальный файл в случае ошибки
            }
        }

        private IFormFile ResizeIfNeed(IFormFile file, Image image, IImageFormat format)
        {
            const int maxWidth = 1024;
            if (image.Width > maxWidth)
            {
                // Вычисляем пропорциональную высоту
                float ratio = (float)maxWidth / (float)image.Width;
                int targetHeight = (int)(image.Height * ratio);

                // Меняем размер с помощью ImageSharp
                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Mode = ResizeMode.Max,
                    Size = new Size(maxWidth, targetHeight)
                }));
                // Сохраняем исправленное изображение в MemoryStream
                var outputStream1 = new MemoryStream();
                image.Save(outputStream1, format); // Используем исходный формат
                outputStream1.Position = 0;

                // Создаём новый IFormFile из MemoryStream
                var fixedFile1 = new FormFile(outputStream1, 0, outputStream1.Length, file.Name, file.FileName)
                {
                    Headers = file.Headers,
                    ContentType = file.ContentType
                };

                return fixedFile1;
            }
            else
            {
                return file; // Ориентация отсутствует, возвращаем исходный файл
            }
        }


        private string GenerateFileUrl(string fileName, FileType fileType, string host)
        {
            //todo проверить что это https Request.Scheme
            return $"https://{host}/{fileType.GetFolderName()}/{fileName}";
        }
    }
}
