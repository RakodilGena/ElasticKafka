using MessagingService;
using StorageService.Messages.Models.Requests;

namespace StorageService.Grpc.Mapping;

internal static class GetMessagesRequestRpcMapper
{
    public static GetMessagesRequestDto ToDto(this GetMessagesRequestRpc request)
    {
        return new GetMessagesRequestDto(
            Count: request.Count,
            Offset: request.Offset);
    }
}