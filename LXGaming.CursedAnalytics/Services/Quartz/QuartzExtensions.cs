using Quartz;
using Serilog;

namespace LXGaming.CursedAnalytics.Services.Quartz; 

public static class QuartzExtensions {

    public static T TryGetOrCreateValue<T>(this IJobExecutionContext context, string key) where T : new() {
        if (context.JobDetail.JobDataMap.TryGetValue(key, out var existingValue)) {
            return (T) existingValue;
        }

        var value = new T();
        context.JobDetail.JobDataMap.Put(key, value);
        return value;
    }

    public static Task<DateTimeOffset> ScheduleJobAsync<T>(this IScheduler scheduler, ITrigger trigger, IDictionary<string, object>? dictionary = null) where T : IJob {
        var key = JobKey.Create(Guid.NewGuid().ToString());
        return scheduler.ScheduleJobAsync<T>(key, trigger, dictionary);
    }

    public static Task<DateTimeOffset> ScheduleJobAsync<T>(this IScheduler scheduler, JobKey key, ITrigger trigger, IDictionary<string, object>? dictionary = null) where T : IJob {
        var jobDetail = JobBuilder.Create<T>().WithIdentity(key).Build();
        return scheduler.ScheduleJobAsync(jobDetail, trigger, dictionary);
    }

    public static Task<DateTimeOffset> ScheduleJobAsync(this IScheduler scheduler, IJobDetail jobDetail, ITrigger trigger, IDictionary<string, object>? dictionary = null) {
        if (dictionary != null) {
            Merge(jobDetail, dictionary);
        }

        return scheduler.ScheduleJob(jobDetail, trigger);
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