using KRPC.Client;
using KRPC.Client.Services.KRPC;
using KRPC.Client.Services.SpaceCenter;
using Temporalio.Activities;
using Temporalio.Workflows;

namespace Orbitflow.Activities;

public class TelemetryActivities
{
    private readonly IConnection conn;

    public TelemetryActivities(IConnection conn)
    {
        this.conn = conn;
    }

    [Activity("Telemetry.GetSurfaceAltitude")]
    public double GetSurfaceAltitude() => conn.SpaceCenter().ActiveVessel.Flight().SurfaceAltitude;

    public static async Task<double> GetSurfaceAltitudeAsync() => await Workflow.ExecuteActivityAsync((TelemetryActivities act) => act.GetSurfaceAltitude(), new()
    {
        ScheduleToCloseTimeout = TimeSpan.FromSeconds(10)
    });

    [Activity("Telemetry.WaitForFuelLevel")]
    public void WaitForFuelLevel(string type, float level)
    {
        using var heartbeat = new Heartbeater();
        var vessel = conn.SpaceCenter().ActiveVessel;

        // Important note about the stage parameter here:
        // kRPC distinguishes between "stages" and "decouple stages". A "decouple stage" of a part is the stage in which
        // that part is decoupled from the rest of the vessel. In the case of boosters, it's often the stage that is
        // immediately following the current stage (hence the - 1).
        var resources = vessel.ResourcesInDecoupleStage(vessel.Control.CurrentStage - 1, false);
        var call = Connection.GetCall(() => resources.Amount(type));
        var expr = Expression.LessThanOrEqual(conn, Expression.Call(conn, call), Expression.ConstantFloat(conn, level));
        var evt = conn.KRPC().AddEvent(expr);
        lock (evt.Condition)
        {
            evt.Wait();
        }
    }

    public static async Task WaitForFuelLevelAsync(string type, float level) => await Workflow.ExecuteActivityAsync((TelemetryActivities act) => act.WaitForFuelLevel(type, level), new()
    {
        ScheduleToCloseTimeout = TimeSpan.FromMinutes(10)
    });

    [Activity("Telemetry.WaitForAtmosphericDensity")]
    public void WaitForAtmosphericDensity(float density)
    {
        using var heartbeat = new Heartbeater();
        var vessel = conn.SpaceCenter().ActiveVessel;
        var call = Connection.GetCall(() => vessel.Flight(vessel.ReferenceFrame).AtmosphereDensity);
        var expr = Expression.LessThanOrEqual(conn,
            Expression.Call(conn, call),
            Expression.ConstantFloat(conn, density)
        );

        var evt = conn.KRPC().AddEvent(expr);
        lock (evt.Condition)
        {
            evt.Wait();
        }
    }

    public static async Task WaitForAtmosphericDensityAsync(float density) => await Workflow.ExecuteActivityAsync((TelemetryActivities act) => act.WaitForAtmosphericDensity(density), new()
    {
        ScheduleToCloseTimeout = TimeSpan.FromMinutes(10)
    });

    [Activity("Telemetry.WaitForApoapsis")]
    public void WaitForApoapsis(double apoapsisAltitude)
    {
        using var heartbeat = new Heartbeater();
        var vessel = conn.SpaceCenter().ActiveVessel;
        var call = Connection.GetCall(() => vessel.Orbit.ApoapsisAltitude);
        var expr = Expression.GreaterThanOrEqual(conn,
            Expression.Call(conn, call),
            Expression.ConstantDouble(conn, apoapsisAltitude)
        );

        var evt = conn.KRPC().AddEvent(expr);
        lock (evt.Condition)
        {
            evt.Wait();
        }
    }

    public static async Task WaitForApoapsisAsync(double apoapsisAltitude) => await Workflow.ExecuteActivityAsync((TelemetryActivities act) => act.WaitForApoapsis(apoapsisAltitude), new()
    {
        ScheduleToCloseTimeout = TimeSpan.FromMinutes(10)
    });

    [Activity("Telemetry.GetCurrentUniversalTime")]
    public double GetCurrentUniversalTime() => conn.SpaceCenter().UT;

    public static async Task<double> GetCurrentUniversalTimeAsync() => await Workflow.ExecuteActivityAsync((TelemetryActivities act) => act.GetCurrentUniversalTime(), new()
    {
        ScheduleToCloseTimeout = TimeSpan.FromSeconds(10)
    });

    [Activity("Telemetry.GetTimeToApoapsis")]
    public double GetTimeToApoapsis() => conn.SpaceCenter().ActiveVessel.Orbit.TimeToApoapsis;


    public static async Task<double> GetTimeToApoapsisAsync() => await Workflow.ExecuteActivityAsync((TelemetryActivities act) => act.GetTimeToApoapsis(), new()
    {
        ScheduleToCloseTimeout = TimeSpan.FromSeconds(10)
    });
}