using GatewayService.Messages.Models.Requests;
using GatewayService.Messages.Models.Responses;

namespace GatewayService.Messages.Services;

public interface ISearchMessagesService
{
    Task<SearchMessagesResponse> SearchMessagesAsync(
        SearchMessagesRequestDto request,
        CancellationToken cancellationToken); 
}