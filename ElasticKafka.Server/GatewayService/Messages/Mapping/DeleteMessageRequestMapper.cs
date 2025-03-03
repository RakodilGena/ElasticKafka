using GatewayService.Messages.Models.Requests;

namespace GatewayService.Messages.Mapping;

internal static class DeleteMessageRequestMapper
{
    public static DeleteMessageRequestDto ToDto(this DeleteMessageRequest request)
    {
        return new DeleteMessageRequestDto(
            request.MessageId);
    }
}