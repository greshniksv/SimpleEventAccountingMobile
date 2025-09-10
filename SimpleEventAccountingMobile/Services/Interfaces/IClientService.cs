using SimpleEventAccountingMobile.Database.DbModels;
using SimpleEventAccountingMobile.Dtos;

namespace SimpleEventAccountingMobile.Services.Interfaces
{
    public interface IClientService
    {
        Task<List<Client>> GetAllClientsAsync();

        Task<List<Client>> GetDeletedAsync();

        Task RestoreDeletedAsync(Guid clientId);

        Task<FullClientDto?> GetClientByIdAsync(Guid clientId);

        Task<List<ClientGroupInfoDto>> GetGroups(Guid clientId);

        Task<Guid> CreateClientAsync(FullClientDto clientDto);

        Task UpdateClientAsync(FullClientDto clientDto);

        TrainingWalletHistory CreateTrainingWalletHistory(TrainingWallet wallet, string comment = "Ручное изменение");

        CashWalletHistory CreateCashWalletHistory(CashWallet wallet, string comment = "Ручное изменение");
    }
}
