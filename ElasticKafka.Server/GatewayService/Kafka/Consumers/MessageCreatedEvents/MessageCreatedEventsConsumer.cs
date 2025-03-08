using Confluent.Kafka;
using GatewayService.Kafka.Consumers.MessageCreatedEvents.Config;
using GatewayService.SignalR;
using Microsoft.Extensions.Options;

namespace GatewayService.Kafka.Consumers.MessageCreatedEvents;

internal sealed class MessageCreatedEventsConsumer : BackgroundService
{
    private readonly IOptions<MessageCreatedEventConsumerConfig> _config;
    private readonly IConsumerProvider _consumerProvider;
    private readonly ILogger<MessageCreatedEventsConsumer> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public MessageCreatedEventsConsumer(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<MessageCreatedEventsConsumer> logger,
        IConsumerProvider consumerProvider,
        IOptions<MessageCreatedEventConsumerConfig> config)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
        _consumerProvider = consumerProvider;
        _config = config;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Yield();

        while (stoppingToken.IsCancellationRequested is false)
        {
            using var consumer = _consumerProvider.Create(_config.Value.Config);

            try
            {
                consumer.Subscribe(_config.Value.Topic);

                await ConsumeCycle(consumer, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Consumer error");

                try
                {
                    consumer.Unsubscribe();
                }
                catch (Exception unsubEx)
                {
                    _logger.LogCritical(unsubEx, "Consumer unsub error");
                }

                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }
        }
    }

    private async Task ConsumeCycle(
        IConsumer<string, string> consumer,
        CancellationToken ct)
    {
        _logger.LogInformation("MessageCreatedEventsConsumer started and listening");

        while (ct.IsCancellationRequested is false)
        {
            var consumeResult = consumer.Consume(ct);

            await HandleAsync(consumeResult, ct);

            consumer.Commit();
        }
    }

    private async Task HandleAsync(
        ConsumeResult<string, string> consumeResult,
        CancellationToken ct)
    {
        var idString = consumeResult.Message.Value;

        if (string.IsNullOrWhiteSpace(idString))
        {
            _logger.LogError("message id is empty");
            return;
        }

        var valid = Guid.TryParse(idString, out var messageId);

        if (valid is false)
        {
            _logger.LogError("message id is invalid [{idString}]", idString);
            return;
        }

        _logger.LogInformation("Received MessageCreatedEvent with ID: [{messageID}]", messageId);

        await SendMessageCreatedNotificationAsync(messageId, ct);
    }

    private async Task SendMessageCreatedNotificationAsync(
        Guid messageId,
        CancellationToken ct)
    {
        _logger.LogInformation("MessageCreatedEventsConsumer sending notification about created message [{messageId}]",
            messageId);

        using var scope = _serviceScopeFactory.CreateScope();
        var hub = scope.ServiceProvider.GetRequiredService<IMessageCreatedEventsHubWrapper>();

        await hub.NotifyMessageCreated(messageId, ct);
    }
}