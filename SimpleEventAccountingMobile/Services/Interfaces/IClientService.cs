using SimpleEventAccountingMobile.Database.DbModels;

namespace SimpleEventAccountingMobile.Services.Interfaces
{
    public interface IClientService
    {
        Task<List<Client>> GetAllClientsAsync();
        // Добавьте другие методы по необходимости
    }
}
