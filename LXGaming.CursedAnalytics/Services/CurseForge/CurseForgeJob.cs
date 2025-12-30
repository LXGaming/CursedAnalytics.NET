using CurseForge.APIClient.Models.Mods;
using LXGaming.CursedAnalytics.Storage;
using LXGaming.CursedAnalytics.Storage.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quartz;

namespace LXGaming.CursedAnalytics.Services.CurseForge;

[DisallowConcurrentExecution]
public class CurseForgeJob(
    CurseForgeService curseForgeService,
    ILogger<CurseForgeJob> logger,
    StorageContext storageContext) : IJob {

    public static readonly JobKey JobKey = JobKey.Create(nameof(CurseForgeJob));

    public async Task Execute(IJobExecutionContext context) {
        if (curseForgeService.ApiClient == null) {
            logger.LogWarning("CurseForgeService is unavailable");
            return;
        }

        var timestamp = context.ScheduledFireTimeUtc?.LocalDateTime ?? context.FireTimeUtc.LocalDateTime;

        var projects = await storageContext.Projects.ToArrayAsync();
        if (projects.Length == 0) {
            logger.LogWarning("Missing Projects");
            return;
        }

        logger.LogInformation("Processing {Count} {Name}", projects.Length, projects.Length == 1 ? "project" : "projects");

        foreach (var project in projects) {
            Mod mod;
            try {
                mod = (await curseForgeService.ApiClient.GetModAsync(Convert.ToInt32(project.Id))).Data;
            } catch (Exception ex) {
                logger.LogError(ex, "Encountered an error while getting addon {Name} ({Slug}#{Id})", project.Name, project.Slug, project.Id);
                continue;
            }

            if (!string.Equals(project.Name, mod.Name) || !string.Equals(project.Slug, mod.Slug)) {
                logger.LogInformation("Project {OldName} ({OldSlug}) -> {NewName} ({NewSlug})", project.Name, project.Slug, mod.Name, mod.Slug);
                project.Name = mod.Name;
                project.Slug = mod.Slug;
            }

            storageContext.ProjectDownloads.Add(new ProjectDownload {
                ProjectId = project.Id,
                Timestamp = timestamp,
                Value = Convert.ToInt64(mod.DownloadCount)
            });

            storageContext.ProjectPopularity.Add(new ProjectPopularity {
                ProjectId = project.Id,
                Timestamp = timestamp,
                Score = 0,
                Rank = mod.GamePopularityRank,
                ThumbsUp = mod.ThumbsUpCount
            });

            logger.LogInformation("Processed {Name} ({Slug}#{Id})", project.Name, project.Slug, project.Id);
        }

        await storageContext.SaveChangesAsync();
    }
}