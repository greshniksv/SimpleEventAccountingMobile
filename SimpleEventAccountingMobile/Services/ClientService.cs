using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SimpleEventAccountingMobile.Database.DbContexts;
using SimpleEventAccountingMobile.Database.DbModels;
using SimpleEventAccountingMobile.Services.Interfaces;

namespace SimpleEventAccountingMobile.Services
{
    public class ClientService(MainContext context, ILogger<ClientService> logger) : IClientService
    {
        private readonly MainContext _context = context;
        private readonly ILogger<ClientService> _logger = logger;

        public async Task<List<Client>> GetAllClientsAsync()
        {
            _logger.LogInformation("Getting all active clients");

            return await _context.Clients
                .AsNoTracking()
                .Where(c => c.DeletedAt == null)
                .ToListAsync();
        }

        public async Task<List<Client>> GetDeletedAsync()
        {
            _logger.LogInformation("Getting all deleted clients");

            return await _context.Clients
                .AsNoTracking()
                .Where(c => c.DeletedAt != null)
                .ToListAsync();
        }

        public async Task RestoreDeletedAsync(Guid clientId)
        {
            _logger.LogInformation("Restoring deleted client with ID: {ClientId}", clientId);

            var client = await _context.Clients.FirstOrDefaultAsync(x => x.Id == clientId);
            if (client == null)
            {
                _logger.LogWarning("Client not found for restoration: {ClientId}", clientId);
                throw new Exception("Client not found exception");
            }

            client.DeletedAt = null;

            _context.Clients.Update(client);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Client restored successfully: {ClientId}", clientId);
        }

        public async Task<Client?> GetClientByIdAsync(Guid clientId, bool includeWallets = true)
        {
            _logger.LogInformation("Getting client by ID: {ClientId}", clientId);

            var query = _context.Clients.AsNoTracking();

            if (includeWallets)
            {
                query = query
                    .Include(x => x.TrainingWallets)
                    .Include(x => x.CashWallets);
            }

            var client = await query.FirstOrDefaultAsync(x => x.Id == clientId);

            if (client == null)
            {
                _logger.LogWarning("Client not found: {ClientId}", clientId);
            }
            else
            {
                _logger.LogInformation("Client found: {ClientId}", clientId);
            }

            return client;
        }

        public async Task<Guid> CreateClientAsync(Client client)
        {
            _logger.LogInformation("Creating new client: {ClientName}", client.Name);

            try
            {
                await _context.Clients.AddAsync(client);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Client created successfully: {ClientId}", client.Id);
                return client.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating client: {ClientName}", client.Name);
                throw;
            }
        }

        public async Task UpdateClientAsync(Client client)
        {
            _logger.LogInformation("Updating client: {ClientId}", client.Id);

            try
            {
                _context.Clients.Update(client);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Client updated successfully: {ClientId}", client.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating client: {ClientId}", client.Id);
                throw;
            }
        }

        public TrainingWalletHistory CreateTrainingWalletHistory(TrainingWallet wallet, string comment = "Ручное изменение")
        {
            _logger.LogDebug("Creating training wallet history entry");

            return new TrainingWalletHistory()
            {
                Skip = wallet.Skip,
                Count = wallet.Count,
                Subscription = wallet.Subscription,
                Free = wallet.Free,
                Comment = comment,
                Date = DateTime.Now,
                TrainingId = null
            };
        }

        public CashWalletHistory CreateCashWalletHistory(CashWallet wallet, string comment = "Ручное изменение")
        {
            _logger.LogDebug("Creating cash wallet history entry");

            return new CashWalletHistory()
            {
                Cash = wallet.Cash,
                Comment = comment,
                Date = DateTime.Now,
                EventId = null
            };
        }
    }
}
