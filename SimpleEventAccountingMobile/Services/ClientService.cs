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
                .Where(c => !c.Deleted)
                .ToListAsync();
        }
    }
}
