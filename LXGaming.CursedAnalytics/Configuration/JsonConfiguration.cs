using System.Text.Json;
using System.Text.Json.Serialization;

namespace LXGaming.CursedAnalytics.Configuration;

public class JsonConfiguration : IConfiguration {

    private static readonly JsonSerializerOptions JsonSerializerOptions = new() {
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
        WriteIndented = true
    };

    private readonly string _configPath;

    public Config? Config { get; private set; }

    public JsonConfiguration(string path) {
        _configPath = Path.Combine(path, "config.json");
    }

    public async Task LoadConfigurationAsync(CancellationToken cancellationToken = default) {
        Config = await DeserializeFileAsync<Config>(_configPath, cancellationToken);
    }

    public async Task SaveConfigurationAsync(CancellationToken cancellationToken = default) {
        await SerializeFileAsync(_configPath, Config, cancellationToken);
    }

    private async Task<T> DeserializeFileAsync<T>(string path, CancellationToken cancellationToken = default) {
        if (!File.Exists(path)) {
            var value = Activator.CreateInstance<T>();
            await SerializeFileAsync(path, value, cancellationToken);
            return value;
        }

        await using var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        return await JsonSerializer.DeserializeAsync<T>(stream, JsonSerializerOptions, cancellationToken)
               ?? throw new JsonException($"Failed to deserialize {nameof(T)}");
    }

    private async Task SerializeFileAsync<T>(string path, T value, CancellationToken cancellationToken = default) {
        if (!File.Exists(path)) {
            var directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directory)) {
                Directory.CreateDirectory(directory);
            }
        }

        await using var stream = File.Open(path, FileMode.Create, FileAccess.Write, FileShare.None);
        await JsonSerializer.SerializeAsync(stream, value, JsonSerializerOptions, cancellationToken);
    }
}