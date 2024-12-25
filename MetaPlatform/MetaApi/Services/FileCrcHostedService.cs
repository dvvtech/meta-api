using MetaApi.Consts;
using System.Collections.Concurrent;
using System.Security.Cryptography;

namespace MetaApi.Services
{
    public class FileCrcHostedService : IHostedService
    {
        private readonly string _uploadsFolderPath;
        //private readonly string _resultFolderPath;
        private readonly ILogger<FileCrcHostedService> _logger;
        private readonly ConcurrentDictionary<string, string> _fileCrcDictionary = new();

        public FileCrcHostedService(IWebHostEnvironment env,
                                    ILogger<FileCrcHostedService> logger)
        {
            _logger = logger;            
            _uploadsFolderPath = Path.Combine(env.WebRootPath, "uploads");
            //_resultFolderPath = Path.Combine(env.WebRootPath, "result");
        }

        // Публичное свойство для доступа к словарю
        public IReadOnlyDictionary<string, string> FileCrcDictionary => _fileCrcDictionary;

        // Метод для добавления новых значений
        public void AddFileCrc(string crc, string fileName)
        {
            _fileCrcDictionary.TryAdd(crc, fileName);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("FileCrcHostedService запущен. Начинается обработка файлов...");

            try
            {                
                //ProcessFiles2(_uploadsFolderPath);
                //ProcessFiles2(_resultFolderPath);
                ProcessFiles();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка при обработке файлов: {ex.Message}");
            }

            _logger.LogInformation("FileCrcHostedService завершил обработку файлов.");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("FileCrcHostedService остановлен.");
            return Task.CompletedTask;
        }        

        private void ProcessFiles()
        {
            if (!Directory.Exists(_uploadsFolderPath))
            {
                _logger.LogWarning($"Папка {_uploadsFolderPath} не найдена.");
                return;
            }

            var files = Directory.GetFiles(_uploadsFolderPath);
            foreach (var filePath in files)
            {
                try
                {                    
                    var fileName = Path.GetFileName(filePath);
                    if (!EndsWithSuffix(fileName))
                    {
                        var crc = CalculateCrc(filePath);
                        // Добавляем CRC и имя файла в словарь
                        _fileCrcDictionary[crc] = fileName;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ошибка при обработке файла {filePath}: {ex.Message}");
                }
            }

            _logger.LogInformation($"Обработано файлов: {_fileCrcDictionary.Count}");
        }

        /// <summary>
        /// Завершается ли название файла _t
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="suffix"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private bool EndsWithSuffix(string fileName, string suffix = "_t")
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentException("Строка не должна быть пустой или null", nameof(fileName));

            var lastDotIndex = fileName.LastIndexOf('.');
            if (lastDotIndex == -1)
                return false; // Нет расширения файла

            // Проверяем, находится ли суффикс перед расширением
            var baseName = fileName.Substring(0, lastDotIndex);
            return baseName.EndsWith(suffix);
        }


        private string CalculateCrc(string filePath)
        {
            using var stream = File.OpenRead(filePath);
            using var crc32 = new Crc32();
            var hash = crc32.ComputeHash(stream);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }

        //public static string AddSuffixToFilePath(string filePath, string suffix = "_t")
        //{
        //    if (string.IsNullOrEmpty(filePath))
        //    {
        //        throw new ArgumentException("Путь к файлу не может быть пустым или null", nameof(filePath));
        //    }

        //    // Найти позицию последней точки для определения расширения файла
        //    var lastDotIndex = filePath.LastIndexOf('.');
        //    if (lastDotIndex == -1 || lastDotIndex <= filePath.LastIndexOf(Path.DirectorySeparatorChar))
        //    {
        //        throw new ArgumentException("Путь не содержит корректного расширения файла", nameof(filePath));
        //    }

        //    // Вставить суффикс перед расширением
        //    return filePath.Insert(lastDotIndex, suffix);
        //}

        //private void ProcessFiles2(string path)
        //{
        //    var files = Directory.GetFiles(path);
        //    foreach (var filePath in files)
        //    {
        //        byte[] imgBytes = File.ReadAllBytes(filePath);
        //        byte[] resizedBytes = ImageResizer.ResizeImage(imgBytes, FittingConstants.THUMBNAIL_WIDTH);

        //        string newfilePath = AddSuffixToFilePath(filePath);

        //        File.WriteAllBytes(newfilePath, resizedBytes);
        //    }
        //}
    }

    // Простая реализация CRC32
    public class Crc32 : HashAlgorithm
    {
        private static readonly uint[] Table = Enumerable.Range(0, 256).Select(i =>
        {
            var value = (uint)i;
            for (var j = 0; j < 8; j++)
                value = (value & 1) != 0 ? (value >> 1) ^ 0xedb88320 : value >> 1;
            return value;
        }).ToArray();

        private uint _crc = 0xffffffff;

        public override void Initialize() => _crc = 0xffffffff;

        protected override void HashCore(byte[] array, int ibStart, int cbSize)
        {
            for (var i = ibStart; i < cbSize; i++)
                _crc = (_crc >> 8) ^ Table[(array[i] ^ _crc) & 0xff];
        }

        protected override byte[] HashFinal()
        {
            _crc ^= 0xffffffff;
            return BitConverter.GetBytes(_crc);
        }
    }
}
