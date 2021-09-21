using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LXGaming.CursedAnalytics.Entity;
using LXGaming.CursedAnalytics.Storage.MySql;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace LXGaming.CursedAnalytics.Storage {

    public class StorageContext : DbContext {

        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectDownload> ProjectDownloads { get; set; }
        public DbSet<ProjectPoint> ProjectPoints { get; set; }
        public DbSet<ProjectPopularity> ProjectPopularity { get; set; }

        /// <summary>
        ///     Use <see cref="StorageContext.Create()"/>
        /// </summary>
        protected StorageContext() {
        }

        public static StorageContext Create() {
            var storageCategory = CursedAnalytics.Instance.Config.StorageCategory;
            if (string.IsNullOrWhiteSpace(storageCategory.Engine)) {
                Log.Warning("No storage engine configured");
                return new StorageContext();
            }

            if (storageCategory.Engine.Equals("mysql", StringComparison.OrdinalIgnoreCase)) {
                return new MySqlStorageContext(storageCategory);
            }

            Log.Warning("Invalid storage engine configured");
            return new StorageContext();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);

            // Fix indexes
            modelBuilder.Entity<Project>().HasIndex(model => model.Slug).IsUnique();
            modelBuilder.Entity<ProjectDownload>().HasIndex(model => new {model.ProjectId, model.Timestamp}).IsUnique();
            modelBuilder.Entity<ProjectPoint>().HasIndex(model => new {model.ProjectId, model.Timestamp}).IsUnique();
            modelBuilder.Entity<ProjectPopularity>().HasIndex(model => new {model.ProjectId, model.Timestamp}).IsUnique();
        }

        public override int SaveChanges() {
            UpdateTimestamps();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) {
            UpdateTimestamps();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateTimestamps() {
            var now = DateTime.Now;
            var entities = ChangeTracker.Entries();
            foreach (var entity in entities) {
                var createdAt = entity.Properties.SingleOrDefault(entry => entry.Metadata.Name.Equals("CreatedAt"));
                if (createdAt != null && entity.State == EntityState.Added) {
                    if (createdAt.CurrentValue == null || createdAt.CurrentValue.Equals(default(DateTime))) {
                        createdAt.CurrentValue = now;
                    }
                }

                var updatedAt = entity.Properties.SingleOrDefault(entry => entry.Metadata.Name.Equals("UpdatedAt"));
                if (updatedAt != null && (entity.State == EntityState.Added || entity.State == EntityState.Modified)) {
                    updatedAt.CurrentValue = now;
                }
            }
        }
    }
}