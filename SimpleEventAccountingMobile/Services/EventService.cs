using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SimpleEventAccountingMobile.Database.DbContexts;
using SimpleEventAccountingMobile.Database.DbModels;
using SimpleEventAccountingMobile.Services.Interfaces;

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
                .Where(e => e.Date < DateTime.Now && !e.Deleted)
                .Include(e => e.EventClients)
                    .ThenInclude(ec => ec.Client)
                .ToListAsync();
        }

        public async Task<Event> GetEventByIdAsync(Guid eventId)
        {
            var ev = await _context.ActionEvents
                .Where(e => e.Id == eventId && !e.Deleted)
                .Include(e => e.EventClients)
                    .ThenInclude(ec => ec.Client)
                .FirstOrDefaultAsync();

            if (ev == null)
                throw new Exception("Событие не найдено.");

            return ev;
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
                        .FirstOrDefaultAsync(cw => cw.ClientId == clientId && !cw.Deleted);

                    if (cashWallet == null)
                        throw new Exception($"CashWallet для клиента {clientId} не найден.");

                    //if (cashWallet.Cash < newEvent.Price)
                    //    throw new Exception($"У клиента {clientId} недостаточно средств.");

                    // Вычитаем стоимость события из кошелька клиента
                    cashWallet.Cash -= newEvent.Price;
                    _context.CashWallets.Update(cashWallet);

                    // Записываем историю транзакции
                    var history = new CashWalletHistory
                    {
                        ClientId = clientId,
                        Date = DateTime.Now,
                        EventId = newEvent.Id,
                        Cash = -newEvent.Price,
                        Comment = $"Оплата события: {newEvent.Name}"
                    };
                    _context.CashWalletHistory.Add(history);

                    // Добавляем связь событие-клиент
                    var eventClient = new EventClient
                    {
                        EventId = newEvent.Id,
                        ClientId = clientId
                    };
                    _context.EventClients.Add(eventClient);
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
