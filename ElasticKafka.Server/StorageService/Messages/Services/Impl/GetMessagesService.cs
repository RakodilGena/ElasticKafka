using Elastic.Clients.Elasticsearch;
using StorageService.Elastic;
using StorageService.Elastic.Models;
using StorageService.Messages.Mapping;
using StorageService.Messages.Models;
using StorageService.Messages.Models.Requests;

namespace StorageService.Messages.Services.Impl;

internal sealed class GetMessagesService : IGetMessagesService
{
    private readonly ElasticsearchClient _client;
    private readonly ILogger<GetMessagesService> _logger;

    public GetMessagesService(
        ElasticsearchClient client,
        ILogger<GetMessagesService> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<IReadOnlyList<MessageDto>> GetMessagesAsync(
        GetMessagesRequestDto request,
        CancellationToken cancellationToken)
    {
        var count = request.Count;
        var offset = request.Offset;

        _logger.LogInformation("Fetching messages from Elasticsearch [Count: {count}, Offset: {offset}]",
            count,
            offset);

        var response = await QueryMessagesAsync(request, cancellationToken);

        if (response.IsSuccess())
        {
            var fetchedCount = response.Documents.Count;
            _logger.LogInformation("Successfully fetched messages, count = {count}",
                fetchedCount);

            return response.Documents.Select(d => d.ToDto()).ToArray();
        }

        _logger.LogError("Failed to fetch messages");
        //any other response
        //will raise an exception
        throw new Exception(response.DebugInformation);
    }

    private async Task<SearchResponse<ElasticMessage>> QueryMessagesAsync(
        GetMessagesRequestDto request,
        CancellationToken cancellationToken)
    {
        var response = await _client.SearchAsync<ElasticMessage>(
            s => s
                .Index(ElasticIndices.Messages)
                .Size(request.Count) // Limit results
                .From(request.Offset) // Skip results
                .Sort(sort => sort.Field(
                        f => f.SentAt,
                        f => f.Order(SortOrder.Desc)) // Newest first
                ), cancellationToken);

        return response;
    }
}