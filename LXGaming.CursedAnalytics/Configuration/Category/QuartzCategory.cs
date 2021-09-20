using System.Text.Json.Serialization;

namespace LXGaming.CursedAnalytics.Configuration.Category {

    public class QuartzCategory {

        public const int DefaultMaxConcurrency = 2;
        public const int DefaultShutdownTimeout = 15000; // 15 Seconds

        [JsonPropertyName("maxConcurrency")]
        public int MaxConcurrency = DefaultMaxConcurrency;

        [JsonPropertyName("shutdownTimeout")]
        public int ShutdownTimeout = DefaultShutdownTimeout;
    }
}