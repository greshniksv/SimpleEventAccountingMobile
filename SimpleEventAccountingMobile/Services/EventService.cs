using Microsoft.EntityFrameworkCore;
using SimpleEventAccountingMobile.Database.DbContexts;
using SimpleEventAccountingMobile.Database.DbModels;
using SimpleEventAccountingMobile.Services.Interfaces;
using System.Diagnostics;

namespace SimpleEventAccountingMobile.Services
{
    public class EventService : IEventService
    {
        private readonly MainContext _context;

        public EventService(MainContext context)
        {
            _context = context;
        }

        public async Task<List<Event>> GetPastEventsAsync()
        {
            return await _context.ActionEvents
                .Where(e => e.Date < DateTime.Now && e.DeletedAt == null)
                .Include(e => e.EventClients)!
                    .ThenInclude(ec => ec.Client)
                .OrderByDescending(x=>x.Date)
                .ToListAsync();
        }

        public async Task<Event> GetEventByIdAsync(Guid eventId)
        {
            var ev = await _context.ActionEvents
                .Where(e => e.Id == eventId && e.DeletedAt == null)
                .Include(e => e.EventClients)!
                    .ThenInclude(ec => ec.Client)
                .FirstOrDefaultAsync();

            if (ev == null)
                throw new Exception("Событие не найдено.");

            return ev;
        }

        public async Task DeleteEventAsync(Guid eventId)
        {
            var eventDetails = await GetEventByIdAsync(eventId);
            var changeSets = await _context.EventChangeSets
                .Where(ec => ec.EventId == eventId)
                .Include(ec => ec.Client)
                .Include(x=>x.Event)
                .ToListAsync();

            try
            {
                using var transaction = await _context.BeginTransactionAsync();

                // Revert changes for each participant
                foreach (var changeSet in changeSets)
                {
                    // Find the cash wallet
                    var cashWallet = await _context.CashWallets
                        .FirstOrDefaultAsync(cw => cw.ClientId == changeSet.ClientId && cw.DeletedAt == null);

                    if (cashWallet != null)
                    {
                        // Refund the money
                        cashWallet.Cash += Math.Abs(changeSet.Cash);
                        _context.CashWallets.Update(cashWallet);

                        // Record the refund transaction
                        var history = new CashWalletHistory
                        {
                            ClientId = changeSet.ClientId,
                            Date = DateTime.Now,
                            EventId = eventId,
                            Cash = Math.Abs(changeSet.Cash),
                            Comment = $"Возврат средств за удаленное событие"
                        };
                        await _context.CashWalletHistory.AddAsync(history);
                    }
                }

                // Mark event as deleted (soft delete)
                eventDetails.DeletedAt = DateTime.Now;
                _context.ActionEvents.Update(eventDetails);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

            }
            catch (Exception ex)
            {
                // Handle error
                Debug.WriteLine($"Error deleting event: {ex.Message}");
            }
        }

        public async Task CreateEventAsync(Event newEvent, List<Guid> clientIds)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Добавляем событие
                _context.ActionEvents.Add(newEvent);
                await _context.SaveChangesAsync();

                // Добавляем участников события
                foreach (var clientId in clientIds)
                {
                    var cashWallet = await _context.CashWallets
                        .FirstOrDefaultAsync(cw => cw.ClientId == clientId && cw.DeletedAt == null);

                    if (cashWallet == null)
                        throw new Exception($"CashWallet для клиента {clientId} не найден.");

                    // Вычитаем стоимость события из кошелька клиента
                    cashWallet.Cash -= newEvent.Price;
                    _context.CashWallets.Update(cashWallet);

                    var changeSet = new EventChangeSet()
                    {
                        ClientId = clientId,
                        EventId = newEvent.Id,
                        Cash = -newEvent.Price
                    };
                    await _context.EventChangeSets.AddAsync(changeSet);

                    // Записываем историю транзакции
                    var history = new CashWalletHistory
                    {
                        ClientId = clientId,
                        Date = DateTime.Now,
                        EventId = newEvent.Id,
                        Cash = -newEvent.Price,
                        Comment = $"Оплата события: {newEvent.Name}"
                    };
                    await _context.CashWalletHistory.AddAsync(history);

                    // Добавляем связь событие-клиент
                    var eventClient = new EventClient
                    {
                        EventId = newEvent.Id,
                        ClientId = clientId
                    };
                    await _context.EventClients.AddAsync(eventClient);
                }

                await _context.SaveChangesAsync();
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
