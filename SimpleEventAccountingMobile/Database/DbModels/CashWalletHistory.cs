using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SimpleEventAccountingMobile.Database.DbModels
{
	public class CashWalletHistory : BaseEntity
	{
		public Guid ClientId { get; set; }

		public Client? Client { get; set; }

		public DateTime Date { get; set; }

		public Guid? EventId { get; set; }

		public Event? Event { get; set; }

		public decimal Cash { get; set; }

		[MaxLength(1000)]
		public string? Comment { get; set; }

        public static void Configure(ModelBuilder builder)
            => Configure(builder.Entity<CashWalletHistory>());

        private static void Configure(EntityTypeBuilder<CashWalletHistory> builder)
        {
            builder
                .HasKey(x => x.Id);

            builder
                .Property(p => p.Id)
                .ValueGeneratedOnAdd();
        }
    }
}
