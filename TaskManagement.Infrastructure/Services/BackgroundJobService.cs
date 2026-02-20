using System.Linq.Expressions;
using Hangfire;
using TaskManagement.Application.Interfaces;

namespace TaskManagement.Infrastructure.Services;

public class BackgroundJobService : IBackgroundJobService
{
    public string Enqueue(Expression<Action> job) => BackgroundJob.Enqueue(job);

    public string Enqueue<T>(Expression<Action<T>> job) => BackgroundJob.Enqueue(job);

    public string Schedule(Expression<Action> job, TimeSpan delay) =>
        BackgroundJob.Schedule(job, delay);

    public void AddOrUpdateRecurringJob(
        string jobId,
        Expression<Action> job,
        string cronExpression
    ) => RecurringJob.AddOrUpdate(jobId, job, cronExpression);
}
