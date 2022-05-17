using System.Reflection;

namespace LXGaming.CursedAnalytics.Utilities; 

public static class Toolbox {
    
    public static string GetAssembly(string assemblyString, string? packageName = null) {
        return GetAssembly(Assembly.Load(assemblyString), packageName ?? assemblyString);
    }

    public static string GetAssembly(Assembly assembly, string? packageName = null) {
        return $"{packageName ?? GetAssemblyName(assembly) ?? "null"} v{GetAssemblyVersion(assembly)}";
    }

    public static string? GetAssemblyName(Assembly assembly) {
        return assembly.GetName().Name;
    }

    public static string GetAssemblyVersion(Assembly assembly) {
        return (assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
                ?? assembly.GetCustomAttribute<AssemblyVersionAttribute>()?.Version
                ?? "null").Split('+', '-')[0];
    }
}