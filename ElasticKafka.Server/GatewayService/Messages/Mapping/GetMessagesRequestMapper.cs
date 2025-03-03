using GatewayService.Messages.Models.Requests;

namespace GatewayService.Messages.Mapping;

internal static class GetMessagesRequestMapper
{
    public static GetMessagesRequestDto ToDto(this GetMessagesRequest request)
    {
        return new GetMessagesRequestDto(
            Count: request.Count,
            Offset: request.Offset);
    }
}