namespace SimpleEventAccountingMobile.Services.Interfaces
{
    public interface ISettingsService
    {
        Task AddAsync<T>(string key, T data) where T : struct;
        Task<T?> GetAsync<T>(string key, T? defaultValue) where T : struct;
    }
}
