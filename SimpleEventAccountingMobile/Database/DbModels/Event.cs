using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SimpleEventAccountingMobile.Database.DbModels
{
	public class Event : BaseEntity
	{
		[MaxLength(100)]
		public string? Name { get; set; }

		[MaxLength(1000)]
		public string? Description { get; set; }

		public DateTime Date { get; set; }

		public decimal Price { get; set; }

		public bool Deleted { get; set; }

		public ICollection<CashWalletHistory>? CashWalletHistory { get; set; }

		public ICollection<EventClient>? EventClients { get; set; }

        public static void Configure(ModelBuilder builder)
            => Configure(builder.Entity<Event>());

        private static void Configure(EntityTypeBuilder<Event> builder)
        {
            builder
                .HasKey(x => x.Id);

            builder
                .Property(p => p.Id)
                .ValueGeneratedOnAdd();

            builder
                .HasMany(e => e.CashWalletHistory)
                .WithOne(e => e.Event)
                .HasForeignKey(e => e.EventId)
                .OnDelete(DeleteBehavior.NoAction);

            builder
                .HasMany(e => e.EventClients)
                .WithOne(e => e.Event)
                .HasForeignKey(e => e.EventId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
