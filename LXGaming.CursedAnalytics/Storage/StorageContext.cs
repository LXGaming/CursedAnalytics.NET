using LXGaming.CursedAnalytics.Storage.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace LXGaming.CursedAnalytics.Storage;

public class StorageContext(DbContextOptions options) : DbContext(options) {

    private static readonly ValueConverter<DateTime, DateTime> DateTimeConverter = new(
        value => value,
        value => DateTime.SpecifyKind(value, DateTimeKind.Local));

    public required DbSet<Project> Projects { get; init; }
    public required DbSet<ProjectDownload> ProjectDownloads { get; init; }
    public required DbSet<ProjectPopularity> ProjectPopularity { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);

        ApplyDateTimeConverter(modelBuilder);

        // Constraints
        modelBuilder.Entity<Project>().HasIndex(model => model.Slug).IsUnique();
        modelBuilder.Entity<ProjectDownload>().HasIndex(model => new { model.ProjectId, model.Timestamp }).IsUnique();
        modelBuilder.Entity<ProjectPopularity>().HasIndex(model => new { model.ProjectId, model.Timestamp }).IsUnique();
        modelBuilder.Entity<ProjectDownload>().Property(model => model.Timestamp).HasColumnType("datetime");
        modelBuilder.Entity<ProjectPopularity>().Property(model => model.Timestamp).HasColumnType("datetime");
        modelBuilder.Entity<ProjectPopularity>().Property(model => model.Score).HasColumnType("decimal(26,16)");
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
            var createdAt = entity.Properties.SingleOrDefault(property => property.Metadata.Name.Equals("CreatedAt"));
            if (createdAt != null && entity.State == EntityState.Added) {
                if (createdAt.CurrentValue == null || createdAt.CurrentValue.Equals(default(DateTime))) {
                    createdAt.CurrentValue = now;
                }
            }

            var updatedAt = entity.Properties.SingleOrDefault(property => property.Metadata.Name.Equals("UpdatedAt"));
            if (updatedAt != null && entity.State is EntityState.Added or EntityState.Modified) {
                updatedAt.CurrentValue = now;
            }
        }
    }

    private void ApplyDateTimeConverter(ModelBuilder modelBuilder) {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes()) {
            foreach (var property in entityType.GetProperties()) {
                if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?)) {
                    property.SetValueConverter(DateTimeConverter);
                }
            }
        }
    }
}