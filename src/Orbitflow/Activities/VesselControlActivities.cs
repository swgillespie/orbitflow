using System.Reflection.Metadata.Ecma335;
using KRPC.Client;
using KRPC.Client.Services.SpaceCenter;
using Temporalio.Activities;
using Temporalio.Workflows;

namespace Orbitflow.Activities;

public class VesselControlActivities
{
    private readonly IConnection conn;

    public VesselControlActivities(IConnection conn)
    {
        this.conn = conn;
    }

    [Activity("VesselControl.SetThrottle")]
    public void SetThrottle(float value)
    {
        if (value < 0.0f || value > 1.0f)
        {
            throw new ArgumentOutOfRangeException(nameof(value), "throttle must be between 0 and 1");
        }

        conn.SpaceCenter().ActiveVessel.Control.Throttle = value;
    }

    public static async Task SetThrottleAsync(float value) => await Workflow.ExecuteActivityAsync((VesselControlActivities act) => act.SetThrottle(value), new()
    {
        ScheduleToCloseTimeout = TimeSpan.FromSeconds(10)
    });

    [Activity("VesselControl.Stage")]
    public void Stage() => conn.SpaceCenter().ActiveVessel.Control.ActivateNextStage();

    public static async Task StageAsync() => await Workflow.ExecuteActivityAsync((VesselControlActivities act) => act.Stage(), new()
    {
        ScheduleToCloseTimeout = TimeSpan.FromSeconds(10)
    });

    [Activity("VesselControl.SetSAS")]
    public void SetSAS(bool value) => conn.SpaceCenter().ActiveVessel.Control.SAS = value;

    public static async Task SetSASAsync(bool value) => await Workflow.ExecuteActivityAsync((VesselControlActivities act) => act.SetSAS(value), new()
    {
        ScheduleToCloseTimeout = TimeSpan.FromSeconds(10)
    });

    [Activity("VesselControl.EngageAutopilot")]
    public void EngageAutopilot() => conn.SpaceCenter().ActiveVessel.AutoPilot.Engage();

    public static async Task EngageAutopilotAsync() => await Workflow.ExecuteActivityAsync((VesselControlActivities act) => act.EngageAutopilot(), new()
    {
        ScheduleToCloseTimeout = TimeSpan.FromSeconds(10)
    });

    [Activity("VesselControl.DisengageAutopilot")]
    public void DisengageAutopilot() => conn.SpaceCenter().ActiveVessel.AutoPilot.Disengage();

    public static async Task DisengageAutopilotAsync() => await Workflow.ExecuteActivityAsync((VesselControlActivities act) => act.DisengageAutopilot(), new()
    {
        ScheduleToCloseTimeout = TimeSpan.FromSeconds(10)
    });

    [Activity("VesselControl.SetTargetPitchAndHeading")]
    public void SetTargetPitchAndHeading(float pitch, float heading)
    {
        conn.SpaceCenter().ActiveVessel.AutoPilot.TargetPitchAndHeading(pitch, heading);
    }

    public static async Task SetTargetPitchAndHeadingAsync(float pitch, float heading) => await Workflow.ExecuteActivityAsync((VesselControlActivities act) => act.SetTargetPitchAndHeading(pitch, heading), new()
    {
        ScheduleToCloseTimeout = TimeSpan.FromSeconds(10)
    });

    [Activity("VesselControl.AddPlanNode")]
    public void AddPlanNode(double time, float prograde, float normal, float radial)
    {
        var vessel = conn.SpaceCenter().ActiveVessel;
        vessel.Control.AddNode(time, prograde, normal, radial);
    }

    public static async Task AddPlanNodeAsync(double time, float prograde, float normal, float radial) => await Workflow.ExecuteActivityAsync((VesselControlActivities act) => act.AddPlanNode(time, prograde, normal, radial), new()
    {
        ScheduleToCloseTimeout = TimeSpan.FromSeconds(10)
    });
}