
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SimpleEventAccountingMobile.Database.DbModels
{
	public class CashWallet : BaseEntity
	{
		public Guid ClientId { get; set; }

		public Client? Client { get; set; }

		public decimal Cash { get; set; }

		public bool Deleted { get; set; }

        public static void Configure(ModelBuilder builder)
            => Configure(builder.Entity<CashWallet>());

        private static void Configure(EntityTypeBuilder<CashWallet> builder)
        {
            builder
                .HasKey(x => x.Id);

            builder
                .Property(p => p.Id)
                .ValueGeneratedOnAdd();
        }
    }
}
