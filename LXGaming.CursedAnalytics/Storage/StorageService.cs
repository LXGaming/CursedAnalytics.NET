using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LXGaming.CursedAnalytics.Storage;

public class StorageService : IHostedService {

    private readonly ILogger<StorageService> _logger;
    private readonly IServiceProvider _serviceProvider;

    public StorageService(ILogger<StorageService> logger, IServiceProvider serviceProvider) {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken) {
        await using var scope = _serviceProvider.CreateAsyncScope();
        await using var storageContext = scope.ServiceProvider.GetRequiredService<StorageContext>();
        if (await storageContext.Database.EnsureCreatedAsync(cancellationToken)) {
            _logger.LogInformation("Database Created");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) {
        return Task.CompletedTask;
    }
}