using System.Text.Json.Serialization;
using LXGaming.CursedAnalytics.Configuration.Categories.Services;

namespace LXGaming.CursedAnalytics.Configuration.Categories;

public class ServiceCategory {

    [JsonPropertyName("curseForge")]
    public CurseForgeCategory CurseForgeCategory { get; init; } = new();
}