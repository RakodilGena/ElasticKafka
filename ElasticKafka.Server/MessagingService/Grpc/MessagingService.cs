using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MessagingService.Grpc.Validation;
using MessagingService.Kafka.Producers.NewMessages;
using MessagingService.Models;

namespace MessagingService.Grpc;

public sealed class MessagingService : MessagingServiceRpc.MessagingServiceRpcBase
{
    private readonly ILogger<MessagingService> _logger;
    private readonly INewMessageProducer _newMessageProducer;

    public MessagingService(
        ILogger<MessagingService> logger,
        INewMessageProducer newMessageProducer)
    {
        _logger = logger;
        _newMessageProducer = newMessageProducer;
    }

    public override async Task<Empty> SendMessage(
        SendMessageRequestRpc request,
        ServerCallContext context)
    {
        try
        {
            request.Validate();

            var messageId = request.MessageId;

            _logger.LogInformation("Sending message {MessageId}", messageId);

            var requestDto = new SendMessageRequestDto(
                Guid.Parse(messageId),
                request.MessageText,
                request.SentAt.ToDateTimeOffset());

            await _newMessageProducer.ProduceAsync(
                requestDto,
                context.CancellationToken);

            return new Empty();
        }
        //todo: move to interceptor?
        catch (OperationCanceledException exception)
        {
            _logger.LogError(exception, "Operation cancelled");

            throw new RpcException(
                new Status(
                    StatusCode.Cancelled,
                    "Operation cancelled",
                    exception));
        }
        catch (Exception exception)
        {
            _logger.LogError(exception,
                "Failed to send message {MessageId}",
                request.MessageId);

            throw new RpcException(
                new Status(
                    StatusCode.Aborted,
                    "MessagingService failed to send message"));
        }
    }
}