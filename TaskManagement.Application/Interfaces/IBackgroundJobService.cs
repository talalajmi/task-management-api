using System.Linq.Expressions;

namespace TaskManagement.Application.Interfaces;

public interface IBackgroundJobService
{
    // Fire and forget — runs once immediately in background
    string Enqueue(Expression<Action> job);
    string Enqueue<T>(Expression<Action<T>> job);

    // Scheduled — runs once after a delay
    string Schedule(Expression<Action> job, TimeSpan delay);

    // Recurring — runs on a cron schedule
    void AddOrUpdateRecurringJob(string jobId, Expression<Action> job, string cronExpression);
}
