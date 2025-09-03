using Microsoft.EntityFrameworkCore;
using SimpleEventAccountingMobile.Database.DbModels;
using SimpleEventAccountingMobile.Database.Interfaces;
using SimpleEventAccountingMobile.Dtos;
using SimpleEventAccountingMobile.Services.Interfaces;

namespace SimpleEventAccountingMobile.Services
{
    public class TrainingService : ITrainingService
    {
        private readonly IMainContext _dbContext;

        public TrainingService(IMainContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Training>> GetTrainingsAsync()
        {
            return await _dbContext.Trainings
                .Where(t => t.DeletedAt == null)
                .Include(t => t.TrainingWalletHistory)
                .OrderByDescending(x=>x.Date)
                .ToListAsync();
        }

        public async Task<Training?> GetTrainingByIdAsync(Guid trainingId)
        {
            return await _dbContext.Trainings
                    .Include(t => t.TrainingWalletHistory)
                    .ThenInclude(th => th.Client)
                .FirstOrDefaultAsync(t => t.Id == trainingId && t.DeletedAt == null);
        }

        public async Task<List<Client>> GetClientsAsync()
        {
            return await _dbContext.Clients
                .Where(c => c.DeletedAt == null)
                .ToListAsync();
        }

        public async Task<List<TrainingDebtClient>> GetTrainingDebtClientsAsync()
        {
            return await _dbContext.TrainingWallets
                .Where(tw => tw.Count < 0 && tw.DeletedAt == null && tw.Client != null && tw.Client.DeletedAt == null)
                .Include(tw => tw.Client)
                .Select(tw => new TrainingDebtClient(tw.Client, tw.Count))
                .Distinct()
                .ToListAsync();
        }

        public async Task<List<CashDebtClient>> GetCashDebtClientsAsync()
        {
            return await _dbContext.CashWallets
                .Where(tw => tw.Cash < 0 && tw.DeletedAt == null && tw.Client != null && tw.Client.DeletedAt == null)
                .Include(tw => tw.Client)
                .Select(tw => new CashDebtClient(tw.Client, tw.Cash))
                .Distinct()
                .ToListAsync() ?? new List<CashDebtClient>();
        }

        public async Task<List<Client>> GetSubscribedClientsAsync()
        {
            return await _dbContext.TrainingWallets
                .Where(tw => tw.Subscription && tw.DeletedAt == null)
                .Include(tw => tw.Client)
                .Select(tw => tw.Client)
                .Where(c => c != null && c.DeletedAt == null)
                .Distinct()
                .ToListAsync() ?? new List<Client>();
        }

        public async Task<bool> DeleteTrainingAsync(Guid trainingId)
        {
            await using var transaction = await _dbContext.GetDatabase().BeginTransactionAsync();

            try
            {
                var training = await _dbContext.Trainings
                    .FirstOrDefaultAsync(t => t.Id == trainingId && t.DeletedAt == null);

                if (training == null) return false;

                var changeSets = await _dbContext.TrainingChangeSets
                    .Where(cs => cs.TrainingId == trainingId)
                    .ToListAsync();

                foreach (var changeSet in changeSets)
                {
                    var wallet = await _dbContext.TrainingWallets
                        .FirstOrDefaultAsync(w => w.ClientId == changeSet.ClientId && w.DeletedAt == null);

                    if (wallet != null)
                    {
                        // Apply reversed changes
                        if (changeSet.Count != null)
                        {
                            wallet.Count += -changeSet.Count.Value;
                        }

                        if (changeSet.Skip != null)
                        {
                            wallet.Skip += -changeSet.Skip.Value;
                        }

                        if (changeSet.Free != null)
                        {
                            wallet.Free += -changeSet.Free.Value;
                        }

                        if (changeSet.Subscription is not null && changeSet.Count > 0)
                        {
                            wallet.Subscription = true;
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
                    }
                }

                training.DeletedAt = DateTime.UtcNow;
                _dbContext.Trainings.Update(training);

                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        /// <summary>
        /// Провести тренировку
        /// </summary>
        /// <param name="training">Тренировка</param>
        /// <param name="clientIds">Все кто пришел</param>
        /// <param name="clientIdWithSubAbsent">Клиенты с подпиской которые не пришли</param>
        /// <returns></returns>
        public async Task ConductTrainingAsync(Training training, List<Guid> clientIds, List<Guid> clientIdWithSubAbsent)
        {
            await using var transaction = await _dbContext.GetDatabase().BeginTransactionAsync();

            try
            {
                // Создаем новую тренировку
                training.DeletedAt = null;
                _dbContext.Trainings.Add(training);

                var clientWithSubscription = await _dbContext.TrainingWallets
                    .Where(x => x.Subscription).Select(x => x.ClientId).ToListAsync();

                var wallets = await _dbContext.TrainingWallets
                    .Where(x => clientIds.Contains(x.ClientId) || clientWithSubscription.Contains(x.ClientId)).ToListAsync();

                /*
                 * Если есть подписка
                 *   Если есть клиент присутствует - списываем Count
                 *   Если отсутствует - списываем Skip, если есть, а если нет списываем Count
                 * Если подписки нет
                 *   Если есть Free - списываем
                 *   Если есть Skip - списываем
                 *   Если есть Count - списываем
                 */

                // Списываем из кошелька и добавляем записи в TrainingWalletHistory

                // Клиент с подпиской и НЕ присутствует
                foreach (var clientId in clientIdWithSubAbsent)
                {
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
                    }
                    else
                    {
                        wallet.Count--;
                        changeSet.Count = -1;
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
                }

                // Клиент и присутствует
                foreach (var clientId in clientIds)
                {
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
                    }
                    else
                    {
                        if (wallet.Free > 0)
                        {
                            wallet.Free--;
                            changeSet.Free = -1;
                        }
                        else
                        {
                            wallet.Count--;
                            changeSet.Count = -1;
                        }
                    }

                    if (wallet is { Count: <= 0, Subscription: true })
                    {
                        wallet.Subscription = false;
                        changeSet.Subscription = false;
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
                }

                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
