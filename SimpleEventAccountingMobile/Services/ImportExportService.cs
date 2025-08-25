using Microsoft.EntityFrameworkCore;
using SimpleEventAccountingMobile.Database.DbContexts;
using SimpleEventAccountingMobile.Database.DbModels;
using SimpleEventAccountingMobile.Database.Interfaces;
using SimpleEventAccountingMobile.Services.Interfaces;
using System.IO.Compression;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SimpleEventAccountingMobile.Services
{
    public class ImportExportService
    {
        private readonly IMainContext _context;
        private readonly string _tempFolder;

        public ImportExportService(IMainContext context)
        {
            _context = context;
            _tempFolder = Path.Combine(FileSystem.Current.CacheDirectory, "ImportExport");
            Directory.CreateDirectory(_tempFolder);
        }

        // DTO классы для экспорта без циклических ссылок
        public class ExportData
        {
            public List<ClientDto> Clients { get; set; } = new();
            public List<CashWalletDto> CashWallets { get; set; } = new();
            public List<CashWalletHistoryDto> CashWalletHistory { get; set; } = new();
            public List<EventDto> Events { get; set; } = new();
            public List<EventClientDto> EventClients { get; set; } = new();
            public List<TrainingDto> Trainings { get; set; } = new();
            public List<TrainingClientDto> TrainingClients { get; set; } = new();
            public List<TrainingWalletDto> TrainingWallets { get; set; } = new();
            public List<TrainingWalletHistoryDto> TrainingWalletHistory { get; set; } = new();
        }

        public class ClientDto
        {
            public Guid Id { get; set; }
            public string? Name { get; set; }
            public DateTime Birthday { get; set; }
            public string? Comment { get; set; }
            public bool Deleted { get; set; }
        }

        public class CashWalletDto
        {
            public Guid Id { get; set; }
            public Guid ClientId { get; set; }
            public decimal Cash { get; set; }
            public bool Deleted { get; set; }
        }

        public class CashWalletHistoryDto
        {
            public Guid Id { get; set; }
            public Guid ClientId { get; set; }
            public DateTime Date { get; set; }
            public Guid? EventId { get; set; }
            public decimal Cash { get; set; }
            public string? Comment { get; set; }
        }

        public class EventDto
        {
            public Guid Id { get; set; }
            public string? Name { get; set; }
            public string? Description { get; set; }
            public DateTime Date { get; set; }
            public decimal Price { get; set; }
            public bool Deleted { get; set; }
        }

        public class EventClientDto
        {
            public Guid Id { get; set; }
            public Guid EventId { get; set; }
            public Guid ClientId { get; set; }
        }

        public class TrainingDto
        {
            public Guid Id { get; set; }
            public string? Name { get; set; }
            public string? Description { get; set; }
            public DateTime Date { get; set; }
            public bool Deleted { get; set; }
        }

        public class TrainingClientDto
        {
            public Guid Id { get; set; }
            public Guid TrainingId { get; set; }
            public Guid ClientId { get; set; }
        }

        public class TrainingWalletDto
        {
            public Guid Id { get; set; }
            public Guid ClientId { get; set; }
            public decimal Count { get; set; }
            public decimal Skip { get; set; }
            public decimal Free { get; set; }
            public bool Subscription { get; set; }
            public bool Deleted { get; set; }
        }

        public class TrainingWalletHistoryDto
        {
            public Guid Id { get; set; }
            public Guid ClientId { get; set; }
            public Guid? TrainingId { get; set; }
            public DateTime Date { get; set; }
            public decimal Count { get; set; }
            public decimal Skip { get; set; }
            public decimal Free { get; set; }
            public bool Subscription { get; set; }
            public string? Comment { get; set; }
        }

        // Часть 1 - Метод экспорта данных
        public async Task<string> ExportDataAsync()
        {
            try
            {
                var exportData = await PrepareExportDataAsync();
                string filePath = await CreateExportFileAsync(exportData);
                return filePath;
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при экспорте данных: {ex.Message}", ex);
            }
        }

        private async Task<ExportData> PrepareExportDataAsync()
        {
            return new ExportData
            {
                Clients = await _context.Clients.Select(c => new ClientDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Birthday = c.Birthday,
                    Comment = c.Comment,
                    Deleted = c.Deleted
                }).ToListAsync(),

                CashWallets = await _context.CashWallets.Select(cw => new CashWalletDto
                {
                    Id = cw.Id,
                    ClientId = cw.ClientId,
                    Cash = cw.Cash,
                    Deleted = cw.Deleted
                }).ToListAsync(),

                CashWalletHistory = await _context.CashWalletHistory.Select(cwh => new CashWalletHistoryDto
                {
                    Id = cwh.Id,
                    ClientId = cwh.ClientId,
                    Date = cwh.Date,
                    EventId = cwh.EventId,
                    Cash = cwh.Cash,
                    Comment = cwh.Comment
                }).ToListAsync(),

                Events = await _context.ActionEvents.Select(e => new EventDto
                {
                    Id = e.Id,
                    Name = e.Name,
                    Description = e.Description,
                    Date = e.Date,
                    Price = e.Price,
                    Deleted = e.Deleted
                }).ToListAsync(),

                EventClients = await _context.EventClients.Select(ec => new EventClientDto
                {
                    Id = ec.Id,
                    EventId = ec.EventId,
                    ClientId = ec.ClientId
                }).ToListAsync(),

                Trainings = await _context.Trainings.Select(t => new TrainingDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    Description = t.Description,
                    Date = t.Date,
                    Deleted = t.Deleted
                }).ToListAsync(),

                TrainingClients = await _context.TrainingClients.Select(tc => new TrainingClientDto
                {
                    Id = tc.Id,
                    TrainingId = tc.TrainingId,
                    ClientId = tc.ClientId
                }).ToListAsync(),

                TrainingWallets = await _context.TrainingWallets.Select(tw => new TrainingWalletDto
                {
                    Id = tw.Id,
                    ClientId = tw.ClientId,
                    Count = tw.Count,
                    Skip = tw.Skip,
                    Free = tw.Free,
                    Subscription = tw.Subscription,
                    Deleted = tw.Deleted
                }).ToListAsync(),

                TrainingWalletHistory = await _context.TrainingWalletHistory.Select(twh => new TrainingWalletHistoryDto
                {
                    Id = twh.Id,
                    ClientId = twh.ClientId,
                    TrainingId = twh.TrainingId,
                    Date = twh.Date,
                    Count = twh.Count,
                    Skip = twh.Skip,
                    Free = twh.Free,
                    Subscription = twh.Subscription,
                    Comment = twh.Comment
                }).ToListAsync()
            };
        }

        private async Task<string> CreateExportFileAsync(ExportData exportData)
        {
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string fileName = $"backup_{timestamp}.seam";
            string tempJsonFile = Path.Combine(_tempFolder, $"data_{timestamp}.json");
            string finalFilePath = Path.Combine(FileSystem.Current.CacheDirectory, fileName);

            var jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            string jsonString = JsonSerializer.Serialize(exportData, jsonOptions);
            await File.WriteAllTextAsync(tempJsonFile, jsonString);

            using (var archive = ZipFile.Open(finalFilePath, ZipArchiveMode.Create))
            {
                archive.CreateEntryFromFile(tempJsonFile, "data.json");
            }

            File.Delete(tempJsonFile);
            return finalFilePath;
        }

        public async Task<bool> ImportDataAsync(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                    return false;

                string tempFolder = Path.Combine(FileSystem.Current.CacheDirectory, "import_temp");
                Directory.CreateDirectory(tempFolder);

                try
                {
                    // Извлекаем данные из архива
                    using (var archive = ZipFile.OpenRead(filePath))
                    {
                        var dataEntry = archive.GetEntry("data.json");
                        if (dataEntry == null)
                            return false;

                        string tempJsonFile = Path.Combine(tempFolder, "data.json");
                        dataEntry.ExtractToFile(tempJsonFile, true);

                        string jsonString = await File.ReadAllTextAsync(tempJsonFile);
                        var importData = JsonSerializer.Deserialize<ExportData>(jsonString);

                        if (importData == null)
                            return false;

                        await ImportDataToDatabaseAsync(importData);
                        return true;
                    }
                }
                finally
                {
                    if (Directory.Exists(tempFolder))
                        Directory.Delete(tempFolder, true);
                }
            }
            catch (Exception ex)
            {
                // Логирование ошибки
                System.Diagnostics.Debug.WriteLine($"Ошибка импорта: {ex.Message}");
                return false;
            }
        }

        private async Task ImportDataToDatabaseAsync(ExportData importData)
        {
            using var transaction = await _context.GetDatabase().BeginTransactionAsync();

            try
            {
                // Очищаем существующие данные
                await ClearExistingDataAsync();

                // Импортируем клиентов
                if (importData.Clients?.Any() == true)
                {
                    foreach (var clientDto in importData.Clients)
                    {
                        var client = new Client
                        {
                            Id = clientDto.Id,
                            Name = clientDto.Name,
                            Birthday = clientDto.Birthday,
                            Comment = clientDto.Comment,
                            Deleted = clientDto.Deleted
                        };
                        _context.Clients.Add(client);
                    }
                    await _context.SaveChangesAsync();
                }

                // Импортируем события
                if (importData.Events?.Any() == true)
                {
                    foreach (var eventDto in importData.Events)
                    {
                        var eventEntity = new Event
                        {
                            Id = eventDto.Id,
                            Name = eventDto.Name,
                            Description = eventDto.Description,
                            Date = eventDto.Date,
                            Price = eventDto.Price,
                            Deleted = eventDto.Deleted
                        };
                        _context.ActionEvents.Add(eventEntity);
                    }
                    await _context.SaveChangesAsync();
                }

                // Импортируем тренировки
                if (importData.Trainings?.Any() == true)
                {
                    foreach (var trainingDto in importData.Trainings)
                    {
                        var training = new Training
                        {
                            Id = trainingDto.Id,
                            Name = trainingDto.Name,
                            Description = trainingDto.Description,
                            Date = trainingDto.Date,
                            Deleted = trainingDto.Deleted
                        };
                        _context.Trainings.Add(training);
                    }
                    await _context.SaveChangesAsync();
                }

                // Импортируем денежные кошельки
                if (importData.CashWallets?.Any() == true)
                {
                    foreach (var walletDto in importData.CashWallets)
                    {
                        var wallet = new CashWallet
                        {
                            Id = walletDto.Id,
                            ClientId = walletDto.ClientId,
                            Cash = walletDto.Cash,
                            Deleted = walletDto.Deleted
                        };
                        _context.CashWallets.Add(wallet);
                    }
                    await _context.SaveChangesAsync();
                }

                // Импортируем тренировочные кошельки
                if (importData.TrainingWallets?.Any() == true)
                {
                    foreach (var walletDto in importData.TrainingWallets)
                    {
                        var wallet = new TrainingWallet
                        {
                            Id = walletDto.Id,
                            ClientId = walletDto.ClientId,
                            Count = walletDto.Count,
                            Skip = walletDto.Skip,
                            Free = walletDto.Free,
                            Subscription = walletDto.Subscription,
                            Deleted = walletDto.Deleted
                        };
                        _context.TrainingWallets.Add(wallet);
                    }
                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private async Task ClearExistingDataAsync()
        {
            // Очищаем данные в правильном порядке с учетом внешних ключей
            _context.TrainingWallets.RemoveRange(_context.TrainingWallets);
            _context.CashWallets.RemoveRange(_context.CashWallets);
            _context.Trainings.RemoveRange(_context.Trainings);
            _context.ActionEvents.RemoveRange(_context.ActionEvents);
            _context.Clients.RemoveRange(_context.Clients);

            await _context.SaveChangesAsync();
        }

        private async Task<ExportData> GetDataFromDatabaseAsync()
        {
            var exportData = new ExportData();

            // Получаем всех клиентов
            var clients = await _context.Clients.ToListAsync();
            exportData.Clients = clients.Select(c => new ClientDto
            {
                Id = c.Id,
                Name = c.Name,
                Birthday = c.Birthday,
                Comment = c.Comment,
                Deleted = c.Deleted
            }).ToList();

            // Получаем все события
            var events = await _context.ActionEvents.ToListAsync();
            exportData.Events = events.Select(e => new EventDto
            {
                Id = e.Id,
                Name = e.Name,
                Description = e.Description,
                Date = e.Date,
                Price = e.Price,
                Deleted = e.Deleted
            }).ToList();

            // Получаем все тренировки
            var trainings = await _context.Trainings.ToListAsync();
            exportData.Trainings = trainings.Select(t => new TrainingDto
            {
                Id = t.Id,
                Name = t.Name,
                Description = t.Description,
                Date = t.Date,
                Deleted = t.Deleted
            }).ToList();

            // Получаем денежные кошельки
            var cashWallets = await _context.CashWallets.ToListAsync();
            exportData.CashWallets = cashWallets.Select(w => new CashWalletDto
            {
                Id = w.Id,
                ClientId = w.ClientId,
                Cash = w.Cash,
                Deleted = w.Deleted
            }).ToList();

            // Получаем тренировочные кошельки
            var trainingWallets = await _context.TrainingWallets.ToListAsync();
            exportData.TrainingWallets = trainingWallets.Select(w => new TrainingWalletDto
            {
                Id = w.Id,
                ClientId = w.ClientId,
                Count = w.Count,
                Skip = w.Skip,
                Free = w.Free,
                Subscription = w.Subscription,
                Deleted = w.Deleted
            }).ToList();

            return exportData;
        }

        public async Task<bool> ValidateBackupFileAsync(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                    return false;

                using var archive = ZipFile.OpenRead(filePath);
                var dataEntry = archive.GetEntry("data.json");

                if (dataEntry == null)
                    return false;

                // Проверяем, что файл можно десериализовать
                using var stream = dataEntry.Open();
                using var reader = new StreamReader(stream);
                string jsonContent = await reader.ReadToEndAsync();

                var testData = JsonSerializer.Deserialize<ExportData>(jsonContent);
                return testData != null;
            }
            catch
            {
                return false;
            }
        }

        public async Task<BackupInfo> GetBackupInfoAsync(string filePath)
        {
            try
            {
                if (!await ValidateBackupFileAsync(filePath))
                    return null;

                var fileInfo = new FileInfo(filePath);

                using var archive = ZipFile.OpenRead(filePath);
                var dataEntry = archive.GetEntry("data.json");
                using var stream = dataEntry.Open();
                using var reader = new StreamReader(stream);
                string jsonContent = await reader.ReadToEndAsync();

                var exportData = JsonSerializer.Deserialize<ExportData>(jsonContent);

                return new BackupInfo
                {
                    FileName = fileInfo.Name,
                    FileSize = fileInfo.Length,
                    CreatedDate = fileInfo.CreationTime,
                    ClientsCount = exportData.Clients?.Count ?? 0,
                    EventsCount = exportData.Events?.Count ?? 0,
                    TrainingsCount = exportData.Trainings?.Count ?? 0,
                    CashWalletsCount = exportData.CashWallets?.Count ?? 0,
                    TrainingWalletsCount = exportData.TrainingWallets?.Count ?? 0
                };
            }
            catch
            {
                return null;
            }
        }

        private string GenerateBackupFileName()
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            return $"backup_{timestamp}.zip";
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    

        // Класс для информации о бэкапе
        public class BackupInfo
        {
            public string FileName { get; set; }
            public long FileSize { get; set; }
            public DateTime CreatedDate { get; set; }
            public int ClientsCount { get; set; }
            public int EventsCount { get; set; }
            public int TrainingsCount { get; set; }
            public int CashWalletsCount { get; set; }
            public int TrainingWalletsCount { get; set; }
        }

        private async Task ShareFile(string fileName)
        {
            await Share.Default.RequestAsync(new ShareFileRequest
            {
                Title = "Экспортированные данные",
                File = new ShareFile(fileName)
            });
        }

    }
}
