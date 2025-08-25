using SimpleEventAccountingMobile.Database.DbModels;

namespace SimpleEventAccountingMobile.Services.Interfaces
{
    public interface IEventCreationStateManager
    {
        List<Client> AllClients { get; }
        List<Client> SelectedClients { get; }
        List<Client> AvailableClientsForAddition { get; }
        bool IsAddClientModalVisible { get; set; }

        Task LoadClientsAsync();
        void ShowAddClientModal();
        void HideAddClientModal();
        bool AddClient(Client client);
        bool RemoveClient(Client client);
        void UpdateAvailableClients();
        List<Guid> GetSelectedClientIds();
    }
}
