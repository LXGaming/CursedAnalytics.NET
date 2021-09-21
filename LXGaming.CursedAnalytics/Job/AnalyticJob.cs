using System;
using System.Threading.Tasks;
using LXGaming.CursedAnalytics.Entity;
using LXGaming.CursedAnalytics.Manager;
using LXGaming.CursedAnalytics.Storage;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Quartz;
using Serilog;

namespace LXGaming.CursedAnalytics.Job {

    [DisallowConcurrentExecution]
    public class AnalyticJob : IJob {

        public static readonly JobKey JobKey = JobKey.Create(nameof(AnalyticJob));

        public async Task Execute(IJobExecutionContext context) {
            var timestamp = context.ScheduledFireTimeUtc?.LocalDateTime ?? context.FireTimeUtc.LocalDateTime;

            await using var storage = StorageContext.Create();
            var projects = await storage.Projects.ToArrayAsync();
            if (projects == null || projects.Length == 0) {
                Log.Warning("Missing Projects");
                return;
            }

            Log.Information("Processing {Count} {Name}", projects.Length, projects.Length == 1 ? "project" : "projects");

            foreach (var project in projects) {
                JObject addon;
                try {
                    addon = await WebManager.GetAddonAsync(project.Id);
                } catch (Exception ex) {
                    Log.Error(ex, "Encountered an error while getting addon {Name} ({Slug}#{Id})", project.Name, project.Slug, project.Id);
                    continue;
                }

                var id = addon.Value<long>("id");
                var name = addon.Value<string>("name");
                var slug = addon.Value<string>("slug");
                var download = addon.Value<long>("downloadCount");
                var score = addon.Value<decimal>("popularityScore");
                var rank = addon.Value<long>("gamePopularityRank");

                if (!string.Equals(project.Name, name) || !string.Equals(project.Slug, slug)) {
                    Log.Information("Project {OldName} ({OldSlug}) -> {NewName} ({NewSlug})", project.Name, project.Slug, name, slug);
                    project.Name = name;
                    project.Slug = slug;
                }

                storage.ProjectDownloads.Add(new ProjectDownload {
                    ProjectId = project.Id,
                    Timestamp = timestamp,
                    Value = download
                });

                storage.ProjectPopularity.Add(new ProjectPopularity {
                    ProjectId = project.Id,
                    Timestamp = timestamp,
                    Score = score,
                    Rank = rank
                });

                Log.Information("Processed {Name} ({Slug}#{Id})", name, slug, id);
            }

            await storage.SaveChangesAsync();
        }
    }
}