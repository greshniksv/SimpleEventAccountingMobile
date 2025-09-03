
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
        public DateTime? DeletedAt { get; set; }

        // Реализация ICloneable
        public object Clone()
        {
            TrainingWallet clone = new TrainingWallet
            {
                // Копируем все свойства
                Id = Id,
                ClientId = ClientId,
                Count = Count,
                Skip = Skip,
                Free = Free,
                Subscription = Subscription,
                DeletedAt = DeletedAt,
                Client = null
            };

            return clone;
        }

        // Реализация IEquatable
        public bool Equals(TrainingWallet? other)
        {
            if (other == null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return
                Id == other.Id &&
                ClientId == other.ClientId &&
                Count == other.Count &&
                Skip == other.Skip &&
                Free == other.Free &&
                Subscription == other.Subscription &&
                DeletedAt == other.DeletedAt;
        }

        // Переопределение Object.Equals
        public override bool Equals(object? obj)
        {
            if (obj is TrainingWallet wallet)
                return Equals(wallet);
            return false;
        }

        // Переопределение GetHashCode
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + Id.GetHashCode();
                hash = hash * 23 + ClientId.GetHashCode();
                hash = hash * 23 + Count.GetHashCode();
                hash = hash * 23 + Skip.GetHashCode();
                hash = hash * 23 + Free.GetHashCode();
                hash = hash * 23 + Subscription.GetHashCode();
                hash = hash * 23 + DeletedAt.GetHashCode();

                return hash;
            }
        }

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
