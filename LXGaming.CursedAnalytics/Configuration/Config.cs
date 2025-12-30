using System.Text.Json.Serialization;
using LXGaming.CursedAnalytics.Configuration.Categories;

namespace LXGaming.CursedAnalytics.Configuration;

public class Config {

    [JsonPropertyName("connectionStrings")]
    public Dictionary<string, string> ConnectionStrings { get; init; } = new() {
        { "MySql", "server=127.0.0.1;port=3306;database=curse_analytics;userid=curse_analytics;password=password;pooling=true;maximumpoolsize=2;minimumpoolsize=1;" }
    };

    [JsonPropertyName("services")]
    public ServiceCategory ServiceCategory { get; init; } = new();

    [JsonPropertyName("quartz")]
    public QuartzCategory QuartzCategory { get; init; } = new();

    [JsonPropertyName("web")]
    public WebCategory WebCategory { get; init; } = new();
}