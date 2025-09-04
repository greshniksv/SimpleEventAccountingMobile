using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SimpleEventAccountingMobile.Database.DbContexts;
using SimpleEventAccountingMobile.Database.DbModels;
using SimpleEventAccountingMobile.Services.Interfaces;

namespace SimpleEventAccountingMobile.Services
{
    public class EventService : IEventService
    {
        private readonly MainContext _context;
        private readonly ILogger<EventService> _logger;

        public EventService(MainContext context, ILogger<EventService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<Event>> GetPastEventsAsync()
        {
            _logger.LogInformation("Getting past events");

            try
            {
                var events = await _context.ActionEvents
                    .Where(e => e.Date < DateTime.Now && e.DeletedAt == null)
                    .Include(e => e.EventClients)!
                        .ThenInclude(ec => ec.Client)
                    .OrderByDescending(x => x.Date)
                    .ToListAsync();

                _logger.LogInformation("Retrieved {EventCount} past events successfully", events.Count);
                return events;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting past events");
                throw;
            }
        }

        public async Task<Event> GetEventByIdAsync(Guid eventId)
        {
            _logger.LogInformation("Getting event by ID: {EventId}", eventId);

            try
            {
                var ev = await _context.ActionEvents
                    .Where(e => e.Id == eventId && e.DeletedAt == null)
                    .Include(e => e.EventClients)!
                        .ThenInclude(ec => ec.Client)
                    .FirstOrDefaultAsync();

                if (ev == null)
                {
                    _logger.LogWarning("Event with ID {EventId} not found", eventId);
                    throw new Exception("Событие не найдено.");
                }

                _logger.LogInformation("Event with ID {EventId} retrieved successfully", eventId);
                return ev;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting event by ID: {EventId}", eventId);
                throw;
            }
        }

        public async Task DeleteEventAsync(Guid eventId)
        {
            _logger.LogInformation("Starting deletion of event with ID: {EventId}", eventId);

            try
            {
                var eventDetails = await GetEventByIdAsync(eventId);
                var changeSets = await _context.EventChangeSets
                    .Where(ec => ec.EventId == eventId)
                    .Include(ec => ec.Client)
                    .Include(x => x.Event)
                    .ToListAsync();

                _logger.LogInformation("Found {ChangeSetCount} change sets for event {EventId}", changeSets.Count, eventId);

                using var transaction = await _context.Database.BeginTransactionAsync();

                // Revert changes for each participant
                foreach (var changeSet in changeSets)
                {
                    _logger.LogDebug("Processing change set for client {ClientId} in event {EventId}",
                        changeSet.ClientId, eventId);

                    // Find the cash wallet
                    var cashWallet = await _context.CashWallets
                        .FirstOrDefaultAsync(cw => cw.ClientId == changeSet.ClientId && cw.DeletedAt == null);

                    if (cashWallet != null)
                    {
                        // Refund the money
                        var refundAmount = Math.Abs(changeSet.Cash);
                        cashWallet.Cash += refundAmount;
                        _context.CashWallets.Update(cashWallet);

                        _logger.LogInformation("Refunded {RefundAmount} to client {ClientId} for event {EventId}",
                            refundAmount, changeSet.ClientId, eventId);

                        // Record the refund transaction
                        var history = new CashWalletHistory
                        {
                            ClientId = changeSet.ClientId,
                            Date = DateTime.Now,
                            EventId = eventId,
                            Cash = refundAmount,
                            Comment = $"Возврат средств за удаленное событие"
                        };
                        await _context.CashWalletHistory.AddAsync(history);
                    }
                    else
                    {
                        _logger.LogWarning("Cash wallet not found for client {ClientId} during event deletion",
                            changeSet.ClientId);
                    }
                }

                // Mark event as deleted (soft delete)
                eventDetails.DeletedAt = DateTime.Now;
                _context.ActionEvents.Update(eventDetails);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Event {EventId} deleted successfully", eventId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting event {EventId}", eventId);
                throw;
            }
        }

        public async Task CreateEventAsync(Event newEvent, List<Guid> clientIds)
        {
            _logger.LogInformation("Creating new event '{EventName}' for {ClientCount} clients",
                newEvent.Name, clientIds.Count);

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Add the event
                _context.ActionEvents.Add(newEvent);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Event '{EventName}' with ID {EventId} created successfully",
                    newEvent.Name, newEvent.Id);

                // Add event participants
                foreach (var clientId in clientIds)
                {
                    _logger.LogDebug("Processing client {ClientId} for event {EventId}", clientId, newEvent.Id);

                    var cashWallet = await _context.CashWallets
                        .FirstOrDefaultAsync(cw => cw.ClientId == clientId && cw.DeletedAt == null);

                    if (cashWallet == null)
                    {
                        _logger.LogError("CashWallet for client {ClientId} not found", clientId);
                        throw new Exception($"CashWallet для клиента {clientId} не найден.");
                    }

                    // Deduct event price from client's wallet
                    cashWallet.Cash -= newEvent.Price;
                    _context.CashWallets.Update(cashWallet);

                    _logger.LogInformation("Deducted {Price} from client {ClientId} for event {EventId}",
                        newEvent.Price, clientId, newEvent.Id);

                    var changeSet = new EventChangeSet()
                    {
                        ClientId = clientId,
                        EventId = newEvent.Id,
                        Cash = -newEvent.Price
                    };
                    await _context.EventChangeSets.AddAsync(changeSet);

                    // Record transaction history
                    var history = new CashWalletHistory
                    {
                        ClientId = clientId,
                        Date = DateTime.Now,
                        EventId = newEvent.Id,
                        Cash = -newEvent.Price,
                        Comment = $"Оплата события: {newEvent.Name}"
                    };
                    await _context.CashWalletHistory.AddAsync(history);

                    // Add event-client relationship
                    var eventClient = new EventClient
                    {
                        EventId = newEvent.Id,
                        ClientId = clientId
                    };
                    await _context.EventClients.AddAsync(eventClient);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Event '{EventName}' with ID {EventId} created successfully for {ClientCount} clients",
                    newEvent.Name, newEvent.Id, clientIds.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating event '{EventName}'", newEvent.Name);
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
