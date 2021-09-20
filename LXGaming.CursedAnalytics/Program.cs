using System;
using System.Threading.Tasks;

namespace LXGaming.CursedAnalytics {

    public static class Program {

        public static async Task Main() {
            AppDomain.CurrentDomain.ProcessExit += (_, _) => CursedAnalytics.Instance?.Shutdown();
            Console.CancelKeyPress += (_, _) => CursedAnalytics.Instance?.Shutdown();

            await new CursedAnalytics().LoadAsync();
        }
    }
}