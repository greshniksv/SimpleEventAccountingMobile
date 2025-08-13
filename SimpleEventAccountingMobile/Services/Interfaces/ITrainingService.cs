using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleEventAccountingMobile.Database.DbModels;

namespace SimpleEventAccountingMobile.Services.Interfaces
{
    public interface ITrainingService
    {
        Task<List<Training>> GetTrainingsAsync();
        Task<Training?> GetTrainingByIdAsync(Guid trainingId);
        Task<List<Client>> GetClientsAsync();
        Task<List<Client>> GetSubscribedClientsAsync();
        Task ConductTrainingAsync(Training training, List<Guid> clientIds);
    }
}
