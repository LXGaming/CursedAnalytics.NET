using System.Text.Json.Serialization;

namespace LXGaming.CursedAnalytics.Configuration.Category {

    public class WebCategory {

        public const int DefaultTimeout = 100000; // 100 Seconds

        [JsonPropertyName("timeout")]
        public int Timeout = DefaultTimeout;
    }
}