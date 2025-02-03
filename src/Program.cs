using KRPC.Client;
using Orbitflow.Activities;
using Orbitflow.Workflows;
using Orbitflow.Workflows.Vessels;
using Orbitflow.Workflows.Vessels.IcarusIII;
using Orbitflow.Workflows.Vessels.IcarusIV;
using Temporalio.Client;
using Temporalio.Worker;

var client = await TemporalClient.ConnectAsync(new("localhost:7233"));

using var tokenSource = new CancellationTokenSource();
Console.CancelKeyPress += (_, eventArgs) =>
{
    tokenSource.Cancel();
    eventArgs.Cancel = true;
};

using var connection = new Connection("orbitflow");
using var worker = new TemporalWorker(client, new TemporalWorkerOptions("orbitflow")
    .AddAllActivities(new VesselControlActivities(connection))
    .AddAllActivities(new UserInterfaceActivities(connection))
    .AddAllActivities(new TelemetryActivities(connection))
    .AddAllActivities(new MathActivities(connection))
    .AddWorkflow<Liftoff>()
    .AddWorkflow<GravityTurn>()
    .AddWorkflow<IcarusI>()
    .AddWorkflow<IcarusII>()
    .AddWorkflow<IcarusIII>()
    .AddWorkflow<IcarusIIIStaging>()
    .AddWorkflow<IcarusIV>()
    .AddWorkflow<IcarusIVStaging>()
    .AddWorkflow<RaiseApoapsis>()
    .AddWorkflow<CircularizeOrbit>()
);
Console.WriteLine("starting worker");
try
{
    await worker.ExecuteAsync(tokenSource.Token);
}
catch (OperationCanceledException)
{
    Console.WriteLine("Worker cancelled");
}
