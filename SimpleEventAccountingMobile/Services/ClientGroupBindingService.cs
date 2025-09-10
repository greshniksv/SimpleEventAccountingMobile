using Microsoft.EntityFrameworkCore;
using SimpleEventAccountingMobile.Database.DbModels;
using SimpleEventAccountingMobile.Database.Interfaces;
using SimpleEventAccountingMobile.Dtos;
using SimpleEventAccountingMobile.Services.Interfaces;

namespace SimpleEventAccountingMobile.Services
{
    public class ClientGroupBindingService(IMainContext context) : IClientGroupBindingService
    {
        private readonly IMainContext _context = context ?? throw new ArgumentNullException(nameof(context));

        public async Task<List<ClientGroupBinding>> GetAllAsync()
        {
            var items =
                await _context.ClientGroupBindings
                    .Include(x => x.ClientGroup)
                    .Include(x => x.Client)
                    .ToListAsync();

            var clientGroups = await (from cg in context.ClientGroups
                                      join cgb in context.ClientGroupBindings on cg.Id equals cgb.ClientGroupId into gj
                                      from sub in gj.DefaultIfEmpty()
                                      where sub == null // No related ClientGroupBinding found
                                      select cg).ToListAsync();

            clientGroups.ForEach(x => items.Add(new ClientGroupBinding() { ClientGroup = x, ClientGroupId = x.Id }));

            return items;
        }

        public async Task<ClientGroup?> GetGroupAsync(Guid id)
        {
            var item = await _context.ClientGroups
                .Include(x=>x.ClientGroupBindings)
                .FirstOrDefaultAsync(x => x.Id == id);
            return item;
        }

        public async Task<List<ClientGroup>> GetGroupsAsync()
        {
            var items = await _context.ClientGroups
                .Include(x => x.ClientGroupBindings)
                .ToListAsync();
            return items;
        }

        public async Task CreateAsync(ClientGroupDto dto)
        {
            var clientGroup = new ClientGroup
            {
                Id = dto.Id,
                Name = dto.Name,
                Description = dto.Description,
                ClientGroupBindings = dto.Clients.Select(clientId => new ClientGroupBinding
                {
                    ClientId = clientId
                }).ToList()
            };

            _context.ClientGroups.Add(clientGroup);
            await _context.SaveChangesAsync();
        }

        public async Task AddClientToGroupAsync(Guid clientId, Guid groupId)
        {
            await _context.ClientGroupBindings.AddAsync(new ClientGroupBinding()
            {
                ClientId = clientId,
                ClientGroupId = groupId
            });

            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ClientGroupDto dto)
        {
            var existingClientGroup = await _context.ClientGroups
                .Include(cg => cg.ClientGroupBindings)
                .FirstOrDefaultAsync(cg => cg.Id == dto.Id);

            if (existingClientGroup == null)
            {
                throw new Exception($"Client group with ID {dto.Id} not found.");
            }

            // Update base data
            existingClientGroup.Name = dto.Name;
            existingClientGroup.Description = dto.Description;

            // Remove existing bindings and add new ones
            _context.ClientGroupBindings.RemoveRange(existingClientGroup.ClientGroupBindings);
            existingClientGroup.ClientGroupBindings = dto.Clients.Select(clientId => new ClientGroupBinding
            {
                ClientId = clientId,
                ClientGroup = existingClientGroup
            }).ToList();

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var clientGroup = await _context.ClientGroups
                .Include(cg => cg.ClientGroupBindings)
                .FirstOrDefaultAsync(cg => cg.Id == id);

            if (clientGroup != null)
            {
                _context.ClientGroupBindings.RemoveRange(clientGroup.ClientGroupBindings);
                _context.ClientGroups.Remove(clientGroup);
                await _context.SaveChangesAsync();
            }
        }
    }
}
