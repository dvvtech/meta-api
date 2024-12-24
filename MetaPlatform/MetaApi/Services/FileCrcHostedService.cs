using System.Collections.Concurrent;
using System.Security.Cryptography;

namespace MetaApi.Services
{
    public class FileCrcHostedService : IHostedService
    {
        private readonly string _uploadsFolderPath;
        private readonly ILogger<FileCrcHostedService> _logger;
        private readonly ConcurrentDictionary<string, string> _fileCrcDictionary = new();

        public FileCrcHostedService(IWebHostEnvironment env,
                                    ILogger<FileCrcHostedService> logger)
        {
            _logger = logger;            
            _uploadsFolderPath = Path.Combine(env.WebRootPath, "uploads");
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
                    var crc = CalculateCrc(filePath);
                    var fileName = Path.GetFileName(filePath);

                    // Добавляем CRC и имя файла в словарь
                    _fileCrcDictionary[crc] = fileName;
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ошибка при обработке файла {filePath}: {ex.Message}");
                }
            }

            _logger.LogInformation($"Обработано файлов: {_fileCrcDictionary.Count}");
        }

        private string CalculateCrc(string filePath)
        {
            using var stream = File.OpenRead(filePath);
            using var crc32 = new Crc32();
            var hash = crc32.ComputeHash(stream);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }
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
