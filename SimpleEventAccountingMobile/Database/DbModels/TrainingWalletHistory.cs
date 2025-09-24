using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SimpleEventAccountingMobile.Database.DbModels
{
	public class TrainingWalletHistory : BaseEntity
	{
		public Guid ClientId { get; set; }

		public Client? Client { get; set; }

		public Guid? TrainingId { get; set; }

		public Training? Training { get; set; }

        /// <summary>
        /// Дата и врема
        /// </summary>
		public DateTime Date { get; set; }

        /// <summary>
        /// Кол-во занятий
        /// </summary>
		public decimal Count { get; set; }

        /// <summary>
        /// Кол-во пропусков
        /// </summary>
		public decimal Skip { get; set; }

        /// <summary>
        /// Сколько есть бесплатных занятий
        /// </summary>
        public decimal Free { get; set; }

        /// <summary>
        /// Абонемент
        /// </summary>
        public bool Subscription { get; set; }

		[MaxLength(1000)]
		public string? Comment { get; set; }

        public static void Configure(ModelBuilder builder)
            => Configure(builder.Entity<TrainingWalletHistory>());

        private static void Configure(EntityTypeBuilder<TrainingWalletHistory> builder)
        {
            builder
                .HasKey(x => x.Id);

            builder
                .Property(p => p.Id)
                .ValueGeneratedOnAdd();
        }
    }
}
