using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SimpleEventAccountingMobile.Database.DbModels
{
    public class EventChangeSet : BaseEntity
    {
        public Guid ClientId { get; set; }

        public Client? Client { get; set; }

        public Guid EventId { get; set; }

        public Event? Event { get; set; }

        public decimal Cash { get; set; }

        public static void Configure(ModelBuilder builder)
            => Configure(builder.Entity<EventChangeSet>());

        private static void Configure(EntityTypeBuilder<EventChangeSet> builder)
        {
            builder
                .HasKey(x => x.Id);

            builder
                .Property(p => p.Id)
                .ValueGeneratedOnAdd();
        }
    }
}
