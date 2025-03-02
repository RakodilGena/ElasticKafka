using GatewayService.NewMessages.Mapping;
using GatewayService.NewMessages.Models;
using GatewayService.NewMessages.Services;
using GatewayService.NewMessages.Validation;
using Microsoft.AspNetCore.Mvc;

namespace GatewayService.NewMessages.Controllers;

public partial class MessageController
{
    [HttpPost]
    public async Task<ActionResult> SendMessageAsync(
        [FromBody] SendMessageRequest request,
        [FromServices] IMessageService service)
    {
        request.Validate();

        var requestDto = request.ToDto();
        
        await service.SendMessageAsync(requestDto);
        
        return Ok();
    }
}