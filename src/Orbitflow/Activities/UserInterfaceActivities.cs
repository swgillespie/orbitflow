using KRPC.Client;
using KRPC.Client.Services.UI;
using Temporalio.Activities;
using Temporalio.Workflows;

namespace Orbitflow.Activities;

public record DisplayMessageInput(string Message);

public record DisplayMessageOutput;

public class UserInterfaceActivities
{
    private readonly IConnection conn;

    public UserInterfaceActivities(IConnection conn)
    {
        this.conn = conn;
    }

    [Activity("UserInterface.DisplayMessage")]
    public DisplayMessageOutput DisplayMessage(DisplayMessageInput input)
    {
        conn.UI().Message(input.Message, 1.0f, MessagePosition.TopCenter, new(255.0, 255.0, 0.0), 20.0f);
        return new();
    }

    public static async Task<DisplayMessageOutput> DisplayMessageAsync(DisplayMessageInput input)
    {
        return await Workflow.ExecuteActivityAsync((UserInterfaceActivities act) => act.DisplayMessage(input), new()
        {
            ScheduleToCloseTimeout = TimeSpan.FromSeconds(10)
        });
    }
}