using Orbitflow.Activities;
using Temporalio.Workflows;

namespace Orbitflow.Workflows;

public record LiftoffInput(float Heading);
public record LiftoffOutput;

[Workflow]
public class Liftoff
{
    [WorkflowRun]
    public async Task<LiftoffOutput> RunAsync(LiftoffInput input)
    {
        await VesselControlActivities.SetSASAsync(true);
        await VesselControlActivities.SetTargetPitchAndHeadingAsync(90.0f, input.Heading);
        for (int i = 5; i > 0; i--)
        {
            await UserInterfaceActivities.DisplayMessageAsync(new($"Liftoff in {i}..."));
            await Workflow.DelayAsync(TimeSpan.FromSeconds(1));
        }

        await UserInterfaceActivities.DisplayMessageAsync(new("Liftoff!"));
        await VesselControlActivities.SetThrottleAsync(1.0f);
        await VesselControlActivities.StageAsync();
        return new();
    }

    public static async Task<LiftoffOutput> RunAsChildAsync(LiftoffInput input)
    {
        return await Workflow.ExecuteChildWorkflowAsync((Liftoff lift) => lift.RunAsync(input));
    }
}