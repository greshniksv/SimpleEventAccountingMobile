using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SimpleEventAccountingMobile.Database.DbContexts;
using SimpleEventAccountingMobile.Database.DbModels;
using SimpleEventAccountingMobile.Dtos;
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

        public async Task<List<ClientGroupInfoDto>> GetGroups(Guid clientId)
        {
            var groups = await _context.ClientGroupBindings
                .Where(x => x.ClientId == clientId)
                .Include(x => x.ClientGroup)
                .Select(x => x.ClientGroup).ToListAsync();

            return groups.Select(x=> new ClientGroupInfoDto()
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description
            }).ToList();
        }

        public async Task<FullClientDto?> GetClientByIdAsync(Guid clientId)
        {
            _logger.LogInformation("Getting client by ID: {ClientId}", clientId);

            var client = await _context.Clients
                .AsNoTracking()
                .Include(x => x.TrainingWallets)
                .Include(x => x.CashWallets)
                .Include(x => x.TrainingWalletHistory)
                .Include(x => x.CashWalletHistory)
                .FirstOrDefaultAsync(x => x.Id == clientId);

            if (client == null)
            {
                _logger.LogWarning("Client not found: {ClientId}", clientId);
                return null;
            }

            var trainingWallet = client.TrainingWallets.FirstOrDefault();
            var cashWallet = client.CashWallets.FirstOrDefault();

            var dto = new FullClientDto
            {
                Id = client.Id,
                Name = client.Name,
                Birthday = client.Birthday,
                Comment = client.Comment,
                Subscription = trainingWallet?.Subscription ?? false,
                TrainingCount = (int?)trainingWallet?.Count ?? 0,
                TrainingSkip = (int?)trainingWallet?.Skip ?? 0,
                TrainingFree = (int?)trainingWallet?.Free ?? 0,
                CashAmount = (int?)cashWallet?.Cash ?? 0
            };

            _logger.LogInformation("Client found: {ClientId}", clientId);
            return dto;
        }

        public async Task<Guid> CreateClientAsync(FullClientDto clientDto)
        {
            _logger.LogInformation("Creating new client: {ClientName}", clientDto.Name);

            try
            {
                var client = new Client
                {
                    Name = clientDto.Name,
                    Birthday = clientDto.Birthday,
                    Comment = clientDto.Comment,
                    TrainingWallets = new List<TrainingWallet>
                    {
                        new TrainingWallet
                        {
                            Subscription = clientDto.Subscription,
                            Count = clientDto.TrainingCount,
                            Skip = clientDto.TrainingSkip,
                            Free = clientDto.TrainingFree
                        }
                    },
                    CashWallets = new List<CashWallet>
                    {
                        new CashWallet
                        {
                            Cash = clientDto.CashAmount
                        }
                    }
                };

                await _context.Clients.AddAsync(client);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Client created successfully: {ClientId}", client.Id);
                return client.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating client: {ClientName}", clientDto.Name);
                throw;
            }
        }

        public async Task UpdateClientAsync(FullClientDto clientDto)
        {
            _logger.LogInformation("Updating client: {ClientId}", clientDto.Id);

            try
            {
                var client = await _context.Clients
                    .Include(x => x.TrainingWallets)
                    .Include(x => x.CashWallets)
                    .FirstOrDefaultAsync(x => x.Id == clientDto.Id);

                if (client == null)
                {
                    throw new Exception($"Client with ID {clientDto.Id} not found");
                }

                // Update basic client info
                client.Name = clientDto.Name;
                client.Birthday = clientDto.Birthday;
                client.Comment = clientDto.Comment;

                // Update training wallet
                var trainingWallet = client.TrainingWallets.FirstOrDefault();
                if (trainingWallet != null)
                {
                    // Check if wallet changed and create history
                    if (trainingWallet.Subscription != clientDto.Subscription ||
                        trainingWallet.Count != clientDto.TrainingCount ||
                        trainingWallet.Skip != clientDto.TrainingSkip ||
                        trainingWallet.Free != clientDto.TrainingFree)
                    {
                        client.TrainingWalletHistory ??= new List<TrainingWalletHistory>();
                        client.TrainingWalletHistory.Add(CreateTrainingWalletHistory(trainingWallet));
                    }

                    trainingWallet.Subscription = clientDto.Subscription;
                    trainingWallet.Count = clientDto.TrainingCount;
                    trainingWallet.Skip = clientDto.TrainingSkip;
                    trainingWallet.Free = clientDto.TrainingFree;
                }

                // Update cash wallet
                var cashWallet = client.CashWallets.FirstOrDefault();
                if (cashWallet != null)
                {
                    // Check if wallet changed and create history
                    if (cashWallet.Cash != clientDto.CashAmount)
                    {
                        client.CashWalletHistory ??= new List<CashWalletHistory>();
                        client.CashWalletHistory.Add(CreateCashWalletHistory(cashWallet));
                    }

                    cashWallet.Cash = clientDto.CashAmount;
                }

                _context.Clients.Update(client);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Client updated successfully: {ClientId}", clientDto.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating client: {ClientId}", clientDto.Id);
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
