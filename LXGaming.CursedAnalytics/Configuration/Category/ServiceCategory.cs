using System.Text.Json.Serialization;
using LXGaming.CursedAnalytics.Configuration.Category.Services;

namespace LXGaming.CursedAnalytics.Configuration.Category; 

public class ServiceCategory {

    [JsonPropertyName("curseForge")]
    public CurseForgeCategory CurseForgeCategory { get; init; } = new();
}