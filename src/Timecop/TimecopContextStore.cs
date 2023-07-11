using System.Threading;
using System;

namespace TCop;

public class TimecopContextStore
{
    private static readonly AsyncLocal<TimecopContext?> AsyncContext = new();

    public delegate void MutateContextWithCurrentDateTime(ref TimecopContext context, DateTime utcNow);
    public delegate void MutateContext(ref TimecopContext context);

    public void Mutate(MutateContextWithCurrentDateTime mutate)
    {
        var utcNow = DateTime.UtcNow;

        var asyncContext = AsyncContext.Value ?? new TimecopContext();

        mutate(ref asyncContext, utcNow);

        AsyncContext.Value = asyncContext;
    }

    public void Mutate(MutateContext mutate)
    {
        Mutate((ref TimecopContext context, DateTime _) => mutate(ref context));
    }

    public static DateTime AsyncContextUtcNow => AsyncContext.Value?.GetUtcNow(DateTime.UtcNow) ?? DateTime.UtcNow;

    public void ResetContext()
    {
        AsyncContext.Value = null;
    }
}