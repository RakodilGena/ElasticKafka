﻿using Confluent.Kafka;
using Microsoft.Extensions.Options;
using StorageService.Kafka.Producers.Config;

namespace StorageService.Kafka.Producers.MessageCreatedEvents.Impl;

internal sealed class MessageCreatedEventProducer : IMessageCreatedEventProducer
{
    private readonly IProducerProvider _producerProvider;
    private readonly IOptions<KafkaProducerConfig> _config;
    private readonly ILogger<MessageCreatedEventProducer> _logger;

    public MessageCreatedEventProducer(
        IProducerProvider producerProvider,
        IOptions<KafkaProducerConfig> config,
        ILogger<MessageCreatedEventProducer> logger)
    {
        _producerProvider = producerProvider;
        _config = config;
        _logger = logger;
    }

    public async Task ProduceAsync(Guid messageId, CancellationToken ct = default)
    {
        _logger.LogInformation(
            "Producing message created event [Message ID:{messageId}] to kafka",
            messageId);

        var producer = _producerProvider.Get();

        var kafkaMessage = ToKafka(messageId);

        await ProduceMessageWithRetryAsync(producer, kafkaMessage, ct);

        _logger.LogInformation(
            "Message created event [Message ID:{messageId}] produced successfully",
            messageId);
    }

    private static Message<string, string> ToKafka(
        Guid messageId)
    {
        var idString = messageId.ToString();

        //key is there to determine the partition (hash -> partition)
        return new Message<string, string>
        {
            Key = idString,
            Value = idString
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
                    _config.Value.MessageCreatedEventsTopic,
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

                throw;
            }
        }
    }
}