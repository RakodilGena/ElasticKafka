using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using StorageService.Kafka.NewMessages.Config;
using StorageService.Kafka.NewMessages.Models;
using StorageService.Kafka.NewMessages.Validation;

namespace StorageService.Kafka.NewMessages;

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
            return;

        //todo: validate

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

        await ProduceConfirmationAsync(serviceScope, kafkaNewMessage.Id);
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

    private Task ProduceConfirmationAsync(
        IServiceScope scope,
        string newMessageId)
    {
        //todo: create kafka event to gateway
        return Task.CompletedTask;

        // var producer = scope.ServiceProvider.GetRequiredService<INewMessageConfirmationProducer>();
        // await producer.ProduceAsync(newMessageId, CancellationToken.None);
    }
}