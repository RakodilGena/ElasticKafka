using GatewayService.Messages.Models;
using Grpc.Net.Client;
using MessagingService;

namespace GatewayService.Messages.Services.Impl;

internal sealed class MessageService : IMessageService
{
    private readonly ILogger<MessageService> _logger;
    private readonly IMessageServiceUrlProvider _urlProvider;
    
    public MessageService(
        ILogger<MessageService> logger, 
        IMessageServiceUrlProvider urlProvider)
    {
        _logger = logger;
        _urlProvider = urlProvider;
    }

    public async Task SendMessageAsync(SendMessageRequest request)
    {
        using var channel = GrpcChannel.ForAddress(_urlProvider.GetUrl());
        var client = new MessageServiceRpc.MessageServiceRpcClient(channel);
        
        var rpcRequest = new SendMessageRequestRpc
        {
            MessageText = request.MessageText
        };
        
        await client.SendMessageAsync(rpcRequest);
    }
}