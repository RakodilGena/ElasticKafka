using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MessagingService.Kafka.Producers;
using MessagingService.Models;

namespace MessagingService.Grpc;

public sealed class MessageService : MessageServiceRpc.MessageServiceRpcBase
{
    private readonly ILogger<MessageService> _logger;
    private readonly IMessageProducer _messageProducer;

    public MessageService(
        ILogger<MessageService> logger, 
        IMessageProducer messageProducer)
    {
        _logger = logger;
        _messageProducer = messageProducer;
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
        
        await _messageProducer.ProduceAsync(requestDto);
        
        return new Empty();
    }
}