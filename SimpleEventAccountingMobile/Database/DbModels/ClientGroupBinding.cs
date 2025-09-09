using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SimpleEventAccountingMobile.Database.DbModels
{
    public class ClientGroupBinding : BaseEntity
    {
        public Guid ClientId { get; set; }

        public Client? Client { get; set; }

        public Guid ClientGroupId { get; set; }

        public ClientGroup? ClientGroup { get; set; }

        public static void Configure(ModelBuilder builder)
            => Configure(builder.Entity<ClientGroupBinding>());

        private static void Configure(EntityTypeBuilder<ClientGroupBinding> builder)
        {
            builder
                .HasKey(x => x.Id);

            builder
                .Property(p => p.Id)
                .ValueGeneratedOnAdd();
        }
    }
}
