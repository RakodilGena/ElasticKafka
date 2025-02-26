using MessagingService.Models;

namespace MessagingService.Kafka.Producers;

public interface IMessageProducer
{
    Task ProduceAsync(SendMessageRequestDto request);
}