using Orbitflow.Activities;
using Temporalio.Workflows;

namespace Orbitflow.Workflows.Vessels.IcarusIV;

[Workflow]
public class IcarusIV
{
    [WorkflowRun]
    public async Task RunAsync()
    {
        var heading = 90.0f; // due east
        await VesselControlActivities.EngageAutopilotAsync();
        await Liftoff.RunAsChildAsync(new(90.0f)); // due east
        await Task.WhenAll(
            Ascend(heading),
            IcarusIVStaging.RunAsChildAsync()
        );
        await VesselControlActivities.DisengageAutopilotAsync();
    }

    private static async Task Ascend(float heading)
    {
        await GravityTurn.RunAsChildAsync(new(TurnStartAltitude: 500.0f, TurnEndAltitude: 50000.0f, Heading: heading));
        await RaiseApoapsis.RunAsChildAsync(new(180000.0f));
        //await CircularizeOrbit.RunAsChildAsync();
    }
}