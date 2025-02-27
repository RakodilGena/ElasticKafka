using Microsoft.AspNetCore.SignalR;

namespace GatewayService.SignalR;

internal sealed class MessagesHub : Hub
{
    private readonly ILogger<MessagesHub> _logger;

    public MessagesHub(ILogger<MessagesHub> logger)
    {
        _logger = logger;
    }

    public async Task JoinGroup(string groupId)
    {
        var connectionId = Context.ConnectionId;

        await Groups.AddToGroupAsync(connectionId, groupId);

        _logger.LogInformation(
            "User [{connId}] joined group [{groupId}]",
            connectionId,
            groupId);
    }
}