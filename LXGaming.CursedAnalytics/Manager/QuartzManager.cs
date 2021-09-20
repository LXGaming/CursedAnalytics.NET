using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using LXGaming.CursedAnalytics.Configuration.Category;
using Quartz;
using Quartz.Impl;
using Serilog;

namespace LXGaming.CursedAnalytics.Manager {

    public static class QuartzManager {

        public static QuartzCategory QuartzCategory => CursedAnalytics.Instance.Config?.QuartzCategory;
        public static IScheduler Scheduler { get; private set; }
        private static readonly NameValueCollection Collection = new();

        public static void Prepare() {
            if (QuartzCategory.MaxConcurrency <= 0) {
                Log.Warning("MaxConcurrency is out of bounds. Resetting to {Value}", QuartzCategory.DefaultMaxConcurrency);
                QuartzCategory.MaxConcurrency = QuartzCategory.DefaultMaxConcurrency;
            }

            if (QuartzCategory.ShutdownTimeout < -1) {
                Log.Warning("ShutdownTimeout is out of bounds. Resetting to {Value}", QuartzCategory.DefaultShutdownTimeout);
                QuartzCategory.ShutdownTimeout = QuartzCategory.DefaultShutdownTimeout;
            }

            Collection.Set("quartz.threadPool.maxConcurrency", $"{QuartzCategory.MaxConcurrency}");
        }

        public static async Task ExecuteAsync() {
            var factory = new StdSchedulerFactory(Collection);
            Scheduler = await factory.GetScheduler();
            await Scheduler.Start();
        }

        public static Task ScheduleJobAsync<T>(IDictionary<string, object> dictionary = null) where T : IJob {
            var trigger = TriggerBuilder.Create().StartNow().Build();
            return ScheduleJobAsync<T>(trigger, dictionary);
        }

        public static Task<DateTimeOffset> ScheduleJobAsync<T>(ITrigger trigger, IDictionary<string, object> dictionary = null) where T : IJob {
            var jobKey = JobKey.Create(Guid.NewGuid().ToString());
            return ScheduleJobAsync<T>(jobKey, trigger, dictionary);
        }

        public static Task<DateTimeOffset> ScheduleJobAsync<T>(JobKey key, ITrigger trigger, IDictionary<string, object> dictionary = null) where T : IJob {
            var jobDetail = JobBuilder.Create<T>().WithIdentity(key).Build();
            return ScheduleJobAsync(jobDetail, trigger, dictionary);
        }

        public static Task<DateTimeOffset> ScheduleJobAsync(IJobDetail jobDetail, ITrigger trigger, IDictionary<string, object> dictionary = null) {
            if (dictionary != null) {
                Merge(jobDetail, dictionary);
            }

            return Scheduler.ScheduleJob(jobDetail, trigger);
        }

        public static void Shutdown() {
            var timeout = QuartzCategory?.ShutdownTimeout ?? QuartzCategory.DefaultShutdownTimeout;
            Scheduler?.Shutdown().Wait(timeout);
        }

        private static void Merge(IJobDetail jobDetail, IDictionary<string, object> dictionary) {
            foreach (var (key, value) in dictionary) {
                if (jobDetail.JobDataMap.TryAdd(key, value)) {
                    continue;
                }

                Log.Warning("Duplicate {Key} JobData for {Name}", jobDetail.Key.ToString(), jobDetail.JobType.Name);
            }
        }
    }
}