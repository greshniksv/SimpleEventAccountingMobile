using SimpleEventAccountingMobile.Database.DbModels;

namespace SimpleEventAccountingMobile.Services.Interfaces
{
    public interface IClientService
    {
        Task<List<Client>> GetAllClientsAsync();

        Task<List<Client>> GetDeletedAsync();

        Task RestoreDeletedAsync(Guid clientId);

        Task<Client?> GetClientByIdAsync(Guid clientId, bool includeWallets = true);

        Task<Guid> CreateClientAsync(Client client);

        Task UpdateClientAsync(Client client);

        TrainingWalletHistory CreateTrainingWalletHistory(TrainingWallet wallet, string comment = "Ручное изменение");

        CashWalletHistory CreateCashWalletHistory(CashWallet wallet, string comment = "Ручное изменение");
    }
}
