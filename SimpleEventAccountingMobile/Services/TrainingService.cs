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
                .Where(t => !t.Deleted)
                .Include(t => t.TrainingWalletHistory)
                .ToListAsync();
        }

        public async Task<Training?> GetTrainingByIdAsync(Guid trainingId)
        {
            return await _dbContext.Trainings
                    .Include(t => t.TrainingWalletHistory)
                    .ThenInclude(th => th.Client)
                .FirstOrDefaultAsync(t => t.Id == trainingId && !t.Deleted);
        }

        public async Task<List<Client>> GetClientsAsync()
        {
            return await _dbContext.Clients
                .Where(c => !c.Deleted)
                .ToListAsync();
        }

        public async Task<List<TrainingDebtClient>> GetTrainingDebtClientsAsync()
        {
            return await _dbContext.TrainingWallets
                .Where(tw => tw.Count < 0 && !tw.Deleted && tw.Client != null && !tw.Client.Deleted)
                .Include(tw => tw.Client)
                .Select(tw => new TrainingDebtClient(tw.Client, tw.Count))
                .Distinct()
                .ToListAsync();
        }

        public async Task<List<CashDebtClient>> GetCashDebtClientsAsync()
        {
            return await _dbContext.CashWallets
                .Where(tw => tw.Cash < 0 && !tw.Deleted && tw.Client != null && !tw.Client.Deleted)
                .Include(tw => tw.Client)
                .Select(tw => new CashDebtClient(tw.Client, tw.Cash))
                .Distinct()
                .ToListAsync() ?? new List<CashDebtClient>();
        }

        public async Task<List<Client>> GetSubscribedClientsAsync()
        {
            return await _dbContext.TrainingWallets
                .Where(tw => tw.Subscription && !tw.Deleted)
                .Include(tw => tw.Client)
                .Select(tw => tw.Client)
                .Where(c => c != null && !c.Deleted)
                .Distinct()
                .ToListAsync() ?? new List<Client>();
        }

        public async Task ConductTrainingAsync(Training training, List<Guid> clientIds)
        {
            await using var transaction = await _dbContext.GetDatabase().BeginTransactionAsync();

            try
            {
                // Создаем новую тренировку
                training.Deleted = false;
                _dbContext.Trainings.Add(training);

                var clientWithSubscription = await _dbContext.TrainingWallets
                    .Where(x => x.Subscription).Select(x=>x.ClientId).ToListAsync();

                var wallets = await _dbContext.TrainingWallets
                    .Where(x => clientIds.Contains(x.ClientId) || clientWithSubscription.Contains(x.ClientId)).ToListAsync();

                var notIn = clientWithSubscription.Where(x => !clientIds.Contains(x));

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
                foreach (var clientId in notIn)
                {
                    var wallet = wallets.First(x => x.ClientId == clientId);

                    if (wallet.Skip > 0)
                    {
                        wallet.Skip--;
                    }
                    else
                    {
                        wallet.Count--;
                    }
                }
                
                foreach (var clientId in clientIds)
                {
                    var isSubscription = clientWithSubscription.Any(x => x == clientId);
                    var wallet = wallets.First(x => x.ClientId == clientId);

                    if (isSubscription)
                    {
                        if (wallet.Skip > 0)
                        {
                            wallet.Skip--;
                        }
                        else
                        {
                            wallet.Count--;
                        }
                    }
                    else
                    {
                        if (wallet.Free > 0)
                        {
                            wallet.Free--;
                        }
                        else
                        {
                            wallet.Count--;
                        }
                    }

                    if (wallet.Count == 0)
                    {
                        wallet.Subscription = false;
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

                    _dbContext.TrainingWalletHistory.Add(history);
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
