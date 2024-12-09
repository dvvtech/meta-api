﻿namespace MetaApi.Services
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
        public async Task<string> UploadFileAsync(IFormFile file, HttpRequest request)
        {
            // Расчёт CRC для загружаемого файла
            string fileCrc = await CalculateCrcAsync(file);

            // Проверка существования файла
            if (_fileCrcService.FileCrcDictionary.TryGetValue(fileCrc, out var existingFileName))
            {
                return GenerateFileUrl(existingFileName, request);                
            }

            // Сохранение файла на диск
            var uniqueFileName = await SaveFileAsync(file);

            // Добавление CRC в словарь
            _fileCrcService.AddFileCrc(fileCrc, uniqueFileName);

            // Генерация публичной ссылки
            return GenerateFileUrl(uniqueFileName, request);            
        }

        private async Task<string> CalculateCrcAsync(IFormFile file)
        {
            using var stream = file.OpenReadStream();
            using var crc32 = new Crc32();
            var hash = await crc32.ComputeHashAsync(stream);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }

        private async Task<string> SaveFileAsync(IFormFile file)
        {
            var uploadsPath = Path.Combine(_env.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
            }

            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsPath, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return uniqueFileName;
        }

        private string GenerateFileUrl(string fileName, HttpRequest request)
        {
            //todo проверить что это https Request.Scheme 
            return $"{request.Scheme}://{request.Host}/uploads/{fileName}";
        }
    }
}
