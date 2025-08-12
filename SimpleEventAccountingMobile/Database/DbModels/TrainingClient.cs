using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SimpleEventAccountingMobile.Database.DbModels
{
	public class TrainingClient : BaseEntity
	{
		public Guid TrainingId { get; set; }

		public Training? Training { get; set; }

		public Guid ClientId { get; set; }

		public Client? Client { get; set; }

        public static void Configure(ModelBuilder builder)
            => Configure(builder.Entity<TrainingClient>());

        private static void Configure(EntityTypeBuilder<TrainingClient> builder)
        {
            builder
                .HasKey(x => x.Id);

            builder
                .Property(p => p.Id)
                .ValueGeneratedOnAdd();
        }
    }
}
