using GatewayService.Messages.Models;

namespace GatewayService.Messages.Mapping;

internal static class SendMessageRequestMapper
{
    public static SendMessageRequestDto ToDto(this SendMessageRequest request)
    {
        return new SendMessageRequestDto(
            request.MessageText!,
            request.SentAt!.Value);
    }
}