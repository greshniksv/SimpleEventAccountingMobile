using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SimpleEventAccountingMobile.Database.DbContexts;
using SimpleEventAccountingMobile.Database.DbModels;
using SimpleEventAccountingMobile.Dtos;
using SimpleEventAccountingMobile.Services.Interfaces;
using System.IO.Compression;
using System.Text.Json;

namespace SimpleEventAccountingMobile.Services
{
    public class ImportExportService : IImportExportService
    {
        private readonly MainContext _context;
        private readonly ILogger<ImportExportService> _logger;

        public ImportExportService(MainContext context, ILogger<ImportExportService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> ExportDataAsync()
        {
            try
            {
                // Step 1: Unload data from tables into a json file
                var exportData = await GetExportDataAsync();

                CleanupTemporaryFiles(Directory.GetFiles(FileSystem.CacheDirectory, "*.zip"));
                var jsonFilePath = Path.Combine(FileSystem.CacheDirectory, $"export_{DateTime.Now:yyyyMMdd_HHmmss}.json");

                await using (var fileStream = File.Create(jsonFilePath))
                {
                    await JsonSerializer.SerializeAsync(fileStream, exportData, new JsonSerializerOptions
                    {
                        WriteIndented = true,
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    });
                }

                // Step 2: compress the file using zip
                var zipFilePath = Path.Combine(FileSystem.CacheDirectory, $"backup_{DateTime.Now:yyyyMMdd_HHmmss}.zip");
                using (var zipArchive = ZipFile.Open(zipFilePath, ZipArchiveMode.Create))
                {
                    zipArchive.CreateEntryFromFile(jsonFilePath, "data.json");
                }

                // Step 3 & 4: ask user to save the file
                await ShareFile(zipFilePath);
                //if (!saveResult)
                //{
                //    _logger.LogWarning("User cancelled file save operation");
                //    return false;
                //}

                // Step 5: Delete temporary files
                CleanupTemporaryFiles(jsonFilePath);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during export operation");
                return false;
            }
        }

        public async Task<bool> ImportDataAsync()
        {
            try
            {
                // Step 2 & 3: Ask user to specify and upload the file
                var zipFilePath = await PickFileFromUserDevice();
                if (string.IsNullOrEmpty(zipFilePath))
                {
                    _logger.LogWarning("User cancelled file pick operation");
                    return false;
                }

                // Step 4: Unzip the archive
                var extractPath = Path.Combine(FileSystem.CacheDirectory, "import_extract");
                if (Directory.Exists(extractPath))
                    Directory.Delete(extractPath, true);

                Directory.CreateDirectory(extractPath);
                ZipFile.ExtractToDirectory(zipFilePath, extractPath);

                var jsonFilePath = Path.Combine(extractPath, "data.json");
                if (!File.Exists(jsonFilePath))
                {
                    throw new FileNotFoundException("JSON data file not found in archive");
                }

                // Step 5: Insert data from json into the database
                await using (var fileStream = File.OpenRead(jsonFilePath))
                {
                    var importData = await JsonSerializer.DeserializeAsync<ExportData>(fileStream, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    });

                    if (importData != null)
                    {
                        await ClearDatabaseAsync();
                        await ImportDataToDatabaseAsync(importData);
                    }
                }

                // Step 6: Delete temporary files
                CleanupTemporaryFiles(zipFilePath, extractPath);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during import operation");
                return false;
            }
        }

        private async Task<ExportData> GetExportDataAsync()
        {
            var exportData = new ExportData
            {
                CashWallets = await _context.CashWallets
                    .AsNoTracking()
                    .Where(x => !x.Deleted)
                    .Select(x => new CashWalletDto
                    {
                        Id = x.Id,
                        ClientId = x.ClientId,
                        Cash = x.Cash,
                        Deleted = x.Deleted
                    }).ToListAsync(),

                CashWalletHistory = await _context.CashWalletHistory
                    .AsNoTracking()
                    .Select(x => new CashWalletHistoryDto
                    {
                        Id = x.Id,
                        ClientId = x.ClientId,
                        Date = x.Date,
                        EventId = x.EventId,
                        Cash = x.Cash,
                        Comment = x.Comment
                    }).ToListAsync(),

                Clients = await _context.Clients
                    .AsNoTracking()
                    .Where(x => !x.Deleted)
                    .Select(x => new ClientDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Birthday = x.Birthday,
                        Comment = x.Comment,
                        Deleted = x.Deleted
                    }).ToListAsync(),

                Events = await _context.ActionEvents
                    .AsNoTracking()
                    .Where(x => !x.Deleted)
                    .Select(x => new EventDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Description = x.Description,
                        Date = x.Date,
                        Price = x.Price,
                        Deleted = x.Deleted
                    }).ToListAsync(),

