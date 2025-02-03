using Orbitflow.Activities;
using Temporalio.Workflows;

namespace Orbitflow.Workflows.Vessels;

/**
 * Icarus I is a simple solid fuel single stage rocket that shoots straight up. It's the first rocket that you can
 * launch in Kerbal Space Program's science mode.
 *
 * Parachute descent is handled manually once the autopilot disengages.
 */
[Workflow]
public class IcarusI
{
    [WorkflowRun]
    public async Task RunAsync()
    {
        await VesselControlActivities.EngageAutopilotAsync();
        await Liftoff.RunAsChildAsync(new(90.0f)); // due east
        await VesselControlActivities.DisengageAutopilotAsync();
    }
}