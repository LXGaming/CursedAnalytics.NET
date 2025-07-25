using System.Reflection;
using LXGaming.Common.Utilities;

namespace LXGaming.CursedAnalytics.Utilities;

public static class Constants {

    public static class Application {

        public const string Name = "CursedAnalytics";
        public const string Authors = "Alex Thomson";
        public const string Website = "https://lxgaming.me/";

        public static readonly string Version = AssemblyUtils.GetVersion(Assembly.GetExecutingAssembly()) ?? "Unknown";
        public static readonly string UserAgent = $"{Name}/{Version} (+{Website})";
    }
}