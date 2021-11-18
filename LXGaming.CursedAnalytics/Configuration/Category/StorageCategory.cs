using System.Text.Json.Serialization;

namespace LXGaming.CursedAnalytics.Configuration.Category;

public class StorageCategory {

    [JsonPropertyName("engine")]
    public string Engine = "mysql";

    [JsonPropertyName("host")]
    public string Host = "localhost";

    [JsonPropertyName("port")]
    public uint Port = 3306;

    [JsonPropertyName("database")]
    public string Database = "curse_analytics";

    [JsonPropertyName("username")]
    public string Username = "curse_analytics";

    [JsonPropertyName("password")]
    public string Password = "password";

    [JsonPropertyName("maximumPoolSize")]
    public uint MaximumPoolSize = 2;

    [JsonPropertyName("minimumPoolSize")]
    public uint MinimumPoolSize = 1;
}