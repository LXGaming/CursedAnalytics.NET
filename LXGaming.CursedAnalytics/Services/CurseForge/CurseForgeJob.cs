using CurseForge.APIClient.Models.Mods;
using LXGaming.CursedAnalytics.Models;
using LXGaming.CursedAnalytics.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quartz;

namespace LXGaming.CursedAnalytics.Services.CurseForge; 

[DisallowConcurrentExecution]
public class CurseForgeJob : IJob {

    public static readonly JobKey JobKey = JobKey.Create(nameof(CurseForgeJob));

    private readonly CurseForgeService _curseForgeService;
    private readonly ILogger<CurseForgeJob> _logger;
    private readonly StorageContext _storageContext;

    public CurseForgeJob(CurseForgeService curseForgeService, ILogger<CurseForgeJob> logger, StorageContext storageContext) {
        _curseForgeService = curseForgeService;
        _logger = logger;
        _storageContext = storageContext;
    }

    public async Task Execute(IJobExecutionContext context) {
        if (_curseForgeService.ApiClient == null) {
            _logger.LogWarning("CurseForgeService is unavailable");
            return;
        }
        
        var timestamp = context.ScheduledFireTimeUtc?.LocalDateTime ?? context.FireTimeUtc.LocalDateTime;
        
        var projects = await _storageContext.Projects.ToArrayAsync();
        if (projects.Length == 0) {
            _logger.LogWarning("Missing Projects");
            return;
        }

        _logger.LogInformation("Processing {Count} {Name}", projects.Length, projects.Length == 1 ? "project" : "projects");

        foreach (var project in projects) {
            Mod mod;
            try {
                mod = (await _curseForgeService.ApiClient.GetModAsync(Convert.ToInt32(project.Id))).Data;
            } catch (Exception ex) {
                _logger.LogError(ex, "Encountered an error while getting addon {Name} ({Slug}#{Id})", project.Name, project.Slug, project.Id);
                continue;
            }
            
            if (!string.Equals(project.Name, mod.Name) || !string.Equals(project.Slug, mod.Slug)) {
                _logger.LogInformation("Project {OldName} ({OldSlug}) -> {NewName} ({NewSlug})", project.Name, project.Slug, mod.Name, mod.Slug);
                project.Name = mod.Name;
                project.Slug = mod.Slug;
            }

            _storageContext.ProjectDownloads.Add(new ProjectDownload {
                ProjectId = project.Id,
                Timestamp = timestamp,
                Value = Convert.ToInt64(mod.DownloadCount)
            });
            
            _storageContext.ProjectPopularity.Add(new ProjectPopularity {
                ProjectId = project.Id,
                Timestamp = timestamp,
                Score = 0,
                Rank = mod.GamePopularityRank,
                ThumbsUp = mod.ThumbsUpCount ?? 0
            });

            _logger.LogInformation("Processed {Name} ({Slug}#{Id})", project.Name, project.Slug, project.Id);
        }

        await _storageContext.SaveChangesAsync();
    }
}