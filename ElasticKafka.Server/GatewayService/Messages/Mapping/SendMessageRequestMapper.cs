using GatewayService.Messages.Models.Requests;

namespace GatewayService.Messages.Mapping;

internal static class SendMessageRequestMapper
{
    public static SendMessageRequestDto ToDto(this SendMessageRequest request)
    {
        return new SendMessageRequestDto(
            request.MessageText.Trim(),
            request.SentAt);
    }
}