                EventClients = await _context.EventClients
                    .AsNoTracking()
                    .Select(x => new EventClientDto
                    {
                        Id = x.Id,
                        EventId = x.EventId,
                        ClientId = x.ClientId
                    }).ToListAsync(),

                Trainings = await _context.Trainings
                    .AsNoTracking()
                    .Where(x => !x.Deleted)
                    .Select(x => new TrainingDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Description = x.Description,
                        Date = x.Date,
                        Deleted = x.Deleted
                    }).ToListAsync(),

                TrainingClients = await _context.TrainingClients
                    .AsNoTracking()
                    .Select(x => new TrainingClientDto
                    {
                        Id = x.Id,
                        TrainingId = x.TrainingId,
                        ClientId = x.ClientId
                    }).ToListAsync(),

                TrainingWallets = await _context.TrainingWallets
                    .AsNoTracking()
                    .Where(x => !x.Deleted)
                    .Select(x => new TrainingWalletDto
                    {
                        Id = x.Id,
                        ClientId = x.ClientId,
                        Count = x.Count,
                        Skip = x.Skip,
                        Free = x.Free,
                        Subscription = x.Subscription,
                        Deleted = x.Deleted
                    }).ToListAsync(),

                TrainingWalletHistory = await _context.TrainingWalletHistory
                    .AsNoTracking()
                    .Select(x => new TrainingWalletHistoryDto
                    {
                        Id = x.Id,
                        ClientId = x.ClientId,
                        TrainingId = x.TrainingId,
                        Date = x.Date,
                        Count = x.Count,
                        Skip = x.Skip,
                        Free = x.Free,
                        Subscription = x.Subscription,
                        Comment = x.Comment
                    }).ToListAsync()
            };

            return exportData;
        }

        private async Task ClearDatabaseAsync()
        {
            // Clear tables in proper order to respect foreign key constraints
            await _context.TrainingWalletHistory.ExecuteDeleteAsync();
            await _context.CashWalletHistory.ExecuteDeleteAsync();
            await _context.EventClients.ExecuteDeleteAsync();
            await _context.TrainingClients.ExecuteDeleteAsync();
            await _context.TrainingWallets.ExecuteDeleteAsync();
            await _context.CashWallets.ExecuteDeleteAsync();
            await _context.Trainings.ExecuteDeleteAsync();
            await _context.ActionEvents.ExecuteDeleteAsync();
            await _context.Clients.ExecuteDeleteAsync();

            await _context.SaveChangesAsync();
        }

        private async Task ImportDataToDatabaseAsync(ExportData importData)
        {
            // Import in proper order to respect foreign key constraints

            // First import entities without foreign key dependencies
            _context.Clients.AddRange(importData.Clients.Select(dto => new Client
            {
                Id = dto.Id,
                Name = dto.Name,
                Birthday = dto.Birthday,
                Comment = dto.Comment,
                Deleted = dto.Deleted
            }));

            _context.ActionEvents.AddRange(importData.Events.Select(dto => new Event
            {
                Id = dto.Id,
                Name = dto.Name,
                Description = dto.Description,
                Date = dto.Date,
                Price = dto.Price,
                Deleted = dto.Deleted
            }));

            _context.Trainings.AddRange(importData.Trainings.Select(dto => new Training
            {
                Id = dto.Id,
                Name = dto.Name,
                Description = dto.Description,
                Date = dto.Date,
                Deleted = dto.Deleted
            }));

            await _context.SaveChangesAsync();

            // Then import entities with foreign key dependencies
            _context.CashWallets.AddRange(importData.CashWallets.Select(dto => new CashWallet
            {
                Id = dto.Id,
                ClientId = dto.ClientId,
                Cash = dto.Cash,
                Deleted = dto.Deleted
            }));

            _context.TrainingWallets.AddRange(importData.TrainingWallets.Select(dto => new TrainingWallet
            {
                Id = dto.Id,
                ClientId = dto.ClientId,
                Count = dto.Count,
                Skip = dto.Skip,
                Free = dto.Free,
                Subscription = dto.Subscription,
                Deleted = dto.Deleted
            }));

            _context.EventClients.AddRange(importData.EventClients.Select(dto => new EventClient
            {
                Id = dto.Id,
                EventId = dto.EventId,
                ClientId = dto.ClientId
            }));

            _context.TrainingClients.AddRange(importData.TrainingClients.Select(dto => new TrainingClient
            {
                Id = dto.Id,
                TrainingId = dto.TrainingId,
                ClientId = dto.ClientId
            }));

            await _context.SaveChangesAsync();

            // Finally import history tables
            _context.CashWalletHistory.AddRange(importData.CashWalletHistory.Select(dto => new CashWalletHistory
            {
                Id = dto.Id,
                ClientId = dto.ClientId,
                Date = dto.Date,
                EventId = dto.EventId,
                Cash = dto.Cash,
                Comment = dto.Comment
            }));

            _context.TrainingWalletHistory.AddRange(importData.TrainingWalletHistory.Select(dto => new TrainingWalletHistory
            {
                Id = dto.Id,
                ClientId = dto.ClientId,
                TrainingId = dto.TrainingId,
                Date = dto.Date,
                Count = dto.Count,
                Skip = dto.Skip,
                Free = dto.Free,
                Subscription = dto.Subscription,
                Comment = dto.Comment
            }));

            await _context.SaveChangesAsync();
        }

        private async Task ShareFile(string fileName)
        {
            await Share.Default.RequestAsync(new ShareFileRequest
            {
                Title = "Экспортированные данные",
                File = new ShareFile(fileName)
            });
        }

        private async Task<bool> SaveFileToUserDevice(string filePath, string suggestedFileName)
        {
            try
            {
                // For MAUI, use file picker to save file
                var fileSaver = Microsoft.Maui.Storage.FilePicker.Default;
                var result = await fileSaver.PickAsync(new PickOptions
                {
                    PickerTitle = "Save Backup File",
                    FileTypes = new FilePickerFileType(
                        new Dictionary<DevicePlatform, IEnumerable<string>>
                        {
                            { DevicePlatform.WinUI, new[] { ".zip" } },
                            { DevicePlatform.MacCatalyst, new[] { ".zip" } },
                            { DevicePlatform.iOS, new[] { ".zip" } },
                            { DevicePlatform.Android, new[] { ".zip" } }
                        })
                });

                if (result != null)
                {
                    using var sourceStream = File.OpenRead(filePath);
                    //using var destinationStream = await result.
                    //await sourceStream.CopyToAsync(destinationStream);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving file to user device");
                return false;
            }
        }

        private async Task<string?> PickFileFromUserDevice()
        {
            try
            {
                var filePicker = FilePicker.Default;

                var pickOptions = new PickOptions
                {
                    PickerTitle = "Select Backup File",
                    FileTypes = GetFilePickerFileType()
                };

                var result = await filePicker.PickAsync(pickOptions);

                // For Android, we need to copy the file to a temporary location
                // because we might not have direct access to the original file
                if (result != null)
                {
                    // Copy the file to app's cache directory for processing
                    var tempFilePath = Path.Combine(FileSystem.CacheDirectory, $"import_{DateTime.Now:yyyyMMdd_HHmmss}.zip");

                    using (var sourceStream = await result.OpenReadAsync())
                    using (var destinationStream = File.Create(tempFilePath))
                    {
                        await sourceStream.CopyToAsync(destinationStream);
                    }

                    return tempFilePath;
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error picking file from user device");
                return null;
            }
        }

        private FilePickerFileType GetFilePickerFileType()
        {
            if (DeviceInfo.Platform == DevicePlatform.Android)
            {
                // Android requires MIME types
                return new FilePickerFileType(
                    new Dictionary<DevicePlatform, IEnumerable<string>>
                    {
                { DevicePlatform.Android, new[] { "application/zip", "application/x-zip-compressed" } }
                    });
            }
            else
            {
                // Other platforms use file extensions
                return new FilePickerFileType(
                    new Dictionary<DevicePlatform, IEnumerable<string>>
                    {
                { DevicePlatform.WinUI, new[] { ".zip" } },
                { DevicePlatform.MacCatalyst, new[] { ".zip" } },
                { DevicePlatform.iOS, new[] { ".zip" } },
                { DevicePlatform.Android, new[] { ".zip" } } // This line is redundant but kept for completeness
                    });
            }
        }

        private async Task<string?> PickFileFromUserDevice2()
        {
            try
            {
                var filePicker = Microsoft.Maui.Storage.FilePicker.Default;
                var result = await filePicker.PickAsync(new PickOptions
                {
                    PickerTitle = "Select Backup File",
                    FileTypes = new FilePickerFileType(
                        new Dictionary<DevicePlatform, IEnumerable<string>>
                        {
                            { DevicePlatform.WinUI, new[] { ".zip" } },
                            { DevicePlatform.MacCatalyst, new[] { ".zip" } },
                            { DevicePlatform.iOS, new[] { ".zip" } },
                            { DevicePlatform.Android, new[] { ".zip" } }
                        })
                });

                return result?.FullPath;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error picking file from user device");
                return null;
            }
        }

        private void CleanupTemporaryFiles(params string[] filePaths)
        {
            foreach (var filePath in filePaths)
            {
                try
                {
                    if (File.Exists(filePath))
                        File.Delete(filePath);

                    if (Directory.Exists(filePath))
                        Directory.Delete(filePath, true);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error deleting temporary file: {FilePath}", filePath);
                }
            }
        }
    }
}
