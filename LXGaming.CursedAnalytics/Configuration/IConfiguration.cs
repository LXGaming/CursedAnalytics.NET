namespace LXGaming.CursedAnalytics.Configuration;

public interface IConfiguration {

    Config? Config { get; }

    Task LoadConfigurationAsync(CancellationToken cancellationToken = default);

    Task SaveConfigurationAsync(CancellationToken cancellationToken = default);
}