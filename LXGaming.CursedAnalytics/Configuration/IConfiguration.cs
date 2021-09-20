using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Serilog;

namespace LXGaming.CursedAnalytics.Configuration {

    public interface IConfiguration {

        Config Config { get; }

        Task<bool> LoadConfigurationAsync(CancellationToken cancellationToken = default);

        Task<bool> SaveConfigurationAsync(CancellationToken cancellationToken = default);

        Task<T> LoadFileAsync<T>(string path, CancellationToken cancellationToken = default) {
            if (File.Exists(path)) {
                return DeserializeFileAsync<T>(path, cancellationToken);
            }

            return Task.FromResult(Activator.CreateInstance<T>());
        }

        Task<bool> SaveFileAsync(string path, object value, CancellationToken cancellationToken = default) {
            if (File.Exists(path) || CreateDirectory(path)) {
                return SerializeFileAsync(path, value, cancellationToken);
            }

            return Task.FromResult(false);
        }

        Task<T> DeserializeFileAsync<T>(string path, CancellationToken cancellationToken = default);

        Task<bool> SerializeFileAsync(string path, object value, CancellationToken cancellationToken = default);

        static bool CreateDirectory(string path) {
            try {
                Directory.CreateDirectory(Path.GetDirectoryName(path) ?? path);
                return true;
            } catch (Exception ex) {
                Log.Error(ex, "Encountered an error while creating {Path}", path);
                return false;
            }
        }
    }
}