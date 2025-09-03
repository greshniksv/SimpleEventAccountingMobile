using Microsoft.EntityFrameworkCore;
using SimpleEventAccountingMobile.Database.DbContexts;
using SimpleEventAccountingMobile.Database.DbModels;
using SimpleEventAccountingMobile.Services.Interfaces;

namespace SimpleEventAccountingMobile.Services
{
    public class ClientService : IClientService
    {
        private readonly MainContext _context;

        public ClientService(MainContext context)
        {
            _context = context;
        }

        public async Task<List<Client>> GetAllClientsAsync()
        {
            return await _context.Clients
                .AsNoTracking()
                .Where(c => c.DeletedAt == null)
                .ToListAsync();
        }

        public async Task<List<Client>> GetDeletedAsync()
        {
            return await _context.Clients
                .AsNoTracking()
                .Where(c => c.DeletedAt != null)
                .ToListAsync();
        }

        public async Task RestoreDeletedAsync(Guid clientId)
        {
            var client = await _context.Clients.FirstOrDefaultAsync(x => x.Id == clientId);
            if (client == null)
            {
                throw new Exception("Client not found exception");
            }

            client.DeletedAt = null;

            _context.Clients.Update(client);
            await _context.SaveChangesAsync();
        }
    }
}
