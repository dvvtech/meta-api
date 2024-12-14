using MetaApi.Models.VirtualFit;

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
        public async Task<string> UploadFileAsync(IFormFile file, FileType fileType, HttpRequest request)
        {            
            // Расчёт CRC для загружаемого файла
            string fileCrc = await CalculateCrcAsync(file);

            // Проверка существования файла
            if (_fileCrcService.FileCrcDictionary.TryGetValue(fileCrc, out var existingFileName))
            {
                return GenerateFileUrl(existingFileName, fileType, request);                
            }
            
            // Сохранение файла на диск
            var uniqueFileName = await SaveFileAsync(file, fileType);
            
            // Добавление CRC в словарь
            _fileCrcService.AddFileCrc(fileCrc, uniqueFileName);

            // Генерация публичной ссылки
            return GenerateFileUrl(uniqueFileName, fileType, request);            
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

        private string GenerateFileUrl(string fileName, FileType fileType, HttpRequest request)
        {
            //todo проверить что это https Request.Scheme
            return $"{request.Scheme}://{request.Host}/{fileType.GetFolderName()}/{fileName}";
        }
    }
}
