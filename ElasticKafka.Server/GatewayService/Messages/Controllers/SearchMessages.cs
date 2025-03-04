using FluentValidation;
using GatewayService.Messages.Mapping;
using GatewayService.Messages.Models.Requests;
using GatewayService.Messages.Models.Responses;
using GatewayService.Messages.Services;
using GatewayService.Messages.Validation;
using Microsoft.AspNetCore.Mvc;

namespace GatewayService.Messages.Controllers;

public partial class MessageController
{
    [HttpGet("search")]
    public async Task<ActionResult<SearchMessagesResponse>> SearchMessagesAsync(
        SearchMessagesRequest request,
        [FromServices] ISearchMessagesService service,
        CancellationToken cancellationToken)
    {
        var validator = new SearchMessagesRequestValidator();
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var requestDto = request.ToDto();
        
        return await service.SearchMessagesAsync(requestDto, cancellationToken);
    }
}