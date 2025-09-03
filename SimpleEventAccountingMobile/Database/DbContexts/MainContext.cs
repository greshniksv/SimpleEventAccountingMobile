using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using SimpleEventAccountingMobile.Database.DbModels;
using SimpleEventAccountingMobile.Database.Interfaces;

namespace SimpleEventAccountingMobile.Database.DbContexts
{
	public class MainContext : DbContext, IMainContext
	{
        public MainContext(DbContextOptions<MainContext> options)
            : base(options)
        {
        }

        public DbContext GetDbContext()
		{
			return this;
		}

		public DbSet<CashWalletHistory> CashWalletHistory { get; set; }
		public DbSet<CashWallet> CashWallets { get; set; }
		public DbSet<Client> Clients { get; set; }
		public DbSet<Event> ActionEvents { get; set; }
		public DbSet<EventClient> EventClients { get; set; }
		public DbSet<Training> Trainings { get; set; }
		public DbSet<TrainingClient> TrainingClients { get; set; }
		public DbSet<TrainingWallet> TrainingWallets { get; set; }
		public DbSet<TrainingWalletHistory> TrainingWalletHistory { get; set; }
        public DbSet<TrainingChangeSet> TrainingChangeSets { get; set; }
        public DbSet<EventChangeSet> EventChangeSets { get; set; }

        public DatabaseFacade GetDatabase()
		{
			return Database;
		}

		public IModel GetModel()
		{
			return Model;
		}

		public Task<IDbContextTransaction> BeginTransactionAsync(
			CancellationToken cancellationToken = default)
		{
			return GetDatabase().BeginTransactionAsync(cancellationToken);
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			CashWallet.Configure(modelBuilder);
            DbModels.CashWalletHistory.Configure(modelBuilder);
            Client.Configure(modelBuilder);
            Event.Configure(modelBuilder);
            EventClient.Configure(modelBuilder);
            Training.Configure(modelBuilder);
            TrainingClient.Configure(modelBuilder);
            TrainingWallet.Configure(modelBuilder);
            DbModels.TrainingWalletHistory.Configure(modelBuilder);
            TrainingChangeSet.Configure(modelBuilder);
            EventChangeSet.Configure(modelBuilder);
        }
	}
}
