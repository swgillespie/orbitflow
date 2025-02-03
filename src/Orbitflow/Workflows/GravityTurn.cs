using Orbitflow.Activities;
using Temporalio.Workflows;

namespace Orbitflow.Workflows;

public record GravityTurnInput(double TurnStartAltitude, double TurnEndAltitude, float Heading);
public record GravityTurnOutput();

/**
 * https://en.wikipedia.org/wiki/Gravity_turn
 *
 * Our gravity turn is modeled exactly off of a real-life gravity turn of a rocket ascending or descending a
 * gravitational body. The purpose of the gravity turn is to maintain a low or zero angle of attack throughout
 * ascent (minimizing aerodynamic stress, particularly at Max Q) while also utilizing the planet's gravity to orient the trajectory
 * of the spacecraft into an orbital trajectory.
 *
 * This workflow models a gravity turn as a parameterized curve from t = 0 to t = 1, where t is the progress of
 * the turn. The curve is defined as
 *   angle(t) = 90 - 90t
 * so that angle(t) = 90 at t = 0 and angle(t) = 0 at t = 1.
 *
 * Substituting t = (currentAltitude - turnStartAltitude) / (turnEndAltitude - turnStartAltitude) aka totalAltitude
 * we get
 *   angle(currentAltitude) = 90 - 90((currentAltitude - turnStartAltitude) / totalAltitude)
 *
 * There are other, more sophisticated curves that can minimize max Q, but Kerbal Space Program is pretty forgiving
 * as it comes to aerodynamic pressure so this isn't necessary for our purposes.
 */
[Workflow]
public class GravityTurn
{
    private double turnProgress = 0.0f;
    private double turnAngle = 0.0f;

    [WorkflowRun]
    public async Task<GravityTurnOutput> RunAsync(GravityTurnInput input)
    {
        var totalAltitude = input.TurnEndAltitude - input.TurnStartAltitude;
        while (true)
        {
            var vesselAltitude = await TelemetryActivities.GetSurfaceAltitudeAsync();
            var currentAltitude = Math.Max(vesselAltitude, input.TurnStartAltitude);
            if (currentAltitude > input.TurnEndAltitude)
            {
                turnProgress = 1.0f;
                turnAngle = 90.0f;
                break;
            }

            turnProgress = currentAltitude - input.TurnStartAltitude;
            turnAngle = 90.0f - 90.0f * turnProgress / totalAltitude;
            await VesselControlActivities.SetTargetPitchAndHeadingAsync((float)turnAngle, input.Heading);
            await Workflow.DelayAsync(TimeSpan.FromSeconds(1));
        }

        return new();
    }

    public static async Task<GravityTurnOutput> RunAsChildAsync(GravityTurnInput input)
    {
        return await Workflow.ExecuteChildWorkflowAsync((GravityTurn turn) => turn.RunAsync(input));
    }
}