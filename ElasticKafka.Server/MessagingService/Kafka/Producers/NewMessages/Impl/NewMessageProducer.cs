using System.Text.Json;
using Confluent.Kafka;
using JetBrains.Annotations;
using MessagingService.Kafka.Producers.Config;
using MessagingService.Models;
using Microsoft.Extensions.Options;

namespace MessagingService.Kafka.Producers.NewMessages.Impl;

internal sealed class NewMessageProducer : INewMessageProducer
{
    private readonly IProducerProvider _producerProvider;
    private readonly IOptions<KafkaProducerConfig> _config;
    private readonly ILogger<NewMessageProducer> _logger;

    public NewMessageProducer(
        IProducerProvider producerProvider,
        IOptions<KafkaProducerConfig> config,
        ILogger<NewMessageProducer> logger)
    {
        _producerProvider = producerProvider;
        _config = config;
        _logger = logger;
    }

    public async Task ProduceAsync(
        SendMessageRequestDto request,
        CancellationToken ct)
    {
        var messageId = request.MessageId;

        _logger.LogInformation(
            "Producing new message [{messageId}] to kafka",
            messageId);

        var producer = _producerProvider.Get();

        var kafkaMessage = ToKafka(request);

        await ProduceMessageWithRetryAsync(producer, kafkaMessage, ct);

        _logger.LogInformation(
            "New message [{messageId}] produced successfully",
            messageId);
    }

    private static Message<string, string> ToKafka(
        SendMessageRequestDto request)
    {
        var kafkaOrder = new KafkaNewMessage(
            request.MessageId,
            request.MessageText,
            request.SentAt);

        var value = JsonSerializer.Serialize(
            kafkaOrder,
            KafkaJsonSerializerOptions.Default);

        //key is there to determine the partition (hash -> partition)
        return new Message<string, string>
        {
            Key = request.MessageId.ToString(),
            Value = value
        };
    }

    private async Task ProduceMessageWithRetryAsync(
        IProducer<string, string> producer,
        Message<string, string> message,
        CancellationToken ct)
    {
        var currentAttempt = 0;
        const int maxAttempts = 3;

        while (true)
        {
            currentAttempt++;
            try
            {
                await producer.ProduceAsync(
                    _config.Value.NewMessagesTopic,
                    message,
                    ct);

                return;
            }
            catch (Exception e)
            {
                if (currentAttempt < maxAttempts)
                {
                    _logger.LogError(e,
                        "Failed to produce message to kafka, attempt: {curAtt}. Retrying...",
                        currentAttempt);

                    continue;
                }

                _logger.LogError(e,
                    "Failed to produce message to kafka, total attempts: {curAtt}",
                    currentAttempt);

                throw new Exception(
                    $"Failed to produce message to kafka, total attempts: {currentAttempt}");
            }
        }
    }

    [UsedImplicitly]
    private sealed record KafkaNewMessage(
        Guid Id,
        string Text,
        DateTimeOffset SentAt);
}