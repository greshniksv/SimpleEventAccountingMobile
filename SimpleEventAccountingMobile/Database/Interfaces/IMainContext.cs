using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using SimpleEventAccountingMobile.Database.DbModels;

namespace SimpleEventAccountingMobile.Database.Interfaces
{
	public interface IMainContext: IDisposable
	{
		DbContext GetDbContext();

		DbSet<CashWalletHistory> CashWalletHistory { get; set; }
		DbSet<CashWallet> CashWallets { get; set; }
		DbSet<Client> Clients { get; set; }
		DbSet<Event> ActionEvents { get; set; }
		DbSet<EventClient> EventClients { get; set; }
		DbSet<Training> Trainings { get; set; }
		DbSet<TrainingClient> TrainingClients { get; set; }
		DbSet<TrainingWallet> TrainingWallets { get; set; }
		DbSet<TrainingWalletHistory> TrainingWalletHistory { get; set; }
        DbSet<TrainingChangeSet> TrainingChangeSets { get; set; }
        DbSet<EventChangeSet> EventChangeSets { get; set; }

        int SaveChanges();

		int SaveChanges(bool acceptAllChangesOnSuccess);

		Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

		Task<int> SaveChangesAsync(
			bool acceptAllChangesOnSuccess,
			CancellationToken cancellationToken = default);

		DbSet<TEntity> Set<TEntity>()
			where TEntity : class;

		DbSet<TEntity> Set<TEntity>(string name)
			where TEntity : class;

		EntityEntry<TEntity> Entry<TEntity>(TEntity entity)
			where TEntity : class;

		EntityEntry Entry(object entity);

		DatabaseFacade GetDatabase();

		IModel GetModel();

		Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
	}
}
