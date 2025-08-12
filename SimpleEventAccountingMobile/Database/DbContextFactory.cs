using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using SimpleEventAccountingMobile.Database.DbContexts;

namespace SimpleEventAccountingMobile.Database
{
    public class DbContextFactory : IDesignTimeDbContextFactory<MainContext>
    {
        public MainContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<MainContext>();

            // Укажите здесь строку подключения для ВРЕМЕНИ РАЗРАБОТКИ.
            // Это может быть простая локальная база данных. EF Core использует ее
            // только для того, чтобы прочитать структуру модели и создать миграцию.
            // Она НЕ будет использоваться при запуске приложения на телефоне.
            // Самый простой вариант для SQLite:
            var connectionString = "Data Source=design_time.db";
            optionsBuilder.UseSqlite(connectionString);

            return new MainContext(optionsBuilder.Options);
        }
    }
}
