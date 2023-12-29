﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LXGaming.CursedAnalytics.Storage;

public class StorageService(ILogger<StorageService> logger, IServiceProvider serviceProvider) : IHostedService {

    public async Task StartAsync(CancellationToken cancellationToken) {
        await using var scope = serviceProvider.CreateAsyncScope();
        await using var storageContext = scope.ServiceProvider.GetRequiredService<StorageContext>();
        if (await storageContext.Database.EnsureCreatedAsync(cancellationToken)) {
            logger.LogInformation("Database Created");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) {
        return Task.CompletedTask;
    }
}