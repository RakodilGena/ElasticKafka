using GatewayService.Messages.Models.Requests;
using GatewayService.Messages.RemoteServiceDiscovery.Messages;
using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using MessagingService;

namespace GatewayService.Messages.Services.Impl;

internal sealed class CreateMessageService : ICreateMessageService
{
    private readonly ILogger<CreateMessageService> _logger;
    private readonly IMessagingServiceUrlProvider _urlProvider;

    public CreateMessageService(
        ILogger<CreateMessageService> logger,
        IMessagingServiceUrlProvider urlProvider)
    {
        _logger = logger;
        _urlProvider = urlProvider;
    }

    public async Task SendMessageAsync(SendMessageRequestDto request)
    {
        var currentAttempt = 0;
        const int maxAttempts = 3;

        while (true)
        {
            currentAttempt++;

            try
            {
                await SendMessageInnerAsync(request);
                return;
            }
            catch (Exception e)
            {
                if (currentAttempt < maxAttempts)
                {
                    _logger.LogError(e,
                        "Failed to send message, attempt: {curAtt}. Retrying...",
                        currentAttempt);

                    continue;
                }

                _logger.LogError(e,
                    "Failed to send message, total attempts: {curAtt}",
                    currentAttempt);

                throw;
            }
        }
    }

    private async Task SendMessageInnerAsync(SendMessageRequestDto request)
    {
        var url = _urlProvider.GetUrl();

        _logger.LogInformation("Sending message to Messaging Service, {url}", url);

        using var channel = GrpcChannel.ForAddress(url);
        var client = new MessagingServiceRpc.MessagingServiceRpcClient(channel);

        var rpcRequest = BuildRequest(request);

        await client.SendMessageAsync(rpcRequest);

        _logger.LogInformation("Message successfully sent to Messaging Service");
    }

    private static SendMessageRequestRpc BuildRequest(SendMessageRequestDto request)
    {
        var id = Guid.CreateVersion7();

        var rpcRequest = new SendMessageRequestRpc
        {
            MessageId = id.ToString(),
            MessageText = request.MessageText,
            SentAt = Timestamp.FromDateTimeOffset(request.SentAt),
        };

        return rpcRequest;
    }
}