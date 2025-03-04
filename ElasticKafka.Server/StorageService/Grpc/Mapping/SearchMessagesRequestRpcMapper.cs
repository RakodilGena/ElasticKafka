using MessagingService;
using StorageService.Messages.Models.Requests;

namespace StorageService.Grpc.Mapping;

internal static class SearchMessagesRequestRpcMapper
{
    public static SearchMessagesRequestDto ToDto(this SearchMessagesRequestRpc request)
    {
        return new SearchMessagesRequestDto(
            Count: request.Count,
            Offset: request.Offset,
            Filter: request.Filter.Trim());
    }
}