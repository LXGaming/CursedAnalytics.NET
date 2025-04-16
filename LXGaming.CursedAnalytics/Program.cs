using System.IO.Compression;
using System.Text.Json;
using LXGaming.Common.Serilog;
using LXGaming.Configuration;
using LXGaming.Configuration.File.Json;
using LXGaming.Configuration.Hosting;
using LXGaming.CursedAnalytics.Configuration;
using LXGaming.CursedAnalytics.Configuration.Categories;
using LXGaming.CursedAnalytics.Storage;
using LXGaming.Hosting.Generated;
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

Log.Information("Initialising...");

try {
    var configuration = new DefaultConfiguration();
    var config = await configuration.LoadJsonFileAsync<Config>(
        options: new JsonSerializerOptions {
            WriteIndented = true
        }
    );

    var builder = Host.CreateDefaultBuilder(args);
    builder.UseConfiguration(configuration);
    builder.UseSerilog();

    builder.ConfigureServices(services => {
        services.AddDbContext<StorageContext>(optionsBuilder => {
            var connectionString = config.Value!.ConnectionStrings["MySql"];
            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), contextOptionsBuilder => {
                contextOptionsBuilder.EnableStringComparisonTranslations();
            });
        });
        services.AddHostedService<StorageService>();

        services.Configure<QuartzOptions>(options => {
            var category = config.Value!.QuartzCategory;
            if (category.MaxConcurrency <= 0) {
                Log.Warning("MaxConcurrency is out of bounds. Resetting to {Value}", QuartzCategory.DefaultMaxConcurrency);
                category.MaxConcurrency = QuartzCategory.DefaultMaxConcurrency;
            }

            options.Add("quartz.threadPool.maxConcurrency", $"{category.MaxConcurrency}");
        });
        services.AddQuartz();
        services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

        services.AddAllServices();
    });

    var host = builder.Build();

    await host.RunAsync();
    return 0;
} catch (Exception ex) {
    Log.Fatal(ex, "Application failed to initialise");
    return 1;
} finally {
    await Log.CloseAndFlushAsync();
}