using System.IO;
using System.Threading;
using System.Threading.Tasks;
using LXGaming.CursedAnalytics.Configuration;
using LXGaming.CursedAnalytics.Job;
using LXGaming.CursedAnalytics.Manager;
using LXGaming.CursedAnalytics.Storage;
using Quartz;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace LXGaming.CursedAnalytics {

    public class CursedAnalytics {

        public const string Id = "cursed_analytics";
        public const string Name = "Cursed Analytics";
        public const string Version = "1.0.2";
        public const string Authors = "LX_Gaming";
        public const string Website = "https://lxgaming.github.io/";
        public const string UserAgent = Name + "/" + Version + " (+" + Website + ")";

        public static CursedAnalytics Instance { get; private set; }
        public readonly IConfiguration Configuration;
        private readonly ManualResetEvent _state;
        private readonly LoggingLevelSwitch _loggingLevelSwitch;

        public Config Config => Configuration.Config;

        public CursedAnalytics() {
            Instance = this;
            Configuration = new JsonConfiguration(Directory.GetCurrentDirectory());
            _state = new ManualResetEvent(false);
            _loggingLevelSwitch = new LoggingLevelSwitch();
        }

        public async Task LoadAsync() {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Quartz", LogEventLevel.Information)
                .WriteTo.Console(
                    levelSwitch: _loggingLevelSwitch)
                .CreateLogger();

            Log.Information("Initializing...");
            if (!await ReloadAsync()) {
                Log.Error("Failed to load");
                return;
            }

            QuartzManager.Prepare();
            WebManager.Prepare();

            await Configuration.SaveConfigurationAsync();

            await QuartzManager.ExecuteAsync();
            await QuartzManager.ScheduleJobAsync<AnalyticJob>(AnalyticJob.JobKey, TriggerBuilder.Create().WithCronSchedule(Config.GeneralCategory.Schedule).Build());

            Log.Information("{Name} v{Version} has loaded", Name, Version);
            _state.WaitOne();
        }

        public async Task<bool> ReloadAsync() {
            if (!await Configuration.LoadConfigurationAsync()) {
                return false;
            }

            await Configuration.SaveConfigurationAsync();
            ReloadLogger();

            await using var storage = StorageContext.Create();
            if (!await storage.Database.CanConnectAsync()) {
                Log.Error("Connection failed");
                return false;
            }

            if (await storage.Database.EnsureCreatedAsync()) {
                Log.Debug("Created Tables");
            }

            return true;
        }

        public void ReloadLogger() {
            if (Config.GeneralCategory.Debug) {
                _loggingLevelSwitch.MinimumLevel = LogEventLevel.Debug;
                Log.Debug("Debug mode enabled");
            } else {
                _loggingLevelSwitch.MinimumLevel = LogEventLevel.Information;
                Log.Information("Debug mode disabled");
            }
        }

        public void Shutdown() {
            if (_state.WaitOne(0) || !_state.Set()) {
                return;
            }

            Log.Information("Shutting down...");

            WebManager.Shutdown();
            QuartzManager.Shutdown();

            Log.CloseAndFlush();
        }
    }
}