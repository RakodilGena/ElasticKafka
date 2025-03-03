using GatewayService.Messages.Models.Requests;
using GatewayService.Messages.Models.Responses;

namespace GatewayService.Messages.Services;

public interface IGetMessagesService
{
    Task<GetMessagesResponse> GetMessagesAsync(
        GetMessagesRequestDto request,
        CancellationToken cancellationToken);
}