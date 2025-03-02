using GatewayService.NewMessages.Models;

namespace GatewayService.NewMessages.Mapping;

internal static class SendMessageRequestMapper
{
    public static SendMessageRequestDto ToDto(this SendMessageRequest request)
    {
        return new SendMessageRequestDto(
            request.MessageText!,
            request.SentAt!.Value);
    }
}