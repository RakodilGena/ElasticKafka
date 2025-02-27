using Microsoft.AspNetCore.SignalR;

namespace GatewayService.SignalR;

internal sealed class MessageCreatedEventsHubWrapper : IMessageCreatedEventsHubWrapper
{
    private const string MessageCreatedEventReceivers = "MessageCreatedEventReceivers";

    private readonly IHubContext<MessagesHub> _hubContext;

    public MessageCreatedEventsHubWrapper(IHubContext<MessagesHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task NotifyMessageCreated(
        Guid messageId,
        CancellationToken cancellationToken)
    {
        await _hubContext.Clients.Group(MessageCreatedEventReceivers)
            .SendAsync(
                "MessageCreated",
                messageId.ToString(),
                cancellationToken);
    }
}