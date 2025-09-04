using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using SimpleEventAccountingMobile.Database.DbContexts;
using SimpleEventAccountingMobile.Database.Interfaces;
using SimpleEventAccountingMobile.Services;
using SimpleEventAccountingMobile.Services.Interfaces;
using System.Diagnostics;
using System.Globalization;
using SimpleEventAccountingMobile.Models;
using ILogger = Serilog.ILogger;

namespace SimpleEventAccountingMobile
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            // Configure Serilog
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Async(a => a.File(
                    Path.Combine(FileSystem.AppDataDirectory, "logs", "log-.txt"),
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 3,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}"))
                .CreateLogger();

            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();

            var path = Path.Combine(FileSystem.AppDataDirectory, "SimpleEventAccountingMobile.db3");
            builder.Services.AddDbContext<MainContext>(opt =>
                opt.UseSqlite($"Data Source={path}"));

            builder.Services.AddScoped<IMainContext, MainContext>();
            builder.Services.AddScoped<ITrainingService, TrainingService>();
            builder.Services.AddScoped<IClientService, ClientService>();
            builder.Services.AddScoped<IEventService, EventService>();
            builder.Services.AddSingleton<ILogFileService, LogFileService>();
            builder.Services.AddTransient<LogFileBrowserViewModel>();

            // Add Serilog logging
            builder.Services.AddSingleton<ILogger>(Log.Logger);

            builder.Services.AddLocalization();
            //builder.Services.AddSingleton<IStringLocalizer>(provider =>
            //{
            //    return provider.GetRequiredService<IStringLocalizerFactory>()
            //        .Create("Strings", Assembly.GetExecutingAssembly().GetName().Name);
            //});

            builder.Services.AddTransient<IStringLocalizer, CustomStringLocalizer>();
            builder.Services.AddScoped<IEventCreationStateManager, EventCreationStateManager>();
            builder.Services.AddScoped<IEventCreationHandler, EventCreationHandler>();
            builder.Services.AddSingleton<IImportExportService, ImportExportService>();
            builder.Services.AddSingleton<ILanguageService, LanguageService>();
            builder.Services.AddSingleton<IAppInfoService, AppInfoService>();
            builder.Services.AddSingleton<IErrorService, ErrorService>();

            //var savedLanguage = Preferences.Get("AppLanguage", CultureInfo.CurrentCulture.Name);
            //var currentCulture = new CultureInfo(savedLanguage);
            //CultureInfo.DefaultThreadCurrentCulture = currentCulture;
            //CultureInfo.DefaultThreadCurrentUICulture = currentCulture;

            Log.Logger.Information("Start application");

            // Настройка русской культуры
            var culture = new CultureInfo("ru-RU");
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

            // Добавьте глобальный обработчик не перехваченных исключений
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            TaskScheduler.UnobservedTaskException += OnTaskSchedulerUnobservedTaskException;

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

        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            // Логирование ошибки (опционально)
            var exception = e.ExceptionObject as Exception;
            Debug.WriteLine($"Unhandled Exception: {exception?.Message}\n{exception?.StackTrace}");
            
            Log.Logger.Fatal(exception, $"Unhandled Exception: {exception?.Message}\n{exception?.StackTrace}");
        }

        private static void OnTaskSchedulerUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
        {
            Log.Logger.Fatal(e.Exception, $"Unobserved Task Exception: {e.Exception.Message}\n{e.Exception.StackTrace}");

            // Помечаем как обработанную
            e.SetObserved();
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
                Debug.WriteLine($"An error occurred while migrating the database: {ex.Message}");
                // В реальном приложении используйте систему логирования
            }
        }
    }
}
