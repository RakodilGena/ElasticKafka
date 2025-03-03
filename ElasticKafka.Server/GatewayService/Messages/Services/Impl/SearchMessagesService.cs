using GatewayService.Messages.Models.Requests;
using GatewayService.Messages.Models.Responses;
using GatewayService.Messages.RemoteServiceDiscovery.Storage;
using Grpc.Net.Client;
using MessagingService;

namespace GatewayService.Messages.Services.Impl;

//no retry policy - it's present in GetMessagesService.
internal sealed class SearchMessagesService : ISearchMessagesService
{
    private readonly IStorageServiceUrlProvider _urlProvider;
    private readonly ILogger<SearchMessagesService> _logger;

    public SearchMessagesService(
        IStorageServiceUrlProvider urlProvider,
        ILogger<SearchMessagesService> logger)
    {
        _urlProvider = urlProvider;
        _logger = logger;
    }

    public async Task<SearchMessagesResponse> SearchMessagesAsync(
        SearchMessagesRequestDto request,
        CancellationToken cancellationToken)
    {
        var url = _urlProvider.GetUrl();

        _logger.LogInformation("Searching messages from Storage Service, {url}", url);

        using var channel = GrpcChannel.ForAddress(url);
        var client = new StoredMessagesServiceRpc.StoredMessagesServiceRpcClient(channel);

        var rpcRequest = BuildRequest(request);

        var rpcResponse = await client.SearchMessagesAsync(
            rpcRequest,
            cancellationToken: cancellationToken);

        var response = BuildResponse(rpcResponse);

        _logger.LogInformation("Successfully got searched messages from Storage Service");

        return response;
    }

    private static SearchMessagesRequestRpc BuildRequest(SearchMessagesRequestDto request)
    {
        var rpcRequest = new SearchMessagesRequestRpc
        {
            Count = request.Count,
            Offset = request.Offset,
            Filter = request.Filter
        };

        return rpcRequest;
    }

    private static SearchMessagesResponse BuildResponse(SearchMessagesResponseRpc rpcResponse)
    {
        var messages = rpcResponse.Messages
            .Select(m => new MessageDto(
                Guid.Parse(m.Id),
                m.Text,
                m.SentAt.ToDateTimeOffset())).ToArray();

        var response = new SearchMessagesResponse(messages);

        return response;
    }
}