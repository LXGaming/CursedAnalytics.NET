using System.Text.Json.Serialization;

namespace LXGaming.CursedAnalytics.Configuration.Category {

    public class GeneralCategory {

        [JsonPropertyName("debug")]
        public bool Debug;

        [JsonPropertyName("schedule")]
        public string Schedule = "0 0 12 * * ?";
    }
}