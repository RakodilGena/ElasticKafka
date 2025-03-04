using MessagingService;
using StorageService.Messages.Models.Requests;

namespace StorageService.Grpc.Mapping;

internal static class DeleteMessageRequestRpcMapper
{
    public static DeleteMessageRequestDto ToDto(this DeleteMessageRequestRpc request)
    {
        return new DeleteMessageRequestDto(Guid.Parse(request.MessageId));
    }
}