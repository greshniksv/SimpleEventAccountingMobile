using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SimpleEventAccountingMobile.Database.DbModels;
using SimpleEventAccountingMobile.Database.Interfaces;
using SimpleEventAccountingMobile.Services.Interfaces;

namespace SimpleEventAccountingMobile.Services
{
    public class SettingsService(IMainContext context) : ISettingsService
    {
        private readonly IMainContext _context = context ?? throw new ArgumentNullException(nameof(context));

        public async Task AddAsync<T>(string key, T data)
            where T : struct
        {
            if (!typeof(T).IsValueType)
            {
                throw new ArgumentException("T must be a value type.");
            }

            var setting = new Setting
            {
                Key = key,
                Type = typeof(T).AssemblyQualifiedName ?? string.Empty,
                Value = JsonSerializer.Serialize(data)
            };

            _context.Settings.Add(setting);
            await _context.SaveChangesAsync();
        }

        public async Task<T?> GetAsync<T>(string key, T? defaultValue)
            where T : struct
        {
            var setting = await _context.Settings.FirstOrDefaultAsync(s => s.Key == key);
            var type = typeof(T).AssemblyQualifiedName ?? string.Empty;

            if (setting == null)
            {
                return defaultValue;
            }

            if (setting == null || !type.Equals(setting.Type))
            {
                throw new Exception($"Setting with key '{key}' not found or type mismatch. Type: '{type}'");
            }

            return JsonSerializer.Deserialize<T>(setting.Value);
        }
    }
}
