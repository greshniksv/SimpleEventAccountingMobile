using SimpleEventAccountingMobile.Dtos;

namespace SimpleEventAccountingMobile.Services.Interfaces
{
    public interface IEventCreationHandler
    {
        Task<(bool Success, string? ErrorMessage)> CreateEventAsync(NewEventModel model, List<Guid> clientIds);
    }
}
