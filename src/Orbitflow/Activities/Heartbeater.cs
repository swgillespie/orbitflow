using Temporalio.Activities;

namespace Orbitflow.Activities;

public class Heartbeater : IDisposable
{
    private readonly System.Timers.Timer timer;

    public Heartbeater(int intervalSeconds = 1)
    {
        this.timer = new System.Timers.Timer(intervalSeconds * 1000)
        {
            AutoReset = true,
            Enabled = true
        };
        timer.Elapsed += (_, _) => ActivityExecutionContext.Current.Heartbeat();
    }

    public void Dispose() => timer.Dispose();
}