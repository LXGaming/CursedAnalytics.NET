using System.Linq;
using System.Threading.Tasks;
using LXGaming.CursedAnalytics.Entity;
using LXGaming.CursedAnalytics.Manager;
using LXGaming.CursedAnalytics.Storage;
using Microsoft.EntityFrameworkCore;
using Quartz;
using Serilog;

namespace LXGaming.CursedAnalytics.Job {

    [DisallowConcurrentExecution]
    public class AnalyticJob : IJob {

        public static readonly JobKey JobKey = JobKey.Create(nameof(AnalyticJob));

        public async Task Execute(IJobExecutionContext context) {
            await using var storage = StorageContext.Create();
            var projects = await storage.Projects.ToArrayAsync();
            if (projects == null || projects.Length == 0) {
                Log.Warning("Missing Projects");
                return;
            }

            var addons = await WebManager.GetAddonsAsync(projects.Select(model => model.Id).ToArray());
            if (addons == null || addons.Count == 0) {
                Log.Warning("Missing Addons");
                return;
            }

            Log.Information("Processing {Count} {Name}", addons.Count, addons.Count == 1 ? "addon" : "addons");

            foreach (var addon in addons) {
                var id = addon.Value<long>("id");
                var name = addon.Value<string>("name");
                var slug = addon.Value<string>("slug");
                var downloads = addon.Value<long>("downloadCount");

                var project = projects.SingleOrDefault(model => model.Id == id);
                if (project == null) {
                    Log.Warning("Missing Project {Name} ({Slug}#{Id})", name, slug, id);
                    continue;
                }

                if (!string.Equals(project.Name, name) || !string.Equals(project.Slug, slug)) {
                    Log.Information("Project {OldName} ({OldSlug}) -> {NewName} ({NewSlug})", project.Name, project.Slug, name, slug);
                    project.Name = name;
                    project.Slug = slug;
                }

                storage.ProjectDownloads.Add(new ProjectDownload {
                    ProjectId = project.Id,
                    Timestamp = context.ScheduledFireTimeUtc?.LocalDateTime ?? context.FireTimeUtc.LocalDateTime,
                    Value = downloads
                });

                Log.Information("Processed {Name} ({Slug}#{Id})", name, slug, id);
            }

            await storage.SaveChangesAsync();
        }
    }
}