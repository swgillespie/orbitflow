using KRPC.Client;
using KRPC.Client.Services.SpaceCenter;
using Temporalio.Activities;
using Temporalio.Workflows;

namespace Orbitflow.Activities;

public class MathActivities
{
    private IConnection conn;

    public MathActivities(IConnection conn)
    {
        this.conn = conn;
    }

    [Activity("Math.CalculateOrbitCircularizationDeltaV")]
    public double CalculateOrbitCircularizationDeltaV()
    {
        var orbit = conn.SpaceCenter().ActiveVessel.Orbit;
        return OrbitMath.OrbitCircularizationCost(orbit);
    }

    public static async Task<double> CalculateOrbitCircularizationDeltaVAsync() => await Workflow.ExecuteActivityAsync((MathActivities act) => act.CalculateOrbitCircularizationDeltaV(), new()
    {
        ScheduleToCloseTimeout = TimeSpan.FromSeconds(10)
    });

    [Activity("Math.CalculateBurnTime")]
    public double CalculateBurnTime(double deltaV)
    {
        var vessel = conn.SpaceCenter().ActiveVessel;
        return OrbitMath.BurnTime(vessel, deltaV);
    }

    public static async Task<double> CalculateBurnTimeAsync(double deltaV) => await Workflow.ExecuteActivityAsync((MathActivities act) => act.CalculateBurnTime(deltaV), new()
    {
        ScheduleToCloseTimeout = TimeSpan.FromSeconds(10)
    });
}