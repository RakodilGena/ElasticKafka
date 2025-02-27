using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MessagingService.Kafka.Producers.NewMessages;
using MessagingService.Models;

namespace MessagingService.Grpc;

public sealed class MessageService : MessageServiceRpc.MessageServiceRpcBase
{
    private readonly ILogger<MessageService> _logger;
    private readonly INewMessageProducer _newMessageProducer;

    public MessageService(
        ILogger<MessageService> logger, 
        INewMessageProducer newMessageProducer)
    {
        _logger = logger;
        _newMessageProducer = newMessageProducer;
    }

    public override async Task<Empty> SendMessage(
        SendMessageRequestRpc request, 
        ServerCallContext context)
    {
        var messageId = Guid.CreateVersion7();
        
        _logger.LogInformation("Sending message {MessageId}", messageId);
        
        var requestDto = new SendMessageRequestDto(
            messageId,
            request.MessageText);
        
        await _newMessageProducer.ProduceAsync(requestDto);
        
        return new Empty();
    }
}