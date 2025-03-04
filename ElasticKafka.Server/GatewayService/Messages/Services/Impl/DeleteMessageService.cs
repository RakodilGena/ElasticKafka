using GatewayService.Messages.Models.Requests;
using GatewayService.Messages.Models.Responses;
using GatewayService.Messages.RemoteServiceDiscovery.Storage;
using Grpc.Net.Client;
using MessagingService;

namespace GatewayService.Messages.Services.Impl;

//no retry policy - it's present in GetMessagesService.
internal sealed class DeleteMessageService : IDeleteMessageService
{
    private readonly IStorageServiceUrlProvider _urlProvider;
    private readonly ILogger<DeleteMessageService> _logger;

    public DeleteMessageService(
        IStorageServiceUrlProvider urlProvider,
        ILogger<DeleteMessageService> logger)
    {
        _urlProvider = urlProvider;
        _logger = logger;
    }

    public async Task<DeleteMessageResponse> DeleteMessageAsync(
        DeleteMessageRequestDto request)
    {
        var url = _urlProvider.GetUrl();

        _logger.LogInformation("Deleting message from Storage Service, {url}", url);

        using var channel = GrpcChannel.ForAddress(url);
        var client = new StoredMessagesServiceRpc.StoredMessagesServiceRpcClient(channel);

        var rpcRequest = BuildRequest(request);

        var rpcResponse = await client.DeleteMessageAsync(
            rpcRequest);

        var response = BuildResponse(rpcResponse);

        if (response.Success)
        {
            _logger.LogInformation("Successfully deleted messages from Storage Service");
        }
        else
        {
            _logger.LogInformation("Failed to delete messages from Storage Service, reason: {reason}", response.Reason);
        }


        return response;
    }

    private static DeleteMessageRequestRpc BuildRequest(DeleteMessageRequestDto request)
    {
        var rpcRequest = new DeleteMessageRequestRpc
        {
            MessageId = request.MessageId.ToString(),
        };

        return rpcRequest;
    }

    private static DeleteMessageResponse BuildResponse(DeleteMessageResponseRpc rpcResponse)
    {
        var reason = rpcResponse.Success
            ? "OK"
            : rpcResponse.Reason ?? "unknown";

        var response = new DeleteMessageResponse(
            rpcResponse.Success,
            reason);

        return response;
    }
}