using GatewayService.Messages.Models.Requests;

namespace GatewayService.Messages.Mapping;

internal static class SearchMessagesRequestMapper
{
    public static SearchMessagesRequestDto ToDto(this SearchMessagesRequest request)
    {
        return new SearchMessagesRequestDto(
            Count: request.Count,
            Offset: request.Offset,
            Filter: request.Filter);
    }
}