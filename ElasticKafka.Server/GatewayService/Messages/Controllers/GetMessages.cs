using FluentValidation;
using GatewayService.Messages.Mapping;
using GatewayService.Messages.Models.Requests;
using GatewayService.Messages.Services;
using GatewayService.Messages.Validation;
using Microsoft.AspNetCore.Mvc;

namespace GatewayService.Messages.Controllers;

public partial class MessageController
{
    [HttpGet]
    public async Task<ActionResult> GetMessagesAsync(
        GetMessagesRequest request,
        [FromServices] IGetMessagesService service,
        CancellationToken cancellationToken)
    {
        var validator = new GetMessagesRequestValidator();
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var requestDto = request.ToDto();
        
        await service.GetMessagesAsync(requestDto, cancellationToken);
        
        return Ok();
    }
}