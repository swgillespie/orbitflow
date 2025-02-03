using Orbitflow.Activities;
using Temporalio.Workflows;

namespace Orbitflow.Workflows.Vessels;

[Workflow]
public class IcarusII
{
    [WorkflowRun]
    public async Task RunAsync()
    {
        var heading = 90.0f; // due east
        await VesselControlActivities.EngageAutopilotAsync();
        await Liftoff.RunAsChildAsync(new(90.0f)); // due east
        await GravityTurn.RunAsChildAsync(new(TurnStartAltitude: 500.0f, TurnEndAltitude: 50000.0f, Heading: heading));
        await VesselControlActivities.DisengageAutopilotAsync();
    }
}