using LXGaming.CursedAnalytics.Configuration.Category;
using LXGaming.CursedAnalytics.Entity;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace LXGaming.CursedAnalytics.Storage.MySql;

public class MySqlStorageContext : StorageContext {

    private readonly string _connectionString;

    public MySqlStorageContext(StorageCategory storageCategory) {
        _connectionString = new MySqlConnectionStringBuilder {
            Server = storageCategory.Host,
            Port = storageCategory.Port,
            Database = storageCategory.Database,
            UserID = storageCategory.Username,
            Password = storageCategory.Password,
            Pooling = true,
            MaximumPoolSize = storageCategory.MaximumPoolSize,
            MinimumPoolSize = storageCategory.MinimumPoolSize
        }.ConnectionString;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseMySql(_connectionString, ServerVersion.AutoDetect(_connectionString));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);

        // Fix columns
        modelBuilder.Entity<ProjectDownload>().Property(model => model.Timestamp).HasColumnType("datetime");
        modelBuilder.Entity<ProjectPoint>().Property(model => model.Timestamp).HasColumnType("datetime");
        modelBuilder.Entity<ProjectPoint>().Property(model => model.Value).HasColumnType("decimal(12,2)");
        modelBuilder.Entity<ProjectPopularity>().Property(model => model.Timestamp).HasColumnType("datetime");
        modelBuilder.Entity<ProjectPopularity>().Property(model => model.Score).HasColumnType("decimal(26,16)");
    }
}