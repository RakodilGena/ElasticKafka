using Elastic.Clients.Elasticsearch;
using StorageService.Elastic;
using StorageService.Elastic.Models;
using StorageService.Messages.Models.Requests;

namespace StorageService.Messages.Services.Impl;

internal sealed class CreateMessageService : ICreateMessageService
{
    private readonly ElasticsearchClient _elasticClient;
    private readonly ILogger<CreateMessageService> _logger;

    public CreateMessageService(
        ElasticsearchClient elasticClient,
        ILogger<CreateMessageService> logger)
    {
        _elasticClient = elasticClient;
        _logger = logger;
    }

    public async Task<bool> TryCreateMessageAsync(
        CreateMessageRequestDto request,
        CancellationToken cancellationToken)
    {
        var savedAt = DateTimeOffset.UtcNow;

        var elasticMessage = new ElasticMessage(
            request.MessageId,
            request.MessageText,
            request.MessageSentAt,
            SavedAt: savedAt);

        var response = await _elasticClient.IndexAsync(
            elasticMessage,
            idx => idx
                .Index(ElasticIndices.Messages)
                .Id(elasticMessage.Id)
                .OpType(OpType.Create), // doesn't override if already exists
            cancellationToken
        );

        if (response.IsSuccess())
        {
            _logger.LogInformation(
                "Message created successfully [ID = {id}]",
                elasticMessage.Id);
            return true;
        }

        if (response.ElasticsearchServerError?.Status is 409)
        {
            _logger.LogWarning(
                "Message already exists [ID = {id}]",
                elasticMessage.Id);

            return false;
        }

        _logger.LogError("Failed to create message [ID = {id}]", request.MessageId);
        //any other response - which is not OKCreated or Conflict
        //will raise an exception and prevent consumer from commiting message
        throw new Exception(response.DebugInformation);
    }
}