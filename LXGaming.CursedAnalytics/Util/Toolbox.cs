using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace LXGaming.CursedAnalytics.Util {

    public static class Toolbox {

        public static readonly JsonSerializer JsonSerializer = JsonSerializer.CreateDefault(new JsonSerializerSettings {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Formatting = Formatting.Indented
        });
    }
}