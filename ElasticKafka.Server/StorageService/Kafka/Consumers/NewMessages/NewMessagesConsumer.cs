using System.Text.Json;
using Confluent.Kafka;
using FluentValidation;
using Microsoft.Extensions.Options;
using StorageService.Kafka.Consumers.NewMessages.Config;
using StorageService.Kafka.Consumers.NewMessages.Models;
using StorageService.Kafka.Consumers.NewMessages.Validation;
using StorageService.Kafka.Producers.MessageCreatedEvents;
using StorageService.Messages.Models.Requests;
using StorageService.Messages.Services;

namespace StorageService.Kafka.Consumers.NewMessages;

internal sealed class NewMessagesConsumer : BackgroundService
{
    private readonly IConsumerProvider _consumerProvider;
    private readonly IOptions<NewMessagesConsumerConfig> _config;
    
    private readonly ICreateMessageService _createMessageService;
    private readonly IMessageCreatedEventProducer _messageCreatedEventProducer;
    
    private readonly ILogger<NewMessagesConsumer> _logger;

    private readonly KafkaNewMessageValidator _kafkaNewMessageValidator = new();

    public NewMessagesConsumer(
        IConsumerProvider consumerProvider, 
        IOptions<NewMessagesConsumerConfig> config, 
        ICreateMessageService createMessageService, 
        IMessageCreatedEventProducer messageCreatedEventProducer,
        ILogger<NewMessagesConsumer> logger)
    {
        _consumerProvider = consumerProvider;
        _config = config;
        _createMessageService = createMessageService;
        _messageCreatedEventProducer = messageCreatedEventProducer;
        _logger = logger;
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
            await _kafkaNewMessageValidator.ValidateAndThrowAsync(
                kafkaNewMessage,
                ct);
        }
        catch (ValidationException e)
        {
            _logger.LogError(e, "NewMessage is invalid");
            return;
        }

        _logger.LogInformation("Received NewMessage with ID: [{messageID}]", kafkaNewMessage.Id);

        bool saved = await TrySaveMessageAsync(
            kafkaNewMessage,
            ct);

        if (saved is false)
            return;

        await ProduceConfirmationAsync(
            kafkaNewMessage,
            ct);
    }

    private async Task<bool> TrySaveMessageAsync(
        KafkaNewMessage message,
        CancellationToken ct)
    {
        //dont try catch ex cz if message failed to create due to elastic fault,
        //it's important not to commit Kafka message.

        var request = new CreateMessageRequestDto(
            message.Id,
            message.Text,
            message.SentAt);
        
        return await _createMessageService.TryCreateMessageAsync(request, ct);
    }

    private async Task ProduceConfirmationAsync(
        KafkaNewMessage newMessage,
        CancellationToken ct)
    {
        try
        {
            await _messageCreatedEventProducer.ProduceAsync(newMessage.Id, ct);
        }
        catch (Exception e)
        {
            //because it's unnecessary to fail consumer completely
            //if confirmation produce failed
            _logger.LogError(e, "Failed to produce new message confirmation");
        }
    }
}