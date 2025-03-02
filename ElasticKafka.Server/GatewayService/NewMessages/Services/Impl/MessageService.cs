using GatewayService.NewMessages.Models;
using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using MessagingService;

namespace GatewayService.NewMessages.Services.Impl;

internal sealed class MessageService : IMessageService
{
    private readonly ILogger<MessageService> _logger;
    private readonly IMessageServiceUrlProvider _urlProvider;

    public MessageService(
        ILogger<MessageService> logger,
        IMessageServiceUrlProvider urlProvider)
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
        var client = new MessageServiceRpc.MessageServiceRpcClient(channel);

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