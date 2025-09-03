using SimpleEventAccountingMobile.Database.DbModels;
using SimpleEventAccountingMobile.Dtos;
using SimpleEventAccountingMobile.Services.Interfaces;

namespace SimpleEventAccountingMobile.Services
{
    public class EventCreationHandler : IEventCreationHandler
    {
        private readonly IEventService _eventService;

        public EventCreationHandler(IEventService eventService)
        {
            _eventService = eventService;
        }

        public async Task<(bool Success, string? ErrorMessage)> CreateEventAsync(NewEventModel model, List<Guid> clientIds)
        {
            try
            {
                if (clientIds == null || !clientIds.Any())
                {
                    return (false, "Необходимо выбрать хотя бы одного клиента для события.");
                }

                var newEvent = new Event
                {
                    Name = model.Name,
                    Description = model.Description,
                    Date = model.Date,
                    Price = model.Price,
                    DeletedAt = null
                };

                await _eventService.CreateEventAsync(newEvent, clientIds);
                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, $"Ошибка при создании события: {ex.Message}");
            }
        }
    }
}
