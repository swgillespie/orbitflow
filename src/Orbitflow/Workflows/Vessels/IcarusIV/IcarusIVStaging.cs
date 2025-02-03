using Orbitflow.Activities;
using Temporalio.Workflows;

namespace Orbitflow.Workflows.Vessels.IcarusIV;

[Workflow]
public class IcarusIVStaging
{
    [WorkflowRun]
    public async Task RunAsync()
    {
        await TelemetryActivities.WaitForFuelLevelAsync("SolidFuel", 0.0f);
        // Separate the first stage SSBs off to the side
        await VesselControlActivities.StageAsync();
        // Ignite the Secondary liquid fuel engine
        await VesselControlActivities.StageAsync();
        await TelemetryActivities.WaitForFuelLevelAsync("LiquidFuel", 0.0f);
        // Separate the secondary from the primary liquid fuel engine
        await VesselControlActivities.StageAsync();
        // Ignite the primary engine
        await VesselControlActivities.StageAsync();
    }

    public static async Task RunAsChildAsync()
    {
        await Workflow.ExecuteChildWorkflowAsync((IcarusIVStaging staging) => staging.RunAsync());
    }
}