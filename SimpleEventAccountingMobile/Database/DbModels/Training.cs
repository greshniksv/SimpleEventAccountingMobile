using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SimpleEventAccountingMobile.Database.DbModels
{
	public class Training : BaseEntity
	{
		[MaxLength(100)]
		public string? Name { get; set; }

		[MaxLength(1000)]
		public string? Description { get; set; }

		public DateTime Date { get; set; }

		public bool Deleted { get; set; }

		public ICollection<TrainingClient>? TrainingClients { get; set; }

		public ICollection<TrainingWalletHistory>? TrainingWalletHistory { get; set; }

        public static void Configure(ModelBuilder builder)
            => Configure(builder.Entity<Training>());

        private static void Configure(EntityTypeBuilder<Training> builder)
        {
            builder
                .HasKey(x => x.Id);

            builder
                .Property(p => p.Id)
                .ValueGeneratedOnAdd();

            builder
                .HasMany(e => e.TrainingClients)
                .WithOne(e => e.Training)
                .HasForeignKey(e => e.TrainingId)
                .OnDelete(DeleteBehavior.NoAction);

            builder
                .HasMany(e => e.TrainingWalletHistory)
                .WithOne(e => e.Training)
                .HasForeignKey(e => e.TrainingId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
