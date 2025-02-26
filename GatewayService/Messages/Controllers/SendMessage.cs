using GatewayService.Messages.Models;
using GatewayService.Messages.Services;
using Microsoft.AspNetCore.Mvc;

namespace GatewayService.Messages.Controllers;

public partial class MessageController
{
    [HttpPost]
    public async Task<ActionResult> SendMessageAsync(
        [FromBody] SendMessageRequest request,
        [FromServices] IMessageService service)
    {
        await service.SendMessageAsync(request);
        
        return Ok();
    }
}