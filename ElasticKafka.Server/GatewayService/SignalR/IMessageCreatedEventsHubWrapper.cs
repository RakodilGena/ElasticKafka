namespace GatewayService.SignalR;

public interface IMessageCreatedEventsHubWrapper
{
    public Task NotifyMessageCreated(Guid messageId, CancellationToken cancellationToken);
}