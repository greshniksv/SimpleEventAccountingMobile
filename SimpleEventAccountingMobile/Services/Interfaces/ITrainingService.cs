using SimpleEventAccountingMobile.Database.DbModels;
using SimpleEventAccountingMobile.Dtos;

namespace SimpleEventAccountingMobile.Services.Interfaces
{
    public interface ITrainingService
    {
        Task<List<Training>> GetTrainingsAsync(int skip, int take);

        Task<Training?> GetTrainingByIdAsync(Guid trainingId);

        Task<List<Client>> GetClientsAsync();

        Task<List<Client>> GetClientsByGroupsAsync(List<Guid> groupList);

        Task<List<Client>> GetSubscribedClientsAsync();

        Task<List<Client>> GetSubscribedClientsByGroupsAsync(List<Guid> groupList);

        Task ConductTrainingAsync(Training training, List<Guid> clientIds, List<Guid> clientIdWithSubAbsent);

        Task<bool> DeleteTrainingAsync(Guid trainingId);

        Task<List<TrainingDebtClient>> GetTrainingDebtClientsAsync();

        Task<List<CashDebtClient>> GetCashDebtClientsAsync();

        Task FixTrainingClientsAsync();
    }
}
