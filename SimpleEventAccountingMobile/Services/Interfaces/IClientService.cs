using SimpleEventAccountingMobile.Database.DbModels;

namespace SimpleEventAccountingMobile.Services.Interfaces
{
    public interface IClientService
    {
        Task<List<Client>> GetAllClientsAsync();

        Task<List<Client>> GetDeletedAsync();

        Task RestoreDeletedAsync(Guid clientId);
    }
}
