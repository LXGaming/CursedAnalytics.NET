using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Serilog;

namespace LXGaming.CursedAnalytics.Configuration {

    public class JsonConfiguration : IConfiguration {

        private static readonly JsonSerializerOptions JsonSerializerOptions = new() {
            IncludeFields = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        private readonly string _path;

        public Config Config { get; private set; }

        public JsonConfiguration(string path) {
            _path = path;
        }

        public async Task<bool> LoadConfigurationAsync(CancellationToken cancellationToken = default) {
            var config = await ((IConfiguration) this).LoadFileAsync<Config>(Path.Combine(_path, "config.json"), cancellationToken);
            if (config == null) {
                return false;
            }

            Config = config;
            return true;
        }

        public Task<bool> SaveConfigurationAsync(CancellationToken cancellationToken = default) {
            return ((IConfiguration) this).SaveFileAsync(Path.Combine(_path, "config.json"), Config, cancellationToken);
        }

        public async Task<T> DeserializeFileAsync<T>(string path, CancellationToken cancellationToken = default) {
            try {
                await using var stream = File.OpenRead(path);
                return await JsonSerializer.DeserializeAsync<T>(stream, JsonSerializerOptions, cancellationToken);
            } catch (Exception ex) {
                Log.Error(ex, "Encountered an error while deserializing {Path}", path);
                return default;
            }
        }

        public async Task<bool> SerializeFileAsync(string path, object value, CancellationToken cancellationToken = default) {
            try {
                await using var stream = File.Open(path, FileMode.Create, FileAccess.Write);
                await JsonSerializer.SerializeAsync(stream, value, JsonSerializerOptions, cancellationToken);
                return true;
            } catch (Exception ex) {
                Log.Error(ex, "Encountered an error while serializing {Path}", path);
                return false;
            }
        }
    }
}