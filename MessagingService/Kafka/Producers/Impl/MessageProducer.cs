using System.Text.Json;
using Confluent.Kafka;
using JetBrains.Annotations;
using MessagingService.Models;
using Microsoft.Extensions.Options;

namespace MessagingService.Kafka.Producers.Impl;

internal sealed class MessageProducer : IMessageProducer
{
    private readonly IProducerProvider _producerProvider;
    private readonly IOptions<KafkaProducerConfig> _config;
    private readonly ILogger<MessageProducer> _logger;

    public MessageProducer(
        IProducerProvider producerProvider,
        IOptions<KafkaProducerConfig> config, 
        ILogger<MessageProducer> logger)
    {
        _producerProvider = producerProvider;
        _config = config;
        _logger = logger;
    }

    public async Task ProduceAsync(SendMessageRequestDto request)
    {
        var messageId = request.MessageId.ToString();
        
        _logger.LogInformation(
            "Producing new message [{messageId}] to kafka", 
            messageId);

        var producer = _producerProvider.Get();
        
        var kafkaMessage = ToKafka(messageId, request.MessageText);
        
        await producer.ProduceAsync(_config.Value.NewMessagesTopic, kafkaMessage);

        _logger.LogInformation("New message [{messageId}] produced successfully", messageId);
    }

    private static Message<string, string> ToKafka(
        string messageId,
        string messageText)
    {
        var kafkaOrder = new KafkaNewMessage(messageId, messageText);

        var value = JsonSerializer.Serialize(
            kafkaOrder, 
            KafkaJsonSerializerOptions.Default);

        return new Message<string, string>
        {
            Key = messageId,
            Value = value
        };
    }
    
    [UsedImplicitly]
    private sealed record KafkaNewMessage( 
        string Id,
        string Text);
}