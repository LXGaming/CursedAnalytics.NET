using System.Text.Json.Serialization;
using LXGaming.CursedAnalytics.Configuration.Categories;

namespace LXGaming.CursedAnalytics.Configuration;

public class Config {

    [JsonPropertyName("connectionStrings")]
    public Dictionary<string, string> ConnectionStrings { get; init; } = new();
    
    [JsonPropertyName("services")]
    public ServiceCategory ServiceCategory { get; init; } = new();
    
    [JsonPropertyName("quartz")]
    public QuartzCategory QuartzCategory { get; init; } = new();

    [JsonPropertyName("web")]
    public WebCategory WebCategory { get; init; } = new();
}