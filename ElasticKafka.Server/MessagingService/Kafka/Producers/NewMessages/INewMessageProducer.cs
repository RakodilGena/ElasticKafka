using MessagingService.Models;

namespace MessagingService.Kafka.Producers.NewMessages;

public interface INewMessageProducer
{
    Task ProduceAsync(SendMessageRequestDto request);
}