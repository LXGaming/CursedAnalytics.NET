using System.IO.Compression;
using System.Reflection;
using LXGaming.Common.Hosting;
using LXGaming.Common.Serilog;
using LXGaming.CursedAnalytics.Configuration;
using LXGaming.CursedAnalytics.Configuration.Category;
using LXGaming.CursedAnalytics.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.File.Archive;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.ControlledBy(new EnvironmentLoggingLevelSwitch(LogEventLevel.Verbose, LogEventLevel.Debug))
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
    .MinimumLevel.Override("Quartz", LogEventLevel.Information)
    .MinimumLevel.Override("Quartz.Core.ErrorLogger", LogEventLevel.Fatal)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File(
        Path.Combine("logs", "app-.log"),
        buffered: true,
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 1,
        hooks: new ArchiveHooks(CompressionLevel.Optimal))
    .CreateBootstrapLogger();

Log.Information("Initializing...");

try {
    var configuration = new JsonConfiguration(Directory.GetCurrentDirectory());
    await configuration.LoadConfigurationAsync();

    var builder = Host.CreateDefaultBuilder(args);
    builder.UseSerilog();

    builder.ConfigureServices(services => {
        services.AddSingleton<IConfiguration>(configuration);

        services.AddDbContext<StorageContext>(optionsBuilder => {
            var connectionString = configuration.Config!.ConnectionStrings["MySql"];
            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), contextOptionsBuilder => {
                contextOptionsBuilder.EnableStringComparisonTranslations();
            });
        });
        services.AddService<StorageService>();

        services.Configure<QuartzOptions>(options => {
            var quartzCategory = configuration.Config!.QuartzCategory;
            if (quartzCategory.MaxConcurrency <= 0) {
                Log.Warning("MaxConcurrency is out of bounds. Resetting to {Value}", QuartzCategory.DefaultMaxConcurrency);
                quartzCategory.MaxConcurrency = QuartzCategory.DefaultMaxConcurrency;
            }

            options.Set("quartz.threadPool.maxConcurrency", $"{quartzCategory.MaxConcurrency}");
        });
        services.AddQuartz(configurator => {
            configurator.UseMicrosoftDependencyInjectionJobFactory();
        });
        services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

        services.AddAllServices(Assembly.GetExecutingAssembly());
    });

    var host = builder.Build();

    await host.RunAsync();
    return 0;
} catch (Exception ex) {
    Log.Fatal(ex, "Application failed to initialize");
    return 1;
} finally {
    Log.CloseAndFlush();
}