using System.Text.Json.Serialization;
using LXGaming.CursedAnalytics.Configuration.Category;

namespace LXGaming.CursedAnalytics.Configuration {

    public class Config {

        [JsonPropertyName("general")]
        public GeneralCategory GeneralCategory = new();

        [JsonPropertyName("quartz")]
        public QuartzCategory QuartzCategory = new();

        [JsonPropertyName("storage")]
        public StorageCategory StorageCategory = new();

        [JsonPropertyName("web")]
        public WebCategory WebCategory = new();
    }
}