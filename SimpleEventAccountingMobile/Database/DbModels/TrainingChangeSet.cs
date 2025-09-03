using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SimpleEventAccountingMobile.Database.DbModels
{
    public class TrainingChangeSet : BaseEntity
    {
        public Guid ClientId { get; set; }

        public Client? Client { get; set; }

        public Guid TrainingId { get; set; }

        public Training? Training { get; set; }

        /// <summary>
        /// Посещения
        /// </summary>
        public decimal? Count { get; set; }

        /// <summary>
        /// Пропуски
        /// </summary>
        public decimal? Skip { get; set; }

        /// <summary>
        /// Сколько есть бесплатных занятий
        /// </summary>
        public decimal? Free { get; set; }

        /// <summary>
        /// Абонемент
        /// </summary>
        public bool? Subscription { get; set; }

        public static void Configure(ModelBuilder builder)
            => Configure(builder.Entity<TrainingChangeSet>());

        private static void Configure(EntityTypeBuilder<TrainingChangeSet> builder)
        {
            builder
                .HasKey(x => x.Id);

            builder
                .Property(p => p.Id)
                .ValueGeneratedOnAdd();
        }
    }
}
