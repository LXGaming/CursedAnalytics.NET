using CurseForge.APIClient;
using LXGaming.Common.Hosting;
using LXGaming.CursedAnalytics.Configuration;
using LXGaming.CursedAnalytics.Services.Quartz;
using LXGaming.CursedAnalytics.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;

namespace LXGaming.CursedAnalytics.Services.CurseForge;

[Service(ServiceLifetime.Singleton)]
public class CurseForgeService : IHostedService {

    public ApiClient? ApiClient { get; private set; }

    private readonly IConfiguration _configuration;
    private readonly ILogger<CurseForgeService> _logger;
    private readonly ISchedulerFactory _schedulerFactory;

    public CurseForgeService(IConfiguration configuration, ILogger<CurseForgeService> logger, ISchedulerFactory schedulerFactory) {
        _configuration = configuration;
        _logger = logger;
        _schedulerFactory = schedulerFactory;
    }

    public async Task StartAsync(CancellationToken cancellationToken) {
        var curseForgeCategory = _configuration.Config?.ServiceCategory.CurseForgeCategory;
        if (curseForgeCategory == null) {
            throw new InvalidOperationException("CurseForgeCategory is unavailable");
        }

        if (string.IsNullOrWhiteSpace(curseForgeCategory.Token)) {
            _logger.LogWarning("Token has not been configured for CurseForge");
            return;
        }

        if (string.IsNullOrWhiteSpace(curseForgeCategory.ContactEmail)) {
            _logger.LogWarning("ContactEmail is out of bounds. Resetting to {Value}", Constants.Application.Website);
            curseForgeCategory.ContactEmail = Constants.Application.Website;
        }

        ApiClient = new ApiClient(curseForgeCategory.Token, curseForgeCategory.PartnerId, curseForgeCategory.ContactEmail);
        if (curseForgeCategory.JobEnabled) {
            var scheduler = await _schedulerFactory.GetScheduler(cancellationToken);
            await scheduler.ScheduleJobAsync<CurseForgeJob>(CurseForgeJob.JobKey, TriggerBuilder.Create().WithCronSchedule(curseForgeCategory.JobSchedule).Build());
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) {
        ApiClient?.Dispose();
        return Task.CompletedTask;
    }
}