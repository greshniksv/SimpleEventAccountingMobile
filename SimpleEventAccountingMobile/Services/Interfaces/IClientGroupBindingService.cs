using SimpleEventAccountingMobile.Database.DbModels;
using SimpleEventAccountingMobile.Dtos;

namespace SimpleEventAccountingMobile.Services.Interfaces
{
    public interface IClientGroupBindingService
    {
        Task<List<ClientGroupBinding>> GetAllAsync();
        Task<ClientGroup?> GetGroupAsync(Guid id);
        Task<List<ClientGroup>> GetGroupsAsync();
        Task CreateAsync(ClientGroupDto dto);
        Task UpdateAsync(ClientGroupDto dto);
        Task AddClientToGroupAsync(Guid clientId, Guid groupId);
        Task DeleteAsync(Guid id);
    }
}
