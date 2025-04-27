using MetaApi.Models.VirtualFit;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;
using SixLabors.ImageSharp;
using Image = SixLabors.ImageSharp.Image;
using MetaApi.Consts;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace MetaApi.Services
{
    public partial class FileService
    {
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
            if (fileType == FileType.Upload && _crcFileProvider.FileCrcDictionary.TryGetValue(fileCrc, out var existingFileName))
            {
                _logger.LogInformation($"{existingFileName} get from cache");
                return GenerateFileUrl(existingFileName, fileType, host);
            }

            string uniqueFileName = Guid.NewGuid().ToString();
            var uploadsPath = Path.Combine(_webRootPath, fileType.GetFolderName());

            string padingFileName = PaddingAndSave(file, uploadsPath, uniqueFileName, out float imageRatio);

            string lastFileName = await SaveFileAsync(file, uploadsPath, uniqueFileName, imageRatio);
            //lastFileName = (string.IsNullOrEmpty(padingFileName)) ? lastFileName : padingFileName;

            await ResizeAndSaveFile(file, uploadsPath, uniqueFileName, imageRatio);

            // Добавление CRC в словарь
            _crcFileProvider.AddFileCrc(fileCrc, lastFileName);

            // Генерация публичной ссылки
            return GenerateFileUrl(lastFileName, fileType, host);
        }

        private string PaddingAndSave(IFormFile file, string uploadsPath, string fileName, out float imageRatio)
        {
            Image image = null;
            try
            {
                using var inputStream = file.OpenReadStream();

                // Определяем формат изображения
                IImageFormat format = Image.DetectFormat(inputStream);
                if (format == null)
                {
                    imageRatio = 0.0f;
                    return string.Empty; // Неподдерживаемый формат изображения
                }

                inputStream.Position = 0; // Сброс позиции потока
                image = Image.Load(inputStream);
                //todo для горизонтальных фото нижняя строка не валидна
                //вычисляем отношение высоты к ширине
                imageRatio = (float)image.Height / image.Width;
                imageRatio = (float)Math.Round(imageRatio, 3);
                if (imageRatio < 1.25 || imageRatio > 1.38)
                {
                    //info: обычно для айфоновских фото сюда уже не заходим
                    using (Image newImage = _imageService.PerformPadding(image, FittingConstants.ASPECT_RATIO))
                    {
                        // Определяем формат на основе расширения или указываем его явно
                        var jpegEncoder = new JpegEncoder
                        {
                            Quality = FittingConstants.QUALITY_JPEG
                        };

                        string newFileName = $"{fileName}_{imageRatio}_p{Path.GetExtension(file.FileName)}";
                        string newfilePath = Path.Combine(uploadsPath, newFileName);

                        // Сохраняем изображение в файл
                        newImage.Save(newfilePath, jpegEncoder);

                        return newFileName;
                        //newFileName = $"{fileName}_res{Path.GetExtension(file.FileName)}";
                        //newfilePath = Path.Combine(uploadsPath, newFileName);
                        //PerformCropping(newImage, imageRatio);
                        //newImage.Save(newfilePath, jpegEncoder);
                    }
                }
                else
                {
                    imageRatio = 0;
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                image?.Dispose();
            }

            imageRatio = 0.0f;
            return string.Empty;
        }

        private async Task<string> CalculateCrcAsync(IFormFile file)
        {
            using var stream = file.OpenReadStream();
            using var crc32 = new Crc32();
            var hash = await crc32.ComputeHashAsync(stream);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }

        /// <summary>
        /// Сохраняем уменьшенную копию для раздела история
        /// </summary>
        /// <param name="file"></param>
        /// <param name="uploadsPath"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private async Task ResizeAndSaveFile(IFormFile file, string uploadsPath, string fileName, float imageRatio)
        {
            byte[] resizedBytes = ImageResizer.ResizeImage(file, FittingConstants.THUMBNAIL_WIDTH);

            string newFileName;
            if (imageRatio > 0.1)
            {
                newFileName = $"{fileName}_{imageRatio}_t{Path.GetExtension(file.FileName)}";
            }
            else
            {
                newFileName = $"{fileName}_t{Path.GetExtension(file.FileName)}";
            }
            string newfilePath = Path.Combine(uploadsPath, newFileName);

            await File.WriteAllBytesAsync(newfilePath, resizedBytes);
        }

        /// <summary>
        /// Сохранение файла на диск c окончанием _v
        /// </summary>
        /// <param name="file"></param>
        /// <param name="uploadsPath"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private async Task<string> SaveFileAsync(IFormFile file, string uploadsPath, string fileName, float imageRatio)
        {
            string uniqueFileName;
            if (imageRatio > 0.1)
            {
                uniqueFileName = $"{fileName}_{imageRatio}_v{Path.GetExtension(file.FileName)}";
            }
            else
            {
                uniqueFileName = $"{fileName}_v{Path.GetExtension(file.FileName)}";
            }
            string filePath = Path.Combine(uploadsPath, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return uniqueFileName;
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
                    _imageService.ResizeIfNeeded(image, 768);
                }
                else
                {
                    _imageService.ResizeIfNeeded(image, 1024);
                }

                // Затем фиксируем ориентацию над уже уменьшенным изображением
                _imageService.FixOrientation(image);

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

        public async Task<string> UploadResultFileAsync(string imageUrl, string host, string humanImg)
        {
            byte[] imageBytes = await GetImageResult(imageUrl);

            var image = Image.Load(imageBytes);

            double? imageRatio = GetImageRatio(humanImg);
            if (imageRatio != null)
            {
                _imageService.PerformCropping(image, imageRatio.Value);
            }

            // Сохранение файла на диск
            var uniqueFileName = await SaveFileAsync(imageUrl, image, FileType.Result);

            // Генерация публичной ссылки
            return GenerateFileUrl(uniqueFileName, FileType.Result, host);
        }

        private async Task<string> SaveFileAsync(string imageUrl, Image image, FileType fileType)
        {
            var uploadsPath = Path.Combine(_webRootPath, fileType.GetFolderName());
            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
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
            string uniqueFileName = $"{fileName}_v{extension}";
            string filePath = Path.Combine(uploadsPath, uniqueFileName);

            // Сохраняем оригинальный файл            
            image.Save(filePath, new JpegEncoder
            {
                Quality = FittingConstants.QUALITY_JPEG
            });

            //Сохраняем уменьшенную копию для раздела история            
            byte[] resizedBytes = ImageResizer.ResizeImage(image, FittingConstants.THUMBNAIL_WIDTH);
            string newFileName = $"{fileName}_t{extension}";
            string newfilePath = Path.Combine(uploadsPath, newFileName);
            await File.WriteAllBytesAsync(newfilePath, resizedBytes);

            return uniqueFileName;
        }

        private double? GetImageRatio(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return null;

            var parts = fileName.Split('_');
            if (parts.Length <= 1) return null;

            return double.TryParse(parts[1], out var ratio) ? ratio : null;
        }

        private async Task<byte[]> GetImageResult(string imageUrl)
        {
            byte[] imageBytes;
            using (var httpClient = _httpClientFactory.CreateClient())
            {
                imageBytes = await httpClient.GetByteArrayAsync(imageUrl);
            }

            return imageBytes;
        }

        private string GenerateFileUrl(string fileName, FileType fileType, string host)
        {
            //todo проверить что это https Request.Scheme
            return $"https://{host}/{fileType.GetFolderName()}/{fileName}";
        }
    }
}
