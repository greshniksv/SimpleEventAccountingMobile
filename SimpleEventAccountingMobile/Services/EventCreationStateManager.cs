using SimpleEventAccountingMobile.Database.DbModels;
using SimpleEventAccountingMobile.Services.Interfaces;

namespace SimpleEventAccountingMobile.Services
{
    public class EventCreationStateManager : IEventCreationStateManager
    {
        private readonly IClientService _clientService;
        private List<Client> _allClients = new();
        private List<Client> _selectedClients = new();
        private List<Client> _availableClientsForAddition = new();

        public EventCreationStateManager(IClientService clientService)
        {
            _clientService = clientService;
        }

        public List<Client> AllClients => _allClients;
        public List<Client> SelectedClients => _selectedClients;
        public List<Client> AvailableClientsForAddition => _availableClientsForAddition;
        public bool IsAddClientModalVisible { get; set; }

        public async Task LoadClientsAsync()
        {
            _allClients = await _clientService.GetAllClientsAsync();
            UpdateAvailableClients();
        }

        public void ShowAddClientModal()
        {
            IsAddClientModalVisible = true;
            UpdateAvailableClients();
        }

        public void HideAddClientModal()
        {
            IsAddClientModalVisible = false;
        }

        public bool AddClient(Client client)
        {
            if (_selectedClients.Any(c => c.Id == client.Id))
                return false;

            _selectedClients.Add(client);
            UpdateAvailableClients();
            return true;
        }

        public bool RemoveClient(Client client)
        {
            var removed = _selectedClients.Remove(client);
            if (removed)
            {
                UpdateAvailableClients();
            }
            return removed;
        }

        public void UpdateAvailableClients()
        {
            _availableClientsForAddition = _allClients
                .Where(c => !_selectedClients.Any(sc => sc.Id == c.Id))
                .ToList();
        }

        public List<Guid> GetSelectedClientIds()
        {
            return _selectedClients.Select(c => c.Id).ToList();
        }
    }
}
