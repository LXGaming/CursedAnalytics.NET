using Serilog.Core;
using Serilog.Events;

namespace LXGaming.CursedAnalytics.Utilities.Serilog; 

public class EnvironmentLoggingLevelSwitch : LoggingLevelSwitch {

    public EnvironmentLoggingLevelSwitch(LogEventLevel developmentLevel = LogEventLevel.Debug, LogEventLevel productionLevel = LogEventLevel.Information) {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
        MinimumLevel = string.Equals(environment, "Development", StringComparison.OrdinalIgnoreCase) ? developmentLevel : productionLevel;
    }
}