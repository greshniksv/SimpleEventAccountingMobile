using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SimpleEventAccountingMobile.Database.DbModels
{
	public class EventClient : BaseEntity
	{
		public Guid EventId { get; set; }

		public Event? Event { get; set; }

		public Guid ClientId { get; set; }

		public Client? Client { get; set; }

        public static void Configure(ModelBuilder builder)
            => Configure(builder.Entity<EventClient>());

        private static void Configure(EntityTypeBuilder<EventClient> builder)
        {
            builder
                .HasKey(x => x.Id);

            builder
                .Property(p => p.Id)
                .ValueGeneratedOnAdd();
        }
    }
}
