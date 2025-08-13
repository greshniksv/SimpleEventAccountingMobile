using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MudBlazor.Services;
using SimpleEventAccountingMobile.Database.DbContexts;
using SimpleEventAccountingMobile.Database.Interfaces;
using SimpleEventAccountingMobile.Services;
using SimpleEventAccountingMobile.Services.Interfaces;

namespace SimpleEventAccountingMobile
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();
            builder.Services.AddMudServices();

            builder.Services.AddDbContext<MainContext>(opt =>
                opt.UseSqlite("Filename=default.sqlite"));
            builder.Services.AddScoped<IMainContext, MainContext>();
            builder.Services.AddScoped<ITrainingService, TrainingService>();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif

            // 1. Собираем приложение
            var app = builder.Build();

            // 2. Применяем миграции
            ApplyMigrations(app.Services);

            // 3. Возвращаем собранное приложение
            return app;
        }

        private static void ApplyMigrations(IServiceProvider services)
        {
            // Создаем "область" для получения сервисов.
            // Это лучшая практика, чтобы гарантировать, что DbContext будет правильно уничтожен.
            using var scope = services.CreateScope();

            try
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<MainContext>();

                // Вызываем метод, который применяет все ожидающие миграции.
                // Если БД не существует, она будет создана.
                dbContext.Database.Migrate();

                // Опционально: здесь же можно добавить начальные данные (сидинг)
                // SeedData(dbContext);
            }
            catch (Exception ex)
            {
                // Здесь можно залогировать ошибку, если миграция не удалась
                Console.WriteLine($"An error occurred while migrating the database: {ex.Message}");
                // В реальном приложении используйте систему логирования
            }
        }
    }
}
