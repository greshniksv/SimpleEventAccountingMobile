
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SimpleEventAccountingMobile.Database.DbModels
{
	[Index(nameof(Subscription))]
	public class TrainingWallet : BaseEntity
	{
		public Guid ClientId { get; set; }

		public Client? Client { get; set; }

		/// <summary>
		/// Сколько есть теренровок
		/// </summary>
		public decimal Count { get; set; }

		/// <summary>
		/// Сколько есть пропусков
		/// </summary>
		public decimal Skip { get; set; }

		/// <summary>
		/// Сколько есть бесплатных занятий
		/// </summary>
		public decimal Free { get; set; }

		/// <summary>
		/// Адонемент
		/// </summary>
		public bool Subscription { get; set; }

		/// <summary>
		/// Удалено
		/// </summary>
		public  bool Deleted {  get; set; }

        public static void Configure(ModelBuilder builder)
            => Configure(builder.Entity<TrainingWallet>());

        private static void Configure(EntityTypeBuilder<TrainingWallet> builder)
        {
            builder
                .HasKey(x => x.Id);

            builder
                .Property(p => p.Id)
                .ValueGeneratedOnAdd();
        }
    }
}
