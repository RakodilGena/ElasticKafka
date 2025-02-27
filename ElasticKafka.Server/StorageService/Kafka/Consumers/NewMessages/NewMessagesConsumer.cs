using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using StorageService.Kafka.Consumers.NewMessages.Config;
using StorageService.Kafka.Consumers.NewMessages.Models;
using StorageService.Kafka.Consumers.NewMessages.Validation;
using StorageService.Kafka.Producers.MessageCreatedEvents;

namespace StorageService.Kafka.Consumers.NewMessages;

internal sealed class NewMessagesConsumer : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<NewMessagesConsumer> _logger;
    private readonly IConsumerProvider _consumerProvider;
    private readonly IOptions<NewMessagesConsumerConfig> _config;

    public NewMessagesConsumer(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<NewMessagesConsumer> logger,
        IConsumerProvider consumerProvider,
        IOptions<NewMessagesConsumerConfig> config)
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
        _logger.LogInformation("NewMessagesConsumer started and listening");

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
        var kafkaNewMessage = JsonSerializer.Deserialize<KafkaNewMessage>(
            consumeResult.Message.Value,
            KafkaJsonSerializerOptions.Default);

        if (kafkaNewMessage is null)
        {
            _logger.LogError("NewMessage is empty");
            return;
        }

        try
        {
            kafkaNewMessage.Validate();
        }
        catch (ValidationException e)
        {
            _logger.LogError(e, "NewMessage is invalid");
            return;
        }

        _logger.LogInformation("Received NewMessage with ID: [{messageID}]", kafkaNewMessage.Id);

        using var serviceScope = _serviceScopeFactory.CreateScope();

        bool saved = await TrySaveMessageAsync(
            serviceScope,
            kafkaNewMessage,
            ct);
        
        if (saved is false)
            return;

        await ProduceConfirmationAsync(serviceScope, kafkaNewMessage);
    }

    private Task<bool> TrySaveMessageAsync(
        IServiceScope scope,
        KafkaNewMessage kafkaNewMessage,
        CancellationToken ct)
    {
        //todo: save to elastic
        
        _logger.LogInformation("NewMessagesConsumer saved message {message}", kafkaNewMessage);

        return Task.FromResult(true);
        // var createMessageService = scope.ServiceProvider.GetRequiredService<ICreateMessageService>();
        // var newMessage = kafkaNewMessage.ToDto();
        //
        // return await createMessageService.TryCreateMessage(newMessage, ct);
    }

    private static async Task ProduceConfirmationAsync(
        IServiceScope scope,
        KafkaNewMessage newMessage)
    {
        var producer = scope.ServiceProvider.GetRequiredService<IMessageCreatedEventProducer>();
        
        var guid = Guid.Parse(newMessage.Id);
        
        await producer.ProduceAsync(guid);
    }
}