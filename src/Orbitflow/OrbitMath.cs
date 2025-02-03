using KRPC.Client.Services.SpaceCenter;

namespace Orbitflow;
public static class OrbitMath
{
    /**
     * https://en.wikipedia.org/wiki/Vis-viva_equation
     * 
     * Calculates the delta-V required to circularize an orbit by making its semi-major axis equal to its apoapsis.
     */
    public static double OrbitCircularizationCost(Orbit orbit)
    {
        var mu = orbit.Body.GravitationalParameter;
        var r = orbit.Apoapsis;
        var a1 = orbit.SemiMajorAxis;
        var v1 = Math.Sqrt(mu * (2.0 / r - 1.0 / a1));
        var v2 = Math.Sqrt(mu * (2.0 / r - 1.0 / r));
        return v2 - v1;
    }

    /**
     * https://en.wikipedia.org/wiki/Tsiolkovsky_rocket_equation
     *
     * Calcualtes the length of the burn required for the vessel to achieve the given delta-V.
     */
    public static double BurnTime(Vessel vessel, double deltaV)
    {
        var force = vessel.AvailableThrust;
        var isp = vessel.SpecificImpulse * vessel.Orbit.Body.SurfaceGravity;
        var m0 = vessel.Mass;
        var m1 = m0 / Math.Exp(deltaV / isp);
        var flowRate = force / isp;
        return (m0 - m1) / flowRate;
    }
}