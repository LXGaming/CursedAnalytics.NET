using CurseForge.APIClient;
using LXGaming.Common.Hosting;
using LXGaming.Configuration;
using LXGaming.Configuration.Generic;
using LXGaming.CursedAnalytics.Configuration;
using LXGaming.CursedAnalytics.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;

namespace LXGaming.CursedAnalytics.Services.CurseForge;

[Service(ServiceLifetime.Singleton)]
public class CurseForgeService(
    IConfiguration configuration,
    ILogger<CurseForgeService> logger,
    ISchedulerFactory schedulerFactory) : IHostedService {

    public ApiClient? ApiClient { get; private set; }

    private readonly IProvider<Config> _config = configuration.GetRequiredProvider<IProvider<Config>>();

    public async Task StartAsync(CancellationToken cancellationToken) {
        var curseForgeCategory = _config.Value?.ServiceCategory.CurseForgeCategory;
        if (curseForgeCategory == null) {
            throw new InvalidOperationException("CurseForgeCategory is unavailable");
        }

        if (string.IsNullOrWhiteSpace(curseForgeCategory.Token)) {
            logger.LogWarning("Token has not been configured for CurseForge");
            return;
        }

        if (string.IsNullOrWhiteSpace(curseForgeCategory.ContactEmail)) {
            logger.LogWarning("ContactEmail is out of bounds. Resetting to {Value}", Constants.Application.Website);
            curseForgeCategory.ContactEmail = Constants.Application.Website;
        }

        ApiClient = new ApiClient(curseForgeCategory.Token, curseForgeCategory.PartnerId, curseForgeCategory.ContactEmail);
        if (curseForgeCategory.JobEnabled) {
            var scheduler = await schedulerFactory.GetScheduler(cancellationToken);
            await scheduler.ScheduleJob(
                JobBuilder.Create<CurseForgeJob>().WithIdentity(CurseForgeJob.JobKey).Build(),
                TriggerBuilder.Create().WithCronSchedule(curseForgeCategory.JobSchedule).Build(),
                cancellationToken);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) {
        ApiClient?.Dispose();
        return Task.CompletedTask;
    }
}