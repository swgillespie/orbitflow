using Orbitflow.Activities;
using Temporalio.Workflows;

namespace Orbitflow.Workflows.Vessels.IcarusIII;

[Workflow]
public class IcarusIIIStaging
{
    [WorkflowRun]
    public async Task RunAsync()
    {
        await TelemetryActivities.WaitForFuelLevelAsync("SolidFuel", 0.0f);
        // Separate the first stage SSBs off to the side
        await VesselControlActivities.StageAsync();
        // Ignite the main engine
        await VesselControlActivities.StageAsync();
    }

    public static async Task RunAsChildAsync()
    {
        await Workflow.ExecuteChildWorkflowAsync((IcarusIIIStaging staging) => staging.RunAsync());
    }
}