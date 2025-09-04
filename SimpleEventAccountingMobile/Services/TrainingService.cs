using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SimpleEventAccountingMobile.Database.DbModels;
using SimpleEventAccountingMobile.Database.Interfaces;
using SimpleEventAccountingMobile.Dtos;
using SimpleEventAccountingMobile.Services.Interfaces;

namespace SimpleEventAccountingMobile.Services
{
    public class TrainingService : ITrainingService
    {
        private readonly IMainContext _dbContext;
        private readonly ILogger<TrainingService> _logger;

        public TrainingService(IMainContext dbContext, ILogger<TrainingService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<List<Training>> GetTrainingsAsync()
        {
            _logger.LogInformation("Getting all trainings");

            try
            {
                var trainings = await _dbContext.Trainings
                    .Where(t => t.DeletedAt == null)
                    .Include(t => t.TrainingWalletHistory)
                    .OrderByDescending(x => x.Date)
                    .ToListAsync();

                _logger.LogInformation("Retrieved {TrainingCount} trainings successfully", trainings.Count);
                return trainings;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting trainings");
                throw;
            }
        }

        public async Task<Training?> GetTrainingByIdAsync(Guid trainingId)
        {
            _logger.LogInformation("Getting training by ID: {TrainingId}", trainingId);

            try
            {
                var training = await _dbContext.Trainings
                    .Include(t => t.TrainingWalletHistory)
                    .ThenInclude(th => th.Client)
                    .FirstOrDefaultAsync(t => t.Id == trainingId && t.DeletedAt == null);

                if (training == null)
                {
                    _logger.LogWarning("Training with ID {TrainingId} not found", trainingId);
                }
                else
                {
                    _logger.LogInformation("Training with ID {TrainingId} retrieved successfully", trainingId);
                }

                return training;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting training by ID: {TrainingId}", trainingId);
                throw;
            }
        }

        public async Task<List<Client>> GetClientsAsync()
        {
            _logger.LogInformation("Getting all clients");

            try
            {
                var clients = await _dbContext.Clients
                    .Where(c => c.DeletedAt == null)
                    .ToListAsync();

                _logger.LogInformation("Retrieved {ClientCount} clients successfully", clients.Count);
                return clients;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting clients");
                throw;
            }
        }

        public async Task<List<TrainingDebtClient>> GetTrainingDebtClientsAsync()
        {
            _logger.LogInformation("Getting training debt clients");

            try
            {
                var debtClients = await _dbContext.TrainingWallets
                    .Where(tw => tw.Count < 0 && tw.DeletedAt == null && tw.Client != null && tw.Client.DeletedAt == null)
                    .Include(tw => tw.Client)
                    .Select(tw => new TrainingDebtClient(tw.Client, tw.Count))
                    .Distinct()
                    .ToListAsync();

                _logger.LogInformation("Found {DebtClientCount} clients with training debt", debtClients.Count);
                return debtClients;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting training debt clients");
                throw;
            }
        }

        public async Task<List<CashDebtClient>> GetCashDebtClientsAsync()
        {
            _logger.LogInformation("Getting cash debt clients");

            try
            {
                var debtClients = await _dbContext.CashWallets
                    .Where(tw => tw.Cash < 0 && tw.DeletedAt == null && tw.Client != null && tw.Client.DeletedAt == null)
                    .Include(tw => tw.Client)
                    .Select(tw => new CashDebtClient(tw.Client, tw.Cash))
                    .Distinct()
                    .ToListAsync() ?? new List<CashDebtClient>();

                _logger.LogInformation("Found {DebtClientCount} clients with cash debt", debtClients.Count);
                return debtClients;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting cash debt clients");
                throw;
            }
        }

        public async Task<List<Client>> GetSubscribedClientsAsync()
        {
            _logger.LogInformation("Getting subscribed clients");

            try
            {
                var subscribedClients = await _dbContext.TrainingWallets
                    .Where(tw => tw.Subscription && tw.DeletedAt == null)
                    .Include(tw => tw.Client)
                    .Select(tw => tw.Client)
                    .Where(c => c != null && c.DeletedAt == null)
                    .Distinct()
                    .ToListAsync() ?? new List<Client>();

                _logger.LogInformation("Found {SubscribedClientCount} subscribed clients", subscribedClients.Count);
                return subscribedClients;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting subscribed clients");
                throw;
            }
        }

        public async Task<bool> DeleteTrainingAsync(Guid trainingId)
        {
            _logger.LogInformation("Starting deletion of training with ID: {TrainingId}", trainingId);

            await using var transaction = await _dbContext.GetDatabase().BeginTransactionAsync();

            try
            {
                var training = await _dbContext.Trainings
                    .FirstOrDefaultAsync(t => t.Id == trainingId && t.DeletedAt == null);

                if (training == null)
                {
                    _logger.LogWarning("Training with ID {TrainingId} not found for deletion", trainingId);
                    return false;
                }

                var changeSets = await _dbContext.TrainingChangeSets
                    .Where(cs => cs.TrainingId == trainingId)
                    .ToListAsync();

                _logger.LogInformation("Found {ChangeSetCount} change sets for training {TrainingId}", changeSets.Count, trainingId);

                foreach (var changeSet in changeSets)
                {
                    _logger.LogDebug("Processing change set for client {ClientId} in training {TrainingId}",
                        changeSet.ClientId, trainingId);

                    var wallet = await _dbContext.TrainingWallets
                        .FirstOrDefaultAsync(w => w.ClientId == changeSet.ClientId && w.DeletedAt == null);

                    if (wallet != null)
                    {
                        // Apply reversed changes
                        if (changeSet.Count != null)
                        {
                            wallet.Count += -changeSet.Count.Value;
                            _logger.LogDebug("Reversed count change: {CountChange} for client {ClientId}",
                                -changeSet.Count.Value, changeSet.ClientId);
                        }

                        if (changeSet.Skip != null)
                        {
                            wallet.Skip += -changeSet.Skip.Value;
                            _logger.LogDebug("Reversed skip change: {SkipChange} for client {ClientId}",
                                -changeSet.Skip.Value, changeSet.ClientId);
                        }

                        if (changeSet.Free != null)
                        {
                            wallet.Free += -changeSet.Free.Value;
                            _logger.LogDebug("Reversed free change: {FreeChange} for client {ClientId}",
                                -changeSet.Free.Value, changeSet.ClientId);
                        }

                        if (changeSet.Subscription is not null && changeSet.Count > 0)
                        {
                            wallet.Subscription = true;
                            _logger.LogDebug("Restored subscription for client {ClientId}", changeSet.ClientId);
                        }

                        _dbContext.TrainingWallets.Update(wallet);

                        var history = new TrainingWalletHistory
                        {
                            ClientId = changeSet.ClientId,
                            TrainingId = training.Id,
                            Date = DateTime.Now,
                            Count = wallet.Count,
                            Skip = wallet.Skip,
                            Free = wallet.Free,
                            Subscription = wallet.Subscription,
                            Comment = "Возврат средств за удаление тренировки"
                        };

                        await _dbContext.TrainingWalletHistory.AddAsync(history);
                        _logger.LogInformation("Created refund history for client {ClientId}", changeSet.ClientId);
                    }
                    else
                    {
                        _logger.LogWarning("Wallet not found for client {ClientId} during training deletion",
                            changeSet.ClientId);
                    }
                }

                training.DeletedAt = DateTime.UtcNow;
                _dbContext.Trainings.Update(training);

                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Training {TrainingId} deleted successfully", trainingId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting training {TrainingId}", trainingId);
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task ConductTrainingAsync(Training training, List<Guid> clientIds, List<Guid> clientIdWithSubAbsent)
        {
            _logger.LogInformation("Conducting training '{TrainingName}' with {PresentCount} present clients and {AbsentCount} absent subscribers",
                training.Name, clientIds.Count, clientIdWithSubAbsent.Count);

            await using var transaction = await _dbContext.GetDatabase().BeginTransactionAsync();

            try
            {
                // Create new training
                training.DeletedAt = null;
                _dbContext.Trainings.Add(training);
                _logger.LogInformation("Created new training '{TrainingName}' with ID {TrainingId}",
                    training.Name, training.Id);

                var clientWithSubscription = await _dbContext.TrainingWallets
                    .Where(x => x.Subscription).Select(x => x.ClientId).ToListAsync();

                _logger.LogInformation("Found {SubscriptionCount} clients with subscriptions", clientWithSubscription.Count);

                var wallets = await _dbContext.TrainingWallets
                    .Where(x => clientIds.Contains(x.ClientId) || clientWithSubscription.Contains(x.ClientId)).ToListAsync();

                _logger.LogInformation("Retrieved {WalletCount} wallets for processing", wallets.Count);

                // Process absent subscribers
                foreach (var clientId in clientIdWithSubAbsent)
                {
                    _logger.LogDebug("Processing absent subscriber client {ClientId}", clientId);

                    TrainingChangeSet changeSet = new TrainingChangeSet()
                    {
                        ClientId = clientId,
                        TrainingId = training.Id
                    };

                    var wallet = wallets.First(x => x.ClientId == clientId);

                    if (wallet.Skip > 0)
                    {
                        wallet.Skip--;
                        changeSet.Skip = -1;
                        _logger.LogInformation("Deducted skip from absent subscriber client {ClientId}", clientId);
                    }
                    else
                    {
                        wallet.Count--;
                        changeSet.Count = -1;
                        _logger.LogInformation("Deducted count from absent subscriber client {ClientId}", clientId);
                    }

                    _dbContext.TrainingWallets.Update(wallet);

                    var history = new TrainingWalletHistory
                    {
                        ClientId = clientId,
                        TrainingId = training.Id,
                        Date = training.Date,
                        Count = wallet.Count,
                        Skip = wallet.Skip,
                        Free = wallet.Free,
                        Subscription = wallet.Subscription,
                        Comment = $"Пропуск тренировки {training.Name}"
                    };

                    await _dbContext.TrainingChangeSets.AddAsync(changeSet);
                    await _dbContext.TrainingWalletHistory.AddAsync(history);
                    _logger.LogDebug("Created history record for absent subscriber client {ClientId}", clientId);
                }

                // Process present clients
                foreach (var clientId in clientIds)
                {
                    _logger.LogDebug("Processing present client {ClientId}", clientId);

                    TrainingChangeSet changeSet = new TrainingChangeSet()
                    {
                        ClientId = clientId,
                        TrainingId = training.Id
                    };

                    var isSubscription = clientWithSubscription.Any(x => x == clientId);
                    var wallet = wallets.First(x => x.ClientId == clientId);

                    if (isSubscription)
                    {
                        wallet.Count--;
                        changeSet.Count = -1;
                        _logger.LogInformation("Deducted count from subscribed client {ClientId}", clientId);
                    }
                    else
                    {
                        if (wallet.Free > 0)
                        {
                            wallet.Free--;
                            changeSet.Free = -1;
                            _logger.LogInformation("Deducted free session from client {ClientId}", clientId);
                        }
                        else
                        {
                            wallet.Count--;
                            changeSet.Count = -1;
                            _logger.LogInformation("Deducted count from client {ClientId}", clientId);
                        }
                    }

                    if (wallet is { Count: <= 0, Subscription: true })
                    {
                        wallet.Subscription = false;
                        changeSet.Subscription = false;
                        _logger.LogInformation("Subscription expired for client {ClientId}", clientId);
                    }

                    _dbContext.TrainingWallets.Update(wallet);

                    var history = new TrainingWalletHistory
                    {
                        ClientId = clientId,
                        TrainingId = training.Id,
                        Date = training.Date,
                        Count = wallet.Count,
                        Skip = wallet.Skip,
                        Free = wallet.Free,
                        Subscription = wallet.Subscription,
                        Comment = $"Участие в тренировке {training.Name}"
                    };

                    await _dbContext.TrainingChangeSets.AddAsync(changeSet);
                    await _dbContext.TrainingWalletHistory.AddAsync(history);
                    _logger.LogDebug("Created history record for present client {ClientId}", clientId);
                }

                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Training '{TrainingName}' conducted successfully for {ClientCount} clients",
                    training.Name, clientIds.Count + clientIdWithSubAbsent.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error conducting training '{TrainingName}'", training.Name);
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
