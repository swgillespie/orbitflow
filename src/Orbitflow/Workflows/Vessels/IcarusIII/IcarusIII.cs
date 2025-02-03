using Orbitflow.Activities;
using Temporalio.Workflows;

namespace Orbitflow.Workflows.Vessels.IcarusIII;

[Workflow]
public class IcarusIII
{
    [WorkflowRun]
    public async Task RunAsync()
    {
        var heading = 90.0f; // due east
        await VesselControlActivities.EngageAutopilotAsync();
        await Liftoff.RunAsChildAsync(new(90.0f)); // due east
        await Task.WhenAll(
            GravityTurn.RunAsChildAsync(new(TurnStartAltitude: 500.0f, TurnEndAltitude: 50000.0f, Heading: heading)),
            IcarusIIIStaging.RunAsChildAsync()
        );
        await VesselControlActivities.DisengageAutopilotAsync();
    }
}