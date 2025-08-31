using SimpleEventAccountingMobile.Database.DbModels;
using SimpleEventAccountingMobile.Dtos;

namespace SimpleEventAccountingMobile.Services.Interfaces
{
    public interface ITrainingService
    {
        Task<List<Training>> GetTrainingsAsync();

        Task<Training?> GetTrainingByIdAsync(Guid trainingId);

        Task<List<Client>> GetClientsAsync();

        Task<List<Client>> GetSubscribedClientsAsync();

        Task ConductTrainingAsync(Training training, List<Guid> clientIds, List<Guid> clientIdWithSubAbsent);

        Task<List<TrainingDebtClient>> GetTrainingDebtClientsAsync();

        Task<List<CashDebtClient>> GetCashDebtClientsAsync();
    }
}
