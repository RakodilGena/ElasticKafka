using GatewayService.Messages.Mapping;
using GatewayService.Messages.Models;
using GatewayService.Messages.Services;
using GatewayService.Messages.Validation;
using Microsoft.AspNetCore.Mvc;

namespace GatewayService.Messages.Controllers;

public partial class MessageController
{
    [HttpPost]
    public async Task<ActionResult> SendMessageAsync(
        [FromBody] SendMessageRequest request,
        [FromServices] ICreateMessageService service)
    {
        request.Validate();

        var requestDto = request.ToDto();
        
        await service.SendMessageAsync(requestDto);
        
        return Ok();
    }
}