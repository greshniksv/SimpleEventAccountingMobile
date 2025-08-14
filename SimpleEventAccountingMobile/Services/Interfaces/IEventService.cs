using SimpleEventAccountingMobile.Database.DbModels;

namespace SimpleEventAccountingMobile.Services.Interfaces
{
    public interface IEventService
    {
        Task<List<Event>> GetPastEventsAsync();

        Task<Event> GetEventByIdAsync(Guid eventId);

        Task CreateEventAsync(Event newEvent, List<Guid> clientIds);
    }
}
