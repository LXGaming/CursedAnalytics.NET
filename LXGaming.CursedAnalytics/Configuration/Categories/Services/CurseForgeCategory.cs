using System.Text.Json.Serialization;
using LXGaming.CursedAnalytics.Utilities;

namespace LXGaming.CursedAnalytics.Configuration.Categories.Services; 

public class CurseForgeCategory {
    
    [JsonPropertyName("token")]
    public string Token { get; init; } = "";

    [JsonPropertyName("partnerId")]
    public long PartnerId { get; init; } = 0;

    [JsonPropertyName("contactEmail")]
    public string ContactEmail { get; set; } = Constants.Application.Website;
    
    [JsonPropertyName("jobEnabled")]
    public bool JobEnabled { get; init; } = false;

    [JsonPropertyName("jobSchedule")]
    public string JobSchedule { get; init; } = "0 0 12 * * ?";
}