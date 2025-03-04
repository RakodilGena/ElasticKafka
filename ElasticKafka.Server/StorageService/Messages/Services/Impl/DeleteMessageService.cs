using Elastic.Clients.Elasticsearch;
using StorageService.Elastic;
using StorageService.Elastic.Models;
using StorageService.Messages.Models.Requests;
using StorageService.Messages.Models.Responses;

namespace StorageService.Messages.Services.Impl;

internal sealed class DeleteMessageService : IDeleteMessageService
{
    private readonly ElasticsearchClient _client;
    private readonly ILogger<DeleteMessageService> _logger;

    public DeleteMessageService(
        ElasticsearchClient client,
        ILogger<DeleteMessageService> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<DeleteMessageResponseDto> DeleteMessageAsync(
        DeleteMessageRequestDto request)
    {
        var response = await _client.DeleteAsync<ElasticMessage>(
            request.MessageId,
            idx => idx.Index(ElasticIndices.Messages));

        //todo: bug. returns OK even if message was not present.
        if (response.IsSuccess())
        {
            _logger.LogInformation(
                "Successfully deleted message [ID = {id}]",
                request.MessageId);

            return DeleteMessageResponseDto.OnSuccess();
        }

        if (response.ElasticsearchServerError?.Status is 404)
        {
            _logger.LogWarning(
                "Message does not exist [ID = {id}]",
                request.MessageId);

            return DeleteMessageResponseDto.OnFailure("Message does not exist");
        }

        _logger.LogError("Failed to delete message [ID = {id}]", request.MessageId);

        //any other response
        //will raise an exception to be caught in interceptor
        throw new Exception(response.DebugInformation);
    }
}