using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations;

namespace SimpleEventAccountingMobile.Database.DbModels
{
	public class Client : BaseEntity
	{
		[MaxLength(100)]
		public string? Name { get; set; }

		public DateTime Birthday { get; set; }

		[MaxLength(500)]
		public string? Comment { get; set; }

		public bool Deleted { get; set; }

		public ICollection<TrainingWallet>? TrainingWallets { get; set; }

		public ICollection<CashWallet>? CashWallets { get; set; }

		public ICollection<TrainingClient>? TrainingClients { get; set; }

		public ICollection<TrainingWalletHistory>? TrainingWalletHistory { get; set; }

		public ICollection<EventClient>? EventClients { get; set; }

        public static void Configure(ModelBuilder builder)
            => Configure(builder.Entity<Client>());

        private static void Configure(EntityTypeBuilder<Client> builder)
        {
            builder
                .HasKey(x => x.Id);

            builder
                .Property(p => p.Id)
                .ValueGeneratedOnAdd();

            builder
                .HasMany(e => e.TrainingWallets)
                .WithOne(e => e.Client)
                .HasForeignKey(e => e.ClientId)
                .OnDelete(DeleteBehavior.NoAction);

            builder
                .HasMany(e => e.CashWallets)
                .WithOne(e => e.Client)
                .HasForeignKey(e => e.ClientId)
                .OnDelete(DeleteBehavior.NoAction);

            builder
                .HasMany(e => e.TrainingClients)
                .WithOne(e => e.Client)
                .HasForeignKey(e => e.ClientId)
                .OnDelete(DeleteBehavior.NoAction);

            builder
                .HasMany(e => e.TrainingWalletHistory)
                .WithOne(e => e.Client)
                .HasForeignKey(e => e.ClientId)
                .OnDelete(DeleteBehavior.NoAction);

            builder
                .HasMany(e => e.EventClients)
                .WithOne(e => e.Client)
                .HasForeignKey(e => e.ClientId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
