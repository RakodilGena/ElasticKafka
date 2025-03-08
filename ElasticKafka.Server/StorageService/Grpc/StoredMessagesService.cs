using FluentValidation;
using Grpc.Core;
using MessagingService;
using StorageService.Grpc.Mapping;
using StorageService.Grpc.Validation;
using StorageService.Messages.Mapping;
using StorageService.Messages.Services;

namespace StorageService.Grpc;

internal sealed class StoredMessagesService : StoredMessagesServiceRpc.StoredMessagesServiceRpcBase
{
    private readonly IDeleteMessageService _deleteMessageService;
    private readonly IGetMessagesService _getMessagesService;
    private readonly ISearchMessagesService _searchMessagesService;
    private readonly ILogger<StoredMessagesService> _logger;

    public StoredMessagesService(
        ILogger<StoredMessagesService> logger,
        IDeleteMessageService deleteMessageService,
        IGetMessagesService getMessagesService,
        ISearchMessagesService searchMessagesService)
    {
        _logger = logger;
        _deleteMessageService = deleteMessageService;
        _getMessagesService = getMessagesService;
        _searchMessagesService = searchMessagesService;
    }


    public override async Task<DeleteMessageResponseRpc> DeleteMessage(
        DeleteMessageRequestRpc request,
        ServerCallContext context)
    {
        try
        {
            _logger.LogInformation("Processing DeleteMessage");

            var validator = new DeleteMessageRequestRpcValidator();
            await validator.ValidateAndThrowAsync(request);

            var requestDto = request.ToDto();
            var result = await _deleteMessageService.DeleteMessageAsync(requestDto);

            return new DeleteMessageResponseRpc
            {
                Success = result.Success,
                Reason = result.Reason
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "failed to delete message");
            throw new RpcException(
                new Status(StatusCode.Internal, "failed to delete message"));
        }
    }

    public override async Task<GetMessagesResponseRpc> GetMessages(GetMessagesRequestRpc request,
        ServerCallContext context)
    {
        try
        {
            _logger.LogInformation("Processing GetMessages");

            var validator = new GetMessagesRequestRpcValidator();
            await validator.ValidateAndThrowAsync(request);

            var requestDto = request.ToDto();
            var result = await _getMessagesService.GetMessagesAsync(
                requestDto,
                context.CancellationToken);

            return new GetMessagesResponseRpc
            {
                Messages = { result.Select(m => m.ToRpc()) }
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "failed to get messages");
            throw new RpcException(
                new Status(StatusCode.Internal, "failed to get messages"));
        }
    }

    public override async Task<SearchMessagesResponseRpc> SearchMessages(SearchMessagesRequestRpc request,
        ServerCallContext context)
    {
        try
        {
            _logger.LogInformation("Processing SearchMessages");

            var validator = new SearchMessagesRequestRpcValidator();
            await validator.ValidateAndThrowAsync(request);

            var requestDto = request.ToDto();
            var result = await _searchMessagesService.SearchMessagesAsync(
                requestDto,
                context.CancellationToken);

            return new SearchMessagesResponseRpc
            {
                Messages = { result.Select(m => m.ToRpc()) }
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "failed to search messages");
            throw new RpcException(
                new Status(StatusCode.Internal, "failed to search messages"));
        }
    }
}