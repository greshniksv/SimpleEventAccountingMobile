using Microsoft.EntityFrameworkCore;
using SimpleEventAccountingMobile.Database.DbContexts;
using SimpleEventAccountingMobile.Database.DbModels;
using SimpleEventAccountingMobile.Database.Interfaces;
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
            using var transaction = await _dbContext.GetDatabase().BeginTransactionAsync();

            try
            {
                // Создаем новую тренировку
                training.Deleted = false;
                _dbContext.Trainings.Add(training);

                // Добавляем записи в TrainingWalletHistory
                foreach (var clientId in clientIds)
                {
                    var history = new TrainingWalletHistory
                    {
                        ClientId = clientId,
                        TrainingId = training.Id,
                        Date = training.Date,
                        Count = 1, // или другое логика подсчета
                        Skip = 0,  // предполагается изменения, если необходимо
                        Free = 0,  // аналогично
                        Subscription = false, // или другая логика
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
