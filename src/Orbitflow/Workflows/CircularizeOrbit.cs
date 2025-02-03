using Orbitflow.Activities;
using Temporalio.Workflows;

namespace Orbitflow.Workflows;

[Workflow]
public class CircularizeOrbit
{
    [WorkflowRun]
    public async Task RunAsync()
    {
        var burnDv = await MathActivities.CalculateOrbitCircularizationDeltaVAsync();
        var burnDuration = await MathActivities.CalculateBurnTimeAsync(burnDv);
        var now = await TelemetryActivities.GetCurrentUniversalTimeAsync();
        var timeToApoapsis = await TelemetryActivities.GetTimeToApoapsisAsync();
        var apoapsisUT = now + timeToApoapsis;
        var burnTime = apoapsisUT - (burnDuration / 2.0f);

        await VesselControlActivities.AddPlanNodeAsync(burnTime, (float)burnDv, 0.0f, 0.0f);
    }

    public static async Task RunAsChildAsync()
    {
        await Workflow.ExecuteChildWorkflowAsync((CircularizeOrbit circularize) => circularize.RunAsync());
    }
}