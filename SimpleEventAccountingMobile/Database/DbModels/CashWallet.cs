using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SimpleEventAccountingMobile.Database.DbModels
{
	public class CashWallet : BaseEntity, ICloneable, IEquatable<CashWallet>
	{
		public Guid ClientId { get; set; }

		public Client? Client { get; set; }

        public decimal Cash { get; set; }

        public DateTime? DeletedAt { get; set; }

        public static void Configure(ModelBuilder builder)
            => Configure(builder.Entity<CashWallet>());

        public object Clone()
        {
            // Создаем новый экземпляр класса
            CashWallet clone = new CashWallet
            {
                Id = Id,
                ClientId = ClientId,
                Cash = Cash,
                DeletedAt = DeletedAt,
                Client = null
            };

            return clone;
        }

        // Реализация IEquatable
        public bool Equals(CashWallet? other)
        {
            // Проверка на null
            if (other == null)
                return false;

            // Проверка на ссылку
            if (ReferenceEquals(this, other))
                return true;

            // Сравнение всех значимых свойств
            return
                Id == other.Id &&
                ClientId == other.ClientId &&
                Cash == other.Cash &&
                DeletedAt == other.DeletedAt;
        }

        // Переопределение Object.Equals
        public override bool Equals(object? obj)
        {
            if (obj is CashWallet wallet)
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
                hash = hash * 23 + Cash.GetHashCode();
                hash = hash * 23 + DeletedAt.GetHashCode();

                return hash;
            }
        }

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
