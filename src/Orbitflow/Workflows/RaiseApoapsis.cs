using Orbitflow.Activities;
using Temporalio.Workflows;

namespace Orbitflow.Workflows;

public record RaiseApoapsisInput(double TargetApoapsis);
public record RaiseApoapsisOutput;

[Workflow]
public class RaiseApoapsis
{
    [WorkflowRun]
    public async Task<RaiseApoapsisOutput> RunAsync(RaiseApoapsisInput input)
    {
        // It's more efficient to burn in vacuum, so we'll kill the engine until we're there.
        await VesselControlActivities.SetThrottleAsync(0.0f);
        await TelemetryActivities.WaitForAtmosphericDensityAsync(0.0f);
        await VesselControlActivities.SetThrottleAsync(1.0f);
        await TelemetryActivities.WaitForApoapsisAsync(input.TargetApoapsis);
        await VesselControlActivities.SetThrottleAsync(0.0f);
        return new();
    }

    public static async Task<RaiseApoapsisOutput> RunAsChildAsync(RaiseApoapsisInput input)
    {
        return await Workflow.ExecuteChildWorkflowAsync((RaiseApoapsis raise) => raise.RunAsync(input));
    }
}
