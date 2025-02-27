using Confluent.Kafka;

namespace StorageService.Kafka.Consumers.Impl;

internal sealed class ConsumerProvider : IConsumerProvider
{
    private readonly ILogger<ConsumerProvider> _logger;

    public ConsumerProvider(ILogger<ConsumerProvider> logger)
    {
        _logger = logger;
    }

    public IConsumer<string, string> Create(ConsumerConfig config)
    {
        return new ConsumerBuilder<string, string>(
                config)
            .SetPartitionsAssignedHandler(
                (_, topicPartitions) =>
                    _logger.LogInformation(
                        "Partition assigned: {TopicPartitions}",
                        string.Join(
                            Environment.NewLine,
                            topicPartitions
                                .Select(part => $"{part.Topic}: {part.Partition.ToString()}"))))
            .Build();
    }
}