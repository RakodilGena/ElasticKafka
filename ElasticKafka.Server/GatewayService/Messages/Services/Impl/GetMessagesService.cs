using GatewayService.Messages.Models.Requests;
using GatewayService.Messages.Models.Responses;
using GatewayService.Messages.RemoteServiceDiscovery.Storage;
using Grpc.Net.Client;
using MessagingService;

namespace GatewayService.Messages.Services.Impl;

//no retry policy - it's present in GetMessagesService.
internal sealed class GetMessagesService : IGetMessagesService
{
    private readonly IStorageServiceUrlProvider _urlProvider;
    private readonly ILogger<GetMessagesService> _logger;

    public GetMessagesService(
        IStorageServiceUrlProvider urlProvider,
        ILogger<GetMessagesService> logger)
    {
        _urlProvider = urlProvider;
        _logger = logger;
    }

    public async Task<GetMessagesResponse> GetMessagesAsync(
        GetMessagesRequestDto request,
        CancellationToken cancellationToken)
    {
        var url = _urlProvider.GetUrl();

        _logger.LogInformation("Getting messages from Storage Service, {url}", url);

        using var channel = GrpcChannel.ForAddress(url);
        var client = new StoredMessagesServiceRpc.StoredMessagesServiceRpcClient(channel);

        var rpcRequest = BuildRequest(request);

        var rpcResponse = await client.GetMessagesAsync(
            rpcRequest,
            cancellationToken: cancellationToken);

        var response = BuildResponse(rpcResponse);

        _logger.LogInformation("Successfully got messages from Storage Service");

        return response;
    }

    private static GetMessagesRequestRpc BuildRequest(GetMessagesRequestDto request)
    {
        var rpcRequest = new GetMessagesRequestRpc
        {
            Count = request.Count,
            Offset = request.Offset
        };

        return rpcRequest;
    }

    private static GetMessagesResponse BuildResponse(GetMessagesResponseRpc rpcResponse)
    {
        var messages = rpcResponse.Messages
            .Select(m => new MessageDto(
                Guid.Parse(m.Id),
                m.Text,
                m.SentAt.ToDateTimeOffset())).ToArray();

        var response = new GetMessagesResponse(messages);

        return response;
    }
